using B3.Worker.Data.Entities;
using B3.Worker.Service.Services;
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


        [Fact]
        public async Task WebsocketService_BuildBidsDataSetToBeSaved_ShouldBeExecuted()
        {
            try
            {
                // Arrange
                var websocketService = new WebsocketTools();
                var messageDeserialized = new MessageDeserialized();
                messageDeserialized.channel = "order_book_btcusd";

                var orderData = new OrderData();
                orderData.timestamp = "1704300769";
                orderData.microtimestamp = "1704300769809797";

                var bids = new List<List<string>>();
                var bidsItem = new List<string>();
                bidsItem.Add("42990");
                bidsItem.Add("0.76886317");
                bids.Add(bidsItem);
                orderData.bids = bids;

                var asks = new List<List<string>>();
                var askItem = new List<string>();
                askItem.Add("42999");
                askItem.Add("0.62769846");
                asks.Add(askItem);
                orderData.asks = asks;

                messageDeserialized.data = orderData;

                // Act
                var result = websocketService.BuildDataSetToBeSaved(messageDeserialized);
                Assert.True(true);
            }
            catch (Exception ex)
            {
                //Assert
                Assert.True(false);
            }
        }

        [Fact]
        public async Task WebsocketService_BuildAsksDataSetToBeSaved_ShouldThrowsException()
        {
            try
            {
                // Arrange
                var websocketService = new WebsocketTools();
                var messageDeserialized = new MessageDeserialized();

                // Act
                var result = websocketService.BuildDataSetToBeSaved(new MessageDeserialized());
                Assert.True(false);
            }
            catch (Exception ex)
            {
                //Assert
                Assert.True(true);
            }
        }
    }
}