using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Interfaces;

namespace B3.Worker.HostedService
{
    public class MonitorHostedService : BackgroundService
    {
        private readonly ILogger<MonitorHostedService> _logger;
        private readonly IMonitorService _monitorService;
        private readonly IAppSettings _appSettings;
        protected Timer? _timer = null;
        private bool isRunning = false;

        public MonitorHostedService(
            ILogger<MonitorHostedService> logger,
            IAppSettings appSettings,
            IMonitorService monitorService)
        {
            _logger = logger;
            _appSettings = appSettings;
            _monitorService = monitorService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogWarning("Serviço executado em: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
                await DoWorkAsync();
        }

        public async Task DoWorkAsync()
        {
            try
            {
                if (!isRunning)
                {
                    isRunning = true;

                    Thread.Sleep(_appSettings.GetExecutionIntervalMiliseconds());

                    await _monitorService.MonitorExecute();
                    isRunning = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante execução do serviço: {message}", ex.Message);
                throw;
            }
        }
    }
}