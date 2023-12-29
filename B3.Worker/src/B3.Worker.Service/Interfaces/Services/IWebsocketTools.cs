using B3.Worker.Data.Entities;
using System.Net.WebSockets;

namespace B3.Worker.Service.Interfaces.Services
{
    public interface IWebsocketTools
    {
        Task GetConnectionAsync(string uri, ClientWebSocket webSocket);

        Task SendWebSocketMessage(ClientWebSocket webSocket, string message);

        Task<string> ReceiveWebSocketMessage(ClientWebSocket webSocket);

        List<OrderEntity> BuildDataSetToBeSaved(MessageDeserialized? messageSerializedJson);
    }
}