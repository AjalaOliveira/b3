using B3.Worker.Data.Entities;

namespace B3.Worker.Service.Interfaces.Services
{
    public interface IMonitorService
    {
        Task MonitorExecute();
        Task<IList<MessageDeserialized>> GetWebsocketCurrentData();
        Task<MessageDeserialized> ConnectWebSocketAsync(string uri, string requestMessage);
        Task<IList<OrderEntity>> GetlastFiveSecondsRegisters();
    }
}