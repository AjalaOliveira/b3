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
            while (!stoppingToken.IsCancellationRequested)
                _timer = new Timer(DoWorkAsync, null, 0, _appSettings.GetExecutionIntervalMiliseconds());
        }

        public async void DoWorkAsync(object? state)
        {
            try
            {
                if (!isRunning)
                {
                    isRunning = true;
                    //_logger.LogWarning("Serviço executado em: {time}", DateTimeOffset.Now);
                    await _monitorService.MonitorExecute();
                    isRunning = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante execução do serviço: {message}", ex.Message);
                throw new Exception();
            }
        }
    }
}