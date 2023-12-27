using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Settings;
using Microsoft.Extensions.Options;

namespace B3.Worker.HostedService
{
    internal class MonitorHostedService : BackgroundService
    {
        private readonly ILogger<MonitorHostedService> _logger;
        private readonly IMonitorService _monitorService;
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        private bool isRunning = false;

        public MonitorHostedService(
            ILogger<MonitorHostedService> logger,
            IOptionsMonitor<AppSettings> appSettings,
            IMonitorService monitorService)
        {
            _logger = logger;
            _appSettings = appSettings;
            _monitorService = monitorService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!isRunning)
                {
                    isRunning = true;
                    _logger.LogWarning("Serviço executado em: {time}", DateTimeOffset.Now);
                    await _monitorService.MonitorExecute();
                    Thread.Sleep(_appSettings.CurrentValue.ExecutionIntervalMiliseconds);
                    isRunning = false;
                }

            }
        }
    }
}