using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Settings;
using B3.Worker.Shared.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace B3.Worker.Service.Process
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        private readonly IOrderRepository _orderRepository;

        public OrderService(
            ILogger<OrderService> logger,
            IOptionsMonitor<AppSettings> appSettings,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _appSettings = appSettings;
            _orderRepository = orderRepository;
        }

        public async Task BtcUsdExecute()
        {
            try
            {
                await ConnectWebSocketAsync(_orderRepository, _appSettings.CurrentValue.BitstampUrl, _appSettings.CurrentValue.BtcUsdMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante execução do serviço: {message}", ex.Message);
            }
        }

        public async Task EthUsdExecute()
        {
            try
            {
                await ConnectWebSocketAsync(_orderRepository, _appSettings.CurrentValue.BitstampUrl, _appSettings.CurrentValue.EthUsdMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante execução do serviço: {message}", ex.Message);
            }
        }

        #region Private Methods

        static async Task ConnectWebSocketAsync(IOrderRepository orderRepository, string uri, string requestMessage)
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
                        orderEntity.dateTime = DateTimeTools.SetDateTimeFromTimestamp((long)Convert.ToDouble(orderEntity.data.timestamp));
                        await orderRepository.SaveOrder(orderEntity);
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

        #endregion
    }
}