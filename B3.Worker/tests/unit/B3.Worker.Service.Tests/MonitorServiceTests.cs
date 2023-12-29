using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Service.Process;
using B3.Worker.Service.Services;
using B3.Worker.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace B3.Worker.Service.Tests
{
    public class MonitorServiceTests
    {
        [Fact]
        public async Task MonitorService_GetConnectionAsync_ShouldBeExecuted()
        {
            // Arrange
            var logger = new Mock<ILogger<MonitorService>>();
            var appSettings = new Mock<IAppSettings>();
            var websocketTools = new Mock<IWebsocketTools>();
            var orderRepository = new Mock<IOrderRepository>();

            websocketTools.Setup(x => x.GetConnectionAsync(It.IsAny<string>(), It.IsAny<ClientWebSocket>()));
            websocketTools.Setup(x => x.SendWebSocketMessage(It.IsAny<ClientWebSocket>(), It.IsAny<string>()));
            websocketTools.Setup(x => x.ReceiveWebSocketMessage(It.IsAny<ClientWebSocket>()));
            websocketTools.Setup(x => x.BuildDataSetToBeSaved(It.IsAny<MessageDeserialized>()));

            var monitorService = new MonitorService(logger.Object, appSettings.Object, orderRepository.Object, websocketTools.Object);

            // Act

            await monitorService.MonitorExecute();

            // Assert
            websocketTools.Verify(_ => _.GetConnectionAsync(It.IsAny<string>(), It.IsAny<ClientWebSocket>()), Times.Exactly(2));
            websocketTools.Verify(_ => _.SendWebSocketMessage(It.IsAny<ClientWebSocket>(), It.IsAny<string>()), Times.Exactly(2));
            websocketTools.Verify(_ => _.ReceiveWebSocketMessage(It.IsAny<ClientWebSocket>()), Times.Never);
            websocketTools.Verify(_ => _.BuildDataSetToBeSaved(It.IsAny<MessageDeserialized>()), Times.Never);
        }
    }
}
