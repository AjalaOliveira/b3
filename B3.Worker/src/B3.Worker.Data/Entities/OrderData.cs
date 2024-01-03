namespace B3.Worker.Data.Entities
{
    public class OrderData
    {
        public string timestamp { get; set; }
        public string microtimestamp { get; set; }
        public List<List<string>> bids { get; set; }
        public List<List<string>> asks { get; set; }
    }
}