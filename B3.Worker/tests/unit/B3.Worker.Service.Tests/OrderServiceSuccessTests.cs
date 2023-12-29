using B3.Worker.Service.Process;
using System;
using Xunit;

namespace B3.Worker.Service.Tests
{
    [Collection(nameof(OrderServiceSuccessCollection))]
    public class OrderServiceSuccessTests
    {
        private readonly OrderService _orderService;

        public OrderServiceSuccessTests(OrderServiceSuccessTestsFixtures orderServiceSuccessTestsFixtures)
        {
            _orderService = orderServiceSuccessTestsFixtures._orderService;
        }

        [Fact]
        public async Task OrderService_BtcUsdExecute_ShouldBeExecuted()
        {
            // Arrange & Act
            await _orderService.BtcUsdExecute();

            // Assert
            //No assertion needed for completion, the test will fail if an exception occurs during the asynchronous operation
        }

        [Fact]
        public async Task OrderService_EthUsdExecute_ShouldBeExecuted()
        {
            // Arrange & Act
            await _orderService.EthUsdExecute();

            // Assert
            //No assertion needed for completion, the test will fail if an exception occurs during the asynchronous operation
        }
    }
}