using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Enums;
using B3.Worker.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace B3.Worker.Service.Services
{
    public class MonitorService : IMonitorService
    {
        private readonly ILogger<MonitorService> _logger;
        private readonly IAppSettings _appSettings;
        private readonly IOrderRepository _orderRepository;
        private readonly IWebsocketTools _websocketTools;

        public MonitorService(
            ILogger<MonitorService> logger,
            IAppSettings appSettings,
            IOrderRepository orderRepository,
            IWebsocketTools websocketTools)
        {
            _logger = logger;
            _appSettings = appSettings;
            _orderRepository = orderRepository;
            _websocketTools = websocketTools;
        }

        public async Task MonitorExecute()
        {
            try
            {
                var websocketCurrentData = await GetWebsocketCurrentData();
                var lastFiveSecondsRegisters = await GetlastFiveSecondsRegisters();

                await ShowMonitorValues(websocketCurrentData, lastFiveSecondsRegisters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante execução do serviço: {message}", ex.Message);
            }
        }

        public async Task<IList<MessageDeserialized>> GetWebsocketCurrentData()
        {
            var orderList = new List<MessageDeserialized>
            {
                await ConnectWebSocketAsync(_appSettings.GetBitstampUrl(), _appSettings.GetBtcUsdMessage()),
                await ConnectWebSocketAsync(_appSettings.GetBitstampUrl(), _appSettings.GetEthUsdMessage())
            };

            return orderList;
        }

        public async Task<MessageDeserialized> ConnectWebSocketAsync(string uri, string requestMessage)
        {
            using ClientWebSocket webSocket = new();
            await _websocketTools.GetConnectionAsync(uri, webSocket);
            await _websocketTools.SendWebSocketMessage(webSocket, requestMessage);

            while (webSocket.State == WebSocketState.Open)
            {
                string receivedMessage = await _websocketTools.ReceiveWebSocketMessage(webSocket);

                var orderEntity = JsonConvert.DeserializeObject<MessageDeserialized>(receivedMessage);

                if (receivedMessage.StartsWith("{\"data") && orderEntity != null)
                    return orderEntity;
            }
            return null;
        }

        public async Task<IList<OrderEntity>> GetlastFiveSecondsRegisters()
        {
            return await _orderRepository.GetMonitorValues();
        }

        public async Task ShowMonitorValues(IList<MessageDeserialized> websocket, IList<OrderEntity> database)
        {
            //var dateTimeTool = new DateTimeTools();

            if (websocket.Count > 0)
            {
                //Console.WriteLine($"WEBSOCKET TIMESTAMP IN DATETIME     ------------------------------------------------------------------------------------>  '{dateTimeTool.SetDateTimeFromTimestamp((long)Convert.ToDouble(websocket.FirstOrDefault().data.timestamp))}'");
                //Console.WriteLine($"WEBSOCKET TOTAL                     ------------------------------------------------------------------------------------>  '{websocket.Count()}'");
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now.ToString());
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX     INSTANTE ATUAL     XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine("");
                await ShowWebsocketCurrentData(websocket);
            }

            if (database.Count > 0)
            {
                //Console.WriteLine($"DATABASE TIMESTAMP IN DATETIME      ------------------------------------------------------------------------------------>  '{dateTimeTool.SetDateTimeFromTimestamp((long)Convert.ToDouble(database.FirstOrDefault().timestamp))}'");
                //Console.WriteLine($"DATABASE TOTAL                      ------------------------------------------------------------------------------------>  '{database.Count()}'");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX     ÚLTIMOS '{_appSettings.GetExecutionIntervalMiliseconds() / 1000}' SEGUNDOS     XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine("");
                await ShowLastFiveMinutesData(database);
                Console.WriteLine("");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            }
        }

        public async Task ShowWebsocketCurrentData(IList<MessageDeserialized> messages)
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

                Console.WriteLine(item.channel.ToUpper());
                Console.WriteLine($"Menor Preço: {lowestPrice}");
                Console.WriteLine($"Maior Preço: {highestPrice}");
                Console.WriteLine($"Média de preço: {averagePrice / item.data.bids.Count}");
                Console.WriteLine("");
            }

            await Task.CompletedTask;
        }

        public async Task ShowLastFiveMinutesData(IList<OrderEntity> orders)
        {
            var averagePriceBtcUsd = orders.Where(x => x.channel == CurrencyPair.BtcUsd).Average(x => x.orderValue);

            Console.WriteLine($"Média de preço acumulado no últimos 5 segundo do ativo BTC/USD: {averagePriceBtcUsd}");

            var averageQuantityBtcUsd = orders.Where(x => x.channel == CurrencyPair.BtcUsd).Average(x => x.amount);

            Console.WriteLine($"Média de quantidade acumulada no últimos 5 segundo do ativo BTC/USD: {averageQuantityBtcUsd}");

            var averagePriceEthUsd = orders.Where(x => x.channel == CurrencyPair.EthUsd).Average(x => x.orderValue);

            Console.WriteLine("");
            Console.WriteLine($"Média de preço acumulado no últimos 5 segundo do ativo ETH/USD: {averagePriceEthUsd}");

            var averageQuantityEthUsd = orders.Where(x => x.channel == CurrencyPair.EthUsd).Average(x => x.amount);

            Console.WriteLine($"Média de quantidade acumulada no últimos 5 segundo do ativo ETH/USD: {averageQuantityEthUsd}");

            await Task.CompletedTask;
        }
    }
}