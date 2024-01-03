namespace B3.Api.Config
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string BestPrices { get; set; } = string.Empty;
        public string Orders { get; set; } = string.Empty;
    }
}