namespace B3.Worker.Service.Interfaces.Services
{
    public interface IOrderService
    {
        Task BtcUsdExecute();
        Task EthUsdExecute();
        Task MonitorExecute();
    }
}