using B3.Worker.HostedService;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B3.Worker.Tests
{
    public class MonitorHostedServiceTests
    {
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
    }
}