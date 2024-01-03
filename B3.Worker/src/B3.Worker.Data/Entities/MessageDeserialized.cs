using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace B3.Worker.Data.Entities
{
    public class MessageDeserialized
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public DateTime dateTime { get; set; }
        public OrderData data { get; set; }
        public string channel { get; set; }
    }
}