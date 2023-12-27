using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Interfaces.Services;
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
                await ConnectWebSocketAsync(_appSettings.CurrentValue.BitstampUrl, _appSettings.CurrentValue.BtcUsdMessage);
                await ConnectWebSocketAsync(_appSettings.CurrentValue.BitstampUrl, _appSettings.CurrentValue.EthUsdMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante execução do serviço: {message}", ex.Message);
            }
        }

        #region Private Methods

        public static async Task ConnectWebSocketAsync(string uri, string requestMessage)
        {
            using (ClientWebSocket webSocket = new())
            {
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);

                await SendWebSocketMessage(webSocket, requestMessage);

                while (webSocket.State == WebSocketState.Open)
                {
                    string receivedMessage = await ReceiveWebSocketMessage(webSocket);

                    var orderEntity = JsonConvert.DeserializeObject<OrderEntity>(receivedMessage);

                    if (receivedMessage.StartsWith("{\"data") && orderEntity != null)
                    {
                        ShowMonitorValues(orderEntity);
                        break;
                    }
                }
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

        private async Task<IList<OrderEntity>> GetMonitorValues()
        {
            return await _orderRepository.GetMonitorValues();
        }

        private static void ShowMonitorValues(OrderEntity order)
        {
            var lowestPrice = Convert.ToDecimal(order.data.bids.First().Last()) / Convert.ToDecimal(order.data.bids.First().First());

            var averagePrice = Convert.ToDecimal(order.data.bids.First().Last()) / Convert.ToDecimal(order.data.bids.First().First());

            var highestPrice = lowestPrice;

            foreach (var item in order.data.bids)
            {
                averagePrice += Convert.ToDecimal(item.Last()) / Convert.ToDecimal(item.First());
                var price = Convert.ToDecimal(item.Last()) / Convert.ToDecimal(item.First());

                if (price < lowestPrice)
                    lowestPrice = price;

                if (price > highestPrice)
                    highestPrice = price;
            }

            Console.WriteLine("");
            Console.WriteLine(order.channel.ToUpper());
            Console.WriteLine($"Websocket Message datetime: {order.dateTime}");
            Console.WriteLine($"Menor Preço: {lowestPrice}");
            Console.WriteLine($"Maior Preço: {highestPrice}");
            Console.WriteLine($"Média de preço entre todos: {averagePrice / order.data.bids.Count}");
            Console.WriteLine("");
        }

        #endregion
    }
}