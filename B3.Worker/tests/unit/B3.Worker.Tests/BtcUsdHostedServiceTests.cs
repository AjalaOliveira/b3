using B3.Worker.HostedService;
using B3.Worker.Service.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B3.Worker.Tests
{
    public class BtcUsdHostedServiceTests
    {
        [Fact]
        public async Task BtcUsdHostedService_DoWorkAsync_ShouldThrowsException()
        {
            // Arrange
            var logger = new Mock<ILogger<BtcUsdHostedService>>();

            var orderService = new Mock<IOrderService>();
            orderService.Setup(x => x.BtcUsdExecute())
                        .Throws<Exception>();

            var btcUsdHostedService = new BtcUsdHostedService(logger.Object, orderService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => btcUsdHostedService.DoWorkAsync());
        }

        [Fact]
        public async Task BtcUsdHostedService_DoWorkAsync_ShouldBeExecuted()
        {
            // Arrange
            var logger = new Mock<ILogger<BtcUsdHostedService>>();
            var orderService = new Mock<IOrderService>();
            var btcUsdHostedService = new BtcUsdHostedService(logger.Object, orderService.Object);

            //Act
            await btcUsdHostedService.DoWorkAsync();

            //Assert
            //No assertion needed for completion, the test will fail if an exception occurs during the asynchronous operation
        }
    }
}