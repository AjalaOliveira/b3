using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Shared.Interfaces;
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
        private readonly IAppSettings _appSettings;

        public OrderRepository(
            ILogger<OrderRepository> logger,
            IOptionsMonitor<MongoDBSettings> mongoDBSettings,
            IAppSettings appSettings)
        {
            _logger = logger;
            _mongoDBSettings = mongoDBSettings;
            _appSettings = appSettings;
        }

        public async Task SaveOrder(IList<OrderEntity> orders)
        {
            MongoClient client = new(_mongoDBSettings.CurrentValue.ConnectionString);
            var playlistCollection = client.GetDatabase(_mongoDBSettings.CurrentValue.DatabaseName).GetCollection<OrderEntity>("orders");
            await playlistCollection.InsertManyAsync(orders);
        }

        public async Task<IList<OrderEntity>> GetMonitorValues()
        {
            MongoClient client = new(_mongoDBSettings.CurrentValue.ConnectionString);

            var playlistCollection = client.GetDatabase(_mongoDBSettings.CurrentValue.DatabaseName).GetCollection<OrderEntity>("orders");

            var query = playlistCollection.AsQueryable<OrderEntity>()
                                          .Where(o => o.timestamp >= (long)Convert.ToDouble(DateTimeOffset.UtcNow.AddMilliseconds(-_appSettings.GetExecutionIntervalMiliseconds()).ToUnixTimeSeconds().ToString()))
                                          .OrderByDescending(o => o.timestamp)
                                          .ToList();

            return await Task.FromResult(query);

        }
    }
}