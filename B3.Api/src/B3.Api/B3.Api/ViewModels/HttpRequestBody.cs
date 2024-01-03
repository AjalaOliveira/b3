using B3.Api.Enums;

namespace B3.Api.ViewModels
{
    public class HttpRequestBody
    {
        public OperatorType operatorType { get; set; }
        public CurrencyPair currencyPair { get; set; }
        public decimal amount { get; set; }
    }
}
