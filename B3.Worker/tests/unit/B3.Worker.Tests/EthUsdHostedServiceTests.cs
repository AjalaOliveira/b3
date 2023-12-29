using B3.Worker.HostedService;
using B3.Worker.Service.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B3.Worker.Tests
{
    public class EthUsdHostedServiceTests
    {
        [Fact]
        public async Task EthUsdHostedService_DoExecute_ShouldThrowsException()
        {
            // Arrange
            var logger = new Mock<ILogger<EthUsdHostedService>>();

            var orderService = new Mock<IOrderService>();
            orderService.Setup(x => x.EthUsdExecute())
                        .Throws<Exception>();

            var ethUsdHostedService = new EthUsdHostedService(logger.Object, orderService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => ethUsdHostedService.DoWorkAsync());
        }

        [Fact]
        public async Task EthUsdHostedService_DoExecute_ShouldBeExecuted()
        {
            // Arrange
            var logger = new Mock<ILogger<EthUsdHostedService>>();
            var orderService = new Mock<IOrderService>();
            var ethUsdHostedService = new EthUsdHostedService(logger.Object, orderService.Object);

            //Act
            await ethUsdHostedService.DoWorkAsync();

            //Assert
            //No assertion needed for completion, the test will fail if an exception occurs during the asynchronous operation
        }
    }
}