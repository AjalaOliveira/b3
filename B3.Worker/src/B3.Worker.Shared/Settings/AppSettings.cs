namespace B3.Worker.Shared.Settings
{
    public class AppSettings
    {
        public string BitstampUrl { get; set; } = string.Empty;
        public string BtcUsdMessage { get; set; } = string.Empty;
        public string EthUsdMessage { get; set; } = string.Empty;
        public int ExecutionIntervalMiliseconds { get; set; }
    }
}