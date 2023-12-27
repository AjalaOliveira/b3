using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Shared.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace B3.Worker.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ILogger<OrderRepository> _logger;
        private readonly IOptionsMonitor<MongoDBSettings> _mongoDBSettings;
        private readonly IOptionsMonitor<AppSettings> _appSettings;

        public OrderRepository(
            ILogger<OrderRepository> logger,
            IOptionsMonitor<MongoDBSettings> mongoDBSettings,
            IOptionsMonitor<AppSettings> appSettings)
        {
            _logger = logger;
            _mongoDBSettings = mongoDBSettings;
            _appSettings = appSettings;
        }

        public async Task SaveOrder(OrderEntity orderEntity)
        {
            MongoClient client = new(_mongoDBSettings.CurrentValue.ConnectionString);
            var playlistCollection = client.GetDatabase(_mongoDBSettings.CurrentValue.DatabaseName).GetCollection<OrderEntity>("orders");
            await playlistCollection.InsertOneAsync(orderEntity);
        }

        public async Task<IList<OrderEntity>> GetMonitorValues()
        {
            MongoClient client = new(_mongoDBSettings.CurrentValue.ConnectionString);
            var playlistCollection = client.GetDatabase(_mongoDBSettings.CurrentValue.DatabaseName).GetCollection<OrderEntity>("orders");

            var query = (from e in playlistCollection.AsQueryable<OrderEntity>()
                         where e.dateTime >= DateTime.Now.AddMilliseconds(-_appSettings.CurrentValue.ExecutionIntervalMiliseconds)
                         orderby e.dateTime descending
                         select e).ToList();

            return query;
        }
    }
}