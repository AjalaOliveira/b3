namespace B3.Worker.Shared.Options
{
    public class AppSettingsOptions
    {
        public int ExecutionIntervalMiliseconds { get; set; }

        public string BitstampUrl { get; set; } = string.Empty;

        public string BtcUsdMessage { get; set; } = string.Empty;

        public string EthUsdMessage { get; set; } = string.Empty;
    }
}