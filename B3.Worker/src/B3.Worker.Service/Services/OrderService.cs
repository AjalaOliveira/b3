using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Enums;
using B3.Worker.Shared.Settings;
using B3.Worker.Shared.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
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

        public async Task ConnectWebSocketAsync(IOrderRepository orderRepository, string uri, string requestMessage)
        {
            using (ClientWebSocket webSocket = new())
            {
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);

                await SendWebSocketMessage(webSocket, requestMessage);

                while (webSocket.State == WebSocketState.Open)
                {
                    string message = await ReceiveWebSocketMessage(webSocket);

                    var messageSerializedJson = JsonConvert.DeserializeObject<MessageDeserialized>(message);

                    if (message.StartsWith("{\"data") && messageSerializedJson != null)
                    {
                        List<OrderEntity> bids = BuildDataSetToBeSaved(messageSerializedJson);

                        await orderRepository.SaveOrder(bids);
                    }
                }
            }
        }

        public async Task SendWebSocketMessage(ClientWebSocket webSocket, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> ReceiveWebSocketMessage(ClientWebSocket webSocket)
        {
            var buffer = new byte[20000];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        public List<OrderEntity> BuildDataSetToBeSaved(MessageDeserialized? messageSerializedJson)
        {
            var dateTimeTools = new DateTimeTools();

            var bids = messageSerializedJson.data.bids.Select(x => new OrderEntity
            {
                dateTime = dateTimeTools.SetDateTimeFromTimestamp((long)Convert.ToDouble(messageSerializedJson.data.timestamp)),
                price = Convert.ToDecimal(x.First(), CultureInfo.InvariantCulture),
                amount = Convert.ToDecimal(x.Last(), CultureInfo.InvariantCulture),
                orderValue = Convert.ToDecimal(x.Last(), CultureInfo.InvariantCulture) / Convert.ToDecimal(x.First(), CultureInfo.InvariantCulture),
                orderType = OrderType.Bid,
                timestamp = messageSerializedJson.data != null ? (long)Convert.ToDouble(messageSerializedJson.data.timestamp) : 0,
                microtimestamp = messageSerializedJson.data != null ? (long)Convert.ToDouble(messageSerializedJson.data.microtimestamp) : 0,
                channel = Enum.GetValues(typeof(CurrencyPair)).Cast<CurrencyPair>().FirstOrDefault(v => v.GetDescription() == messageSerializedJson.channel)
            }).ToList();

            var asks = messageSerializedJson.data.asks.Select(x => new OrderEntity
            {
                dateTime = dateTimeTools.SetDateTimeFromTimestamp((long)Convert.ToDouble(messageSerializedJson.data.timestamp)),
                price = Convert.ToDecimal(x.First(), CultureInfo.InvariantCulture),
                amount = Convert.ToDecimal(x.Last(), CultureInfo.InvariantCulture),
                orderValue = Convert.ToDecimal(x.Last(), CultureInfo.InvariantCulture) / Convert.ToDecimal(x.First(), CultureInfo.InvariantCulture),
                orderType = OrderType.Ask,
                timestamp = messageSerializedJson.data != null ? (long)Convert.ToDouble(messageSerializedJson.data.timestamp) : 0,
                microtimestamp = messageSerializedJson.data != null ? (long)Convert.ToDouble(messageSerializedJson.data.microtimestamp) : 0,
                channel = Enum.GetValues(typeof(CurrencyPair)).Cast<CurrencyPair>().FirstOrDefault(v => v.GetDescription() == messageSerializedJson.channel)
            }).ToList();

            bids.AddRange(asks);

            return bids;
        }
    }
}