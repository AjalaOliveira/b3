using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace B3.Worker.Data.Entities
{
    public class OrderEntity
    {
        [BsonRepresentation(BsonType.ObjectId)] 
        public string _id { get; set; }
        public DateTime dateTime { get; set; }
        public Data data { get; set; }
        public string channel { get; set; }
    }
}