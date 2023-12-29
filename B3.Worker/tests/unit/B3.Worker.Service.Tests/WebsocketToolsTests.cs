using B3.Worker.Data.Interfaces;
using B3.Worker.Service.Services;
using Moq;
using System.Net.WebSockets;
using Xunit;

namespace B3.Worker.Service.Tests
{
    public class WebsocketToolsTests
    {

        [Fact]
        public async Task WebsocketService_GetConnectionAsync_ShouldThrowsException()
        {
            // Arrange
            var websocketService = new WebsocketTools();

            // Act & Assert
            await Assert.ThrowsAsync<UriFormatException>(() => websocketService.GetConnectionAsync(string.Empty, new ClientWebSocket()));
        }

        [Fact]
        public async Task WebsocketService_GetConnectionAsync_ShouldBeExecuted()
        {
            // Arrange
            var websocketService = new WebsocketTools();

            // Act
            websocketService.GetConnectionAsync("wss://ws.bitstamp.net/", new ClientWebSocket());

            //Assert
            //No assertion needed for completion, the test will fail if an exception occurs during the asynchronous operation
        }

        [Fact]
        public async Task WebsocketService_SendWebSocketMessage_ShouldThrowsException()
        {
            // Arrange
            var websocketService = new WebsocketTools();
            var clientWebsocket = new ClientWebSocket();
            clientWebsocket.ConnectAsync(new Uri("wss://ws.bitstamp.net/"), CancellationToken.None);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => websocketService.SendWebSocketMessage(clientWebsocket, string.Empty));
        }

        [Fact]
        public async Task WebsocketService_ReceiveWebSocketMessage_ShouldThrowsException()
        {
            // Arrange
            var websocketService = new WebsocketTools();
            var clientWebsocket = new ClientWebSocket();
            clientWebsocket.ConnectAsync(new Uri("wss://ws.bitstamp.net/"), CancellationToken.None);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => websocketService.ReceiveWebSocketMessage(clientWebsocket));
        }
    }
}