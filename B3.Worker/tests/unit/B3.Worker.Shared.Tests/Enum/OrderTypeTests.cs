using B3.Worker.Shared.Enums;
using Xunit;

namespace B3.Worker.Shared.Tests.Enum
{
    public class OrderTypeTests
    {
        [Fact]
        public void EnumExtensions_GetDescription_ShouldReturnCorrectBidEnumDescription()
        {
            // Arrange && Act
            var enumDescription = OrderType.Bid.GetDescription();

            // Assert
            Assert.Equal("Bid", enumDescription);
        }

        [Fact]
        public void EnumExtensions_GetDescription_ShouldReturnCorrectAskEnumDescription()
        {
            // Arrange && Act
            var enumDescription = OrderType.Ask.GetDescription();

            // Assert
            Assert.Equal("Ask", enumDescription);
        }

        [Fact]
        public void EnumExtensions_GetDescription_ShouldReturnCorrectBidEnumType()
        {
            // Arrange && Act
            var enumDescription = OrderType.Bid.GetType();

            // Assert
            Assert.Equal("OrderType", enumDescription.Name);
        }

        [Fact]
        public void EnumExtensions_GetDescription_ShouldReturnCorrectAskEnumType()
        {
            // Arrange && Act
            var enumDescription = OrderType.Ask.GetType();

            // Assert
            Assert.Equal("OrderType", enumDescription.Name);
        }
    }
}
