using B3.Worker.Shared.Interfaces;
using B3.Worker.Shared.Options;
using Microsoft.Extensions.Options;

namespace B3.Worker.Shared.Settings
{
    public class AppSettings : IAppSettings
    {
        private readonly IOptionsMonitor<AppSettingsOptions> _appSettingsOptions;

        public AppSettings(
            IOptionsMonitor<AppSettingsOptions> appSettingsOptions)
        {
            _appSettingsOptions = appSettingsOptions;
        }

        public string GetBitstampUrl()
        {
            return _appSettingsOptions.CurrentValue.BitstampUrl;
        }

        public string GetBtcUsdMessage()
        {
            return _appSettingsOptions.CurrentValue.BtcUsdMessage;
        }

        public string GetEthUsdMessage()
        {
            return _appSettingsOptions.CurrentValue.EthUsdMessage;
        }

        public int GetExecutionIntervalMiliseconds()
        {
            return _appSettingsOptions.CurrentValue.ExecutionIntervalMiliseconds;
        }
    }
}