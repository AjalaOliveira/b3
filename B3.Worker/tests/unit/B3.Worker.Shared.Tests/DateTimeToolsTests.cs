using B3.Worker.Shared.Utils;

namespace B3.Worker.Shared.Tests
{
    public class DateTimeToolsTests
    {
        [Theory]
        [InlineData(1703723098, "28/12/2023 00:24:58")]
        [InlineData(1703724175, "28/12/2023 00:42:55")]
        [InlineData(1703722884, "28/12/2023 00:21:24")]
        [InlineData(1703723616, "28/12/2023 00:33:36")]
        [InlineData(1703722742, "28/12/2023 00:19:02")]
        public void DateTimeTools_SetDateTimeFromTimestamp_ShouldReturnCorrectDateTime(long timestamp, string dateTime)
        {
            // Arrange
            var dateTimeTools = new DateTimeTools();

            // Act
            var resultado = dateTimeTools.SetDateTimeFromTimestamp(timestamp);

            // Assert
            Assert.Equal(dateTime, resultado.ToString());
        }
    }
}