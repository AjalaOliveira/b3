namespace B3.Api.Models
{
    public class Order
    {
        public string _id { get; set; }
        public decimal amount { get; set; }
        public decimal orderValue { get; set; }
        public decimal price { get; set; }
        public string asset { get; set; }
    }
}