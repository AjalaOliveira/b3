using B3.Worker.Data;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Service.Process;
using B3.Worker.Shared.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace B3.Worker.Service.Tests
{

    [CollectionDefinition(nameof(OrderServiceSuccessCollection))]
    public class OrderServiceSuccessCollection : ICollectionFixture<OrderServiceSuccessTestsFixtures>
    {

    }

    public class OrderServiceSuccessTestsFixtures
    {
        public readonly OrderService _orderService;

        public OrderServiceSuccessTestsFixtures()
        {
            var mocker = new AutoMocker();
            var orderService = mocker.CreateInstance<OrderService>();

            mocker.GetMock<IOrderService>().Setup(x => x.ConnectWebSocketAsync(It.IsAny<string>(), It.IsAny<string>()));

            mocker.GetMock<IAppSettings>().Setup(x => x.GetBitstampUrl()).Returns(string.Empty);
            mocker.GetMock<IAppSettings>().Setup(x => x.GetBtcUsdMessage()).Returns(string.Empty);
            mocker.GetMock<IAppSettings>().Setup(x => x.GetEthUsdMessage()).Returns(string.Empty);

            _orderService = orderService;
        }
    }
}