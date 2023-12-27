using B3.Worker.Data.Entities;

namespace B3.Worker.Data.Interfaces
{
    public interface IOrderRepository
    {
        Task<IList<OrderEntity>> GetMonitorValues();
        Task SaveOrder(OrderEntity orderEntity);
    }
}