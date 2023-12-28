using B3.Worker.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace B3.Worker.Data.Entities
{
    public class OrderEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public DateTime dateTime { get; set; }
        public decimal price { get; set; }
        public decimal amount { get; set; }
        public decimal orderValue { get; set; }
        public long timestamp { get; set; }
        public long microtimestamp { get; set; }
        public OrderType orderType { get; set; }
        public CurrencyPair channel { get; set; }
    }
}