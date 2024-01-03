using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Service.Process;
using B3.Worker.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B3.Worker.Service.Tests
{

    [CollectionDefinition(nameof(OrderServiceFailCollection))]
    public class OrderServiceFailCollection : ICollectionFixture<OrderServiceFailTestsFixtures>
    {

    }

    public class OrderServiceFailTestsFixtures
    {
        public readonly OrderService _orderService;

        public OrderServiceFailTestsFixtures()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var appSettings = new Mock<IAppSettings>();
            var websocketTools = new Mock<IWebsocketTools>();

            appSettings.Setup(x => x.GetBitstampUrl()).Throws<Exception>();
            appSettings.Setup(x => x.GetBtcUsdMessage()).Throws<Exception>();
            appSettings.Setup(x => x.GetEthUsdMessage()).Throws<Exception>();

            var orderRepository = new Mock<IOrderRepository>();

            _orderService = new OrderService(logger.Object, appSettings.Object, orderRepository.Object, websocketTools.Object);
        }
    }
}