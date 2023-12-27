using B3.Worker.Service.Interfaces.Services;

namespace B3.Worker.HostedService
{
    internal class BtcUsdHostedService : BackgroundService
    {
        private readonly ILogger<BtcUsdHostedService> _logger;
        private readonly IOrderService _orderService;

        public BtcUsdHostedService(
            ILogger<BtcUsdHostedService> logger,
            IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("BTC/USD.ExecuteAsync - Serviço executado em: {time}", DateTimeOffset.Now);
                await _orderService.BtcUsdExecute();
            }
        }
    }
}