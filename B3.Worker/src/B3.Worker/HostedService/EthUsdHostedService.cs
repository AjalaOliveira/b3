using B3.Worker.Service.Interfaces.Services;

namespace B3.Worker.HostedService
{
    internal class EthUsdHostedService : BackgroundService
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
            {
                _logger.LogInformation("ETH/USD.ExecuteAsync - Serviço executado em: {time}", DateTimeOffset.Now);
                await _orderService.EthUsdExecute();
            }
        }
    }
}