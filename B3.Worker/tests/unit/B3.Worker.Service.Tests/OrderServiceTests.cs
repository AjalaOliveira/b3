using B3.Worker.Data.Entities;
using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Interfaces.Services;
using B3.Worker.Service.Process;
using B3.Worker.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.WebSockets;
using Xunit;

namespace B3.Worker.Service.Tests
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task OrderService_BtcUsdExecute_ShouldBeExecuted()
        {
            // Arrange
            var logger = new Mock<ILogger<OrderService>>();
            var appSettings = new Mock<IAppSettings>();
            var websocketTools = new Mock<IWebsocketTools>();
            var orderRepository = new Mock<IOrderRepository>();

            websocketTools.Setup(x => x.GetConnectionAsync(It.IsAny<string>(), It.IsAny<ClientWebSocket>()));
            websocketTools.Setup(x => x.SendWebSocketMessage(It.IsAny<ClientWebSocket>(), It.IsAny<string>()));
            websocketTools.Setup(x => x.ReceiveWebSocketMessage(It.IsAny<ClientWebSocket>()));
            websocketTools.Setup(x => x.BuildDataSetToBeSaved(It.IsAny<MessageDeserialized>()));

            var orderService = new OrderService(logger.Object, appSettings.Object, orderRepository.Object, websocketTools.Object);

            // Act

            await orderService.BtcUsdExecute();

            // Assert
            websocketTools.Verify(_ => _.GetConnectionAsync(It.IsAny<string>(), It.IsAny<ClientWebSocket>()), Times.Once);
            websocketTools.Verify(_ => _.SendWebSocketMessage(It.IsAny<ClientWebSocket>(), It.IsAny<string>()), Times.Once);
            websocketTools.Verify(_ => _.ReceiveWebSocketMessage(It.IsAny<ClientWebSocket>()), Times.Never);
            websocketTools.Verify(_ => _.BuildDataSetToBeSaved(It.IsAny<MessageDeserialized>()), Times.Never);
        }

        [Fact]
        public async Task OrderService_EthUsdExecute_ShouldBeExecuted()
        {
            // Arrange
            var logger = new Mock<ILogger<OrderService>>();
            var appSettings = new Mock<IAppSettings>();
            var websocketTools = new Mock<IWebsocketTools>();
            var orderRepository = new Mock<IOrderRepository>();

            websocketTools.Setup(x => x.GetConnectionAsync(It.IsAny<string>(), It.IsAny<ClientWebSocket>()));
            websocketTools.Setup(x => x.SendWebSocketMessage(It.IsAny<ClientWebSocket>(), It.IsAny<string>()));
            websocketTools.Setup(x => x.ReceiveWebSocketMessage(It.IsAny<ClientWebSocket>()));
            websocketTools.Setup(x => x.BuildDataSetToBeSaved(It.IsAny<MessageDeserialized>()));

            var orderService = new OrderService(logger.Object, appSettings.Object, orderRepository.Object, websocketTools.Object);

            // Act

            await orderService.EthUsdExecute();

            // Assert
            websocketTools.Verify(_ => _.GetConnectionAsync(It.IsAny<string>(), It.IsAny<ClientWebSocket>()), Times.Once);
            websocketTools.Verify(_ => _.SendWebSocketMessage(It.IsAny<ClientWebSocket>(), It.IsAny<string>()), Times.Once);
            websocketTools.Verify(_ => _.ReceiveWebSocketMessage(It.IsAny<ClientWebSocket>()), Times.Never);
            websocketTools.Verify(_ => _.BuildDataSetToBeSaved(It.IsAny<MessageDeserialized>()), Times.Never);
        }
    }
}