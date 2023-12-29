using B3.Worker.Shared.Enums;
using Xunit;

namespace B3.Worker.Shared.Tests.Enum
{
    public class CurrencyPairTests
    {
        [Fact]
        public void EnumExtensions_GetDescription_ShouldReturnCorrectBtcUsdEnumDescription()
        {
            // Arrange && Act
            var enumDescription = CurrencyPair.BtcUsd.GetDescription();

            // Assert
            Assert.Equal("order_book_btcusd", enumDescription);
        }

        [Fact]
        public void EnumExtensions_GetDescription_ShouldReturnCorrectEthUsdEnumDescription()
        {
            // Arrange && Act
            var enumDescription = CurrencyPair.EthUsd.GetDescription();

            // Assert
            Assert.Equal("order_book_ethusd", enumDescription);
        }

        [Fact]
        public void EnumExtensions_GetDescription_ShouldReturnCorrectBtcUsdEnumType()
        {
            // Arrange && Act
            var enumDescription = CurrencyPair.BtcUsd.GetType();

            // Assert
            Assert.Equal("CurrencyPair", enumDescription.Name);
        }

        [Fact]
        public void EnumExtensions_GetDescription_ShouldReturnCorrectEthUsdEnumType()
        {
            // Arrange && Act
            var enumDescription = CurrencyPair.EthUsd.GetType();

            // Assert
            Assert.Equal("CurrencyPair", enumDescription.Name);
        }
    }
}
