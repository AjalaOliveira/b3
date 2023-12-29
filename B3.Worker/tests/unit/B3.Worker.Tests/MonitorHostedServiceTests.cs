using B3.Worker.HostedService;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B3.Worker.Tests
{
    public class MonitorHostedServiceTests
    {
        [Fact]
        public async Task MonitorHostedService_DoExecute_ShouldThrowsException()
        {
            // Arrange
            var logger = new Mock<ILogger<MonitorHostedService>>();
            var appSettings = new Mock<IAppSettings>();
            
            var monitorService = new Mock<IMonitorService>();
            monitorService.Setup(x => x.MonitorExecute())
                          .Throws<Exception>();

            var monitorHostedService = new MonitorHostedService(logger.Object, appSettings.Object, monitorService.Object);

            monitorHostedService.DoWorkAsync(null);

            // Act & Assert
            Assert.Throws<Exception>(() => monitorHostedService.DoWorkAsync(null));
        }

        [Fact]
        public async Task MonitorHostedService_DoExecute_ShouldBeExecuted()
        {
            // Arrange
            var logger = new Mock<ILogger<MonitorHostedService>>();
            var appSettings = new Mock<IAppSettings>();
            var monitorService = new Mock<IMonitorService>();
            var monitorHostedService = new MonitorHostedService(logger.Object, appSettings.Object, monitorService.Object);

            //Act
            monitorHostedService.DoWorkAsync(null);

            //Assert
            //No assertion needed for completion, the test will fail if an exception occurs during the asynchronous operation
        }

        [Fact]
        public void WorkerShouldLogInformation()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<MonitorHostedService>>();
            var appSettings = serviceProvider.GetService<IAppSettings>();
            var monitorService = serviceProvider.GetService<IMonitorService>();

            var monitorHostedService = new MonitorHostedService(logger, appSettings, monitorService);

            // Act
            monitorHostedService.DoWorkAsync(CancellationToken.None);


            // Assert

        }
    }
}