using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Enums;
using B3.Worker.Shared.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace B3.Worker.Service.Services
{
    public class MonitorService : IMonitorService
    {
        private readonly ILogger<MonitorService> _logger;
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        private readonly IOrderRepository _orderRepository;

        public MonitorService(
            ILogger<MonitorService> logger,
            IOptionsMonitor<AppSettings> appSettings,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _appSettings = appSettings;
            _orderRepository = orderRepository;
        }

        public async Task MonitorExecute()
        {
            try
            {
                var websocketCurrentData = await GetWebsocketCurrentData();
                var lastFiveSecondsRegisters = await GetlastFiveSecondsRegisters();

                ShowMonitorValues(websocketCurrentData, lastFiveSecondsRegisters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante execução do serviço: {message}", ex.Message);
            }
        }

        #region Private Methods

        private async Task<IList<MessageDeserialized>> GetWebsocketCurrentData()
        {
            var orderList = new List<MessageDeserialized>
            {
                await ConnectWebSocketAsync(_appSettings.CurrentValue.BitstampUrl, _appSettings.CurrentValue.BtcUsdMessage),
                await ConnectWebSocketAsync(_appSettings.CurrentValue.BitstampUrl, _appSettings.CurrentValue.EthUsdMessage)
            };

            return orderList;
        }

        public static async Task<MessageDeserialized> ConnectWebSocketAsync(string uri, string requestMessage)
        {
            using (ClientWebSocket webSocket = new())
            {
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);

                await SendWebSocketMessage(webSocket, requestMessage);

                while (webSocket.State == WebSocketState.Open)
                {
                    string receivedMessage = await ReceiveWebSocketMessage(webSocket);

                    var orderEntity = JsonConvert.DeserializeObject<MessageDeserialized>(receivedMessage);

                    if (receivedMessage.StartsWith("{\"data") && orderEntity != null)
                        return orderEntity;
                }
                return await ConnectWebSocketAsync(uri, requestMessage);
            }
        }

        static async Task SendWebSocketMessage(ClientWebSocket webSocket, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        static async Task<string> ReceiveWebSocketMessage(ClientWebSocket webSocket)
        {
            var buffer = new byte[20000];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        private async Task<IList<OrderEntity>> GetlastFiveSecondsRegisters()
        {
            return await _orderRepository.GetMonitorValues();
        }

        private void ShowMonitorValues(IList<MessageDeserialized> websocket, IList<OrderEntity> database)
        {

            if (websocket.Count > 0)
            {
                //Console.WriteLine($"WEBSOCKET TIMESTAMP IN DATETIME     ------------------------------------------------------------------------------------>  '{DateTimeTools.SetDateTimeFromTimestamp((long)Convert.ToDouble(websocket.FirstOrDefault().data.timestamp))}'");
                //Console.WriteLine($"WEBSOCKET TOTAL                     ------------------------------------------------------------------------------------>  '{websocket.Count()}'");
                Console.WriteLine("");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX     INSTANTE ATUAL     XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                ShowWebsocketCurrentData(websocket);
            }

            if (database.Count > 0)
            {
                //Console.WriteLine($"DATABASE TIMESTAMP IN DATETIME      ------------------------------------------------------------------------------------>  '{DateTimeTools.SetDateTimeFromTimestamp((long)Convert.ToDouble(database.FirstOrDefault().timestamp))}'");
                //Console.WriteLine($"DATABASE TOTAL                      ------------------------------------------------------------------------------------>  '{database.Count()}'");
                Console.WriteLine("");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine("");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX     ÚLTIMOS '{_appSettings.CurrentValue.ExecutionIntervalMiliseconds / 1000}' SEGUNDOS     XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine("");

                ShowLastFiveMinutesData(database);
            }
        }

        private static void ShowWebsocketCurrentData(IList<MessageDeserialized> messages)
        {
            foreach (var item in messages)
            {
                var lowestPrice = Convert.ToDecimal(item.data.bids.First().Last()) / Convert.ToDecimal(item.data.bids.First().First());
                var averagePrice = Convert.ToDecimal(item.data.bids.First().Last()) / Convert.ToDecimal(item.data.bids.First().First());
                var highestPrice = lowestPrice;

                foreach (var bid in item.data.bids)
                {
                    averagePrice += Convert.ToDecimal(bid.Last()) / Convert.ToDecimal(bid.First());
                    var price = Convert.ToDecimal(bid.Last()) / Convert.ToDecimal(bid.First());

                    if (price < lowestPrice)
                        lowestPrice = price;

                    if (price > highestPrice)
                        highestPrice = price;
                }

                Console.WriteLine("");
                Console.WriteLine(item.channel.ToUpper());
                Console.WriteLine($"Menor Preço: {lowestPrice}");
                Console.WriteLine($"Maior Preço: {highestPrice}");
                Console.WriteLine($"Média de preço: {averagePrice / item.data.bids.Count}");
            }
        }

        private static void ShowLastFiveMinutesData(IList<OrderEntity> orders)
        {
            var averagePriceBtcUsd = orders.Where(x => x.channel == CurrencyPair.BtcUsd)
                                           .Average(x => x.orderValue);

            Console.WriteLine($"Média de preço acumulado no últimos 5 segundo do ativo BTC/USD: {averagePriceBtcUsd}");

            var averageQuantityBtcUsd = orders.Where(x => x.channel == CurrencyPair.BtcUsd)
                                              .Average(x => x.amount);

            Console.WriteLine($"Média de quantidade acumulada no últimos 5 segundo do ativo BTC/USD: {averageQuantityBtcUsd}");

            var averagePriceEthUsd = orders.Where(x => x.channel == CurrencyPair.EthUsd)
                                           .Average(x => x.orderValue);

            Console.WriteLine("");
            Console.WriteLine($"Média de preço acumulado no últimos 5 segundo do ativo ETH/USD: {averagePriceEthUsd}");

            var averageQuantityEthUsd = orders.Where(x => x.channel == CurrencyPair.EthUsd)
                                              .Average(x => x.amount);

            Console.WriteLine($"Média de quantidade acumulada no últimos 5 segundo do ativo ETH/USD: {averageQuantityEthUsd}");

            #endregion
        }
    }
}