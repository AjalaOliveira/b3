using B3.Api.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace B3.Api.Models
{
    public class BestPrice
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; } = string.Empty;
        public List<OrderEntity> orderList { get; set; } = new List<OrderEntity>();
        public OperatorType orderType { get; set; }
        public decimal amount { get; set; }
        public decimal orderValue { get; set; }
        public string  asset { get; set; }
    }
}