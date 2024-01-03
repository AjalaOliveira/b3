using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace B3.Worker.Service.Process
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IAppSettings _appSettings;
        private readonly IOrderRepository _orderRepository;
        private readonly IWebsocketTools _websocketTools;

        public OrderService(
            ILogger<OrderService> logger,
            IAppSettings appSettings,
            IOrderRepository orderRepository,
            IWebsocketTools websocketTools)
        {
            _logger = logger;
            _appSettings = appSettings;
            _orderRepository = orderRepository;
            _websocketTools = websocketTools;
        }

        public async Task BtcUsdExecute()
        {
            try
            {
                await ConnectWebSocketAsync(_appSettings.GetBitstampUrl(), _appSettings.GetBtcUsdMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante execução do serviço: {message}", ex.Message);
                throw;
            }
        }

        public async Task EthUsdExecute()
        {
            try
            {
                await ConnectWebSocketAsync(_appSettings.GetBitstampUrl(), _appSettings.GetEthUsdMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante execução do serviço: {message}", ex.Message);
                throw;
            }
        }

        public async Task ConnectWebSocketAsync(string uri, string requestMessage)
        {
            using ClientWebSocket webSocket = new();
            await _websocketTools.GetConnectionAsync(uri, webSocket);
            await _websocketTools.SendWebSocketMessage(webSocket, requestMessage);

            while (webSocket.State == WebSocketState.Open)
            {
                string message = await _websocketTools.ReceiveWebSocketMessage(webSocket);
                var messageSerializedJson = JsonConvert.DeserializeObject<MessageDeserialized>(message);

                if (message.StartsWith("{\"data") && messageSerializedJson != null)
                {
                    List<OrderEntity> bids = _websocketTools.BuildDataSetToBeSaved(messageSerializedJson);
                    await _orderRepository.SaveOrder(bids);
                }
            }
        }
    }
}