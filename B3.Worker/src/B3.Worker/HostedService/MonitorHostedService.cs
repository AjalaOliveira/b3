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
        protected Timer? _timer = null;
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
                _timer = new Timer(DoWorkAsync, null, 0, _appSettings.CurrentValue.ExecutionIntervalMiliseconds);
            }

            await Task.FromResult(_timer);
        }

        private async void DoWorkAsync(object? state)
        {
            if (!isRunning)
            {
                isRunning = true;

                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");

                _logger.LogWarning("Serviço executado em: {time}", DateTimeOffset.Now);
                await _monitorService.MonitorExecute();
                isRunning = false;
            }
        }
    }
}