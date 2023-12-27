using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Settings;
using Microsoft.Extensions.Options;

namespace B3.Worker.HostedService
{
    internal class MonitorHostedService : BackgroundService
    {
        private readonly ILogger<MonitorHostedService> _logger;
        private readonly IOrderService _orderService;
        private readonly IOptionsMonitor<AppSettings> _appSettings;

        public MonitorHostedService(
            ILogger<MonitorHostedService> logger,
            IOptionsMonitor<AppSettings> appSettings,
        IOrderService orderService)
        {
            _logger = logger;
            _appSettings = appSettings;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Thread.Sleep(_appSettings.CurrentValue.ExecutionIntervalMiliseconds);

                _logger.LogInformation("MonitorHostedService.ExecuteAsync - Serviço executado em: {time}", DateTimeOffset.Now);
                await _orderService.MonitorExecute();
            }
        }
    }
}