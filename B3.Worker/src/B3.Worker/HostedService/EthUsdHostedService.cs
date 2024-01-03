using B3.Worker.Service.Interfaces.Services;

namespace B3.Worker.HostedService
{
    public class EthUsdHostedService : BackgroundService
    {
        private readonly ILogger<EthUsdHostedService> _logger;
        private readonly IOrderService _orderService;

        public EthUsdHostedService(
            ILogger<EthUsdHostedService> logger,
            IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
                await DoWorkAsync();
        }

        public async Task DoWorkAsync()
        {
            _logger.LogInformation("Serviço executado em: {time}", DateTimeOffset.Now);
            try
            {
                await _orderService.EthUsdExecute();
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante execução do serviço: {message}", ex.Message);
            }
        }
    }
}