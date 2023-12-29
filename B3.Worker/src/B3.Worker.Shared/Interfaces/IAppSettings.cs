namespace B3.Worker.Shared.Interfaces
{
    public interface IAppSettings
    {
        public string GetBitstampUrl();

        public string GetBtcUsdMessage();

        public string GetEthUsdMessage();

        public int GetExecutionIntervalMiliseconds();
    }
}