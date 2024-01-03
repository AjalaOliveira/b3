using B3.Worker.Data.Entities;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Enums;
using B3.Worker.Shared.Utils;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;

namespace B3.Worker.Service.Services
{
    public class WebsocketTools : IWebsocketTools
    {
        public async Task GetConnectionAsync(string uri, ClientWebSocket webSocket)
        {
            await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
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
                orderValue = Convert.ToDecimal(x.First(), CultureInfo.InvariantCulture) * Convert.ToDecimal(x.Last(), CultureInfo.InvariantCulture),
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
                orderValue = Convert.ToDecimal(x.First(), CultureInfo.InvariantCulture) * Convert.ToDecimal(x.Last(), CultureInfo.InvariantCulture),
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