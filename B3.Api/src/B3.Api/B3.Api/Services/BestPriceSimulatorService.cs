using B3.Api.Config;
using B3.Api.Enums;
using B3.Api.Interfaces;
using B3.Api.Models;
using B3.Api.ViewModels;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace B3.Api.Services
{
    public class BestPriceSimulatorService : IBestPriceSimulatorService
    {
        private readonly IMongoCollection<BestPrice> bestPrices;
        private readonly IMongoCollection<OrderEntity> orders;

        public BestPriceSimulatorService(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            bestPrices = mongoDatabase.GetCollection<BestPrice>(databaseSettings.Value.BestPrices);
            orders = mongoDatabase.GetCollection<OrderEntity>(databaseSettings.Value.Orders);
        }

        public async Task<BestPrice> CreateAsync(HttpRequestBody HttpRequestBody)
        {
            var orders = new List<OrderEntity>();

            var lastRegister = await GetLastRegister(HttpRequestBody);
            var allRegisters = await GetAllRegistersFromTimestamp(lastRegister.timestamp);

            if (lastRegister != null && allRegisters != null)
            {
                if (HttpRequestBody.operatorType == OperatorType.Purchase)
                    orders = GetOrdersForPurchase(allRegisters).ToList();
                else
                    orders = GetOrdersForSale(allRegisters).ToList();
            }

            var bestPrice = CreateSimulatedOrder(orders, HttpRequestBody);
            await SaveSimulatedOrder(bestPrice);

            return bestPrice;
        }

        public async Task<List<BestPrice>> GetAllAsync()
            => await bestPrices.Find(x => true)
                               .ToListAsync();

        #region Private Methods

        private async Task<OrderEntity> GetLastRegister(HttpRequestBody HttpRequestBody)
        {
            return await orders.Find(x => x.channel == HttpRequestBody.currencyPair)
                               .Sort(new BsonDocument("$natural", -1))
                               .FirstOrDefaultAsync();
        }

        private async Task<IList<OrderEntity>> GetAllRegistersFromTimestamp(long timestamp)
        {
            return await orders.Find(x => x.timestamp == timestamp)
                               .ToListAsync();
        }

        private static IList<OrderEntity> GetOrdersForPurchase(IList<OrderEntity> orders)
        {
            return orders.Where(x => x.orderType == OrderType.Ask)
                         .OrderBy(x => x.price)
                         .ToList();
        }

        private static IList<OrderEntity> GetOrdersForSale(IList<OrderEntity> orders)
        {
            return orders.Where(x => x.orderType == OrderType.Bid)
                         .OrderByDescending(x => x.price)
                         .ToList();
        }

        public async Task SaveSimulatedOrder(BestPrice bestPrice)
            => await bestPrices.InsertOneAsync(bestPrice);

        private static BestPrice CreateSimulatedOrder(IList<OrderEntity> orders, HttpRequestBody httpRequestBody)
        {
            var bestPrice = new BestPrice();
            var control = 0M;
            bestPrice.orderType = httpRequestBody.operatorType;

            foreach (var order in orders)
            {
                if (control < httpRequestBody.amount)
                {
                    control += order.amount;
                    bestPrice.orderList.Add(order);
                    bestPrice.orderValue += order.orderValue;
                }
                else
                {
                    break;
                }
            }

            bestPrice.amount = httpRequestBody.amount;

            return bestPrice;
        }

        #endregion
    }
}