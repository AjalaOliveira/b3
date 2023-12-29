using B3.Worker.Service.Process;
using Xunit;

namespace B3.Worker.Service.Tests
{
    [Collection(nameof(OrderServiceFailCollection))]
    public class OrderServiceFailTests
    {
        private readonly OrderService _orderService;

        public OrderServiceFailTests(OrderServiceFailTestsFixtures orderServiceFailTestsFixtures)
        {
            _orderService = orderServiceFailTestsFixtures._orderService;
        }

        [Fact]
        public async Task OrderService_BtcUsdExecute_ShouldThrowsException()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _orderService.BtcUsdExecute());
        }

        [Fact]
        public async Task OrderService_EthUsdExecute_ShouldThrowsException()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _orderService.EthUsdExecute());
        }
    }
}