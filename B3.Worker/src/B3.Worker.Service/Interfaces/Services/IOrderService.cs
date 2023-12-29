using B3.Worker.Data.Entities;

namespace B3.Worker.Service.Interfaces.Services
{
    public interface IOrderService
    {
        Task BtcUsdExecute();
        Task EthUsdExecute();
        Task ConnectWebSocketAsync(string uri, string requestMessage);
    }
}