using System.ComponentModel;

namespace B3.Api.Enums
{
    public enum OrderType
    {
        [Description("Bid")]
        Bid = 0,

        [Description("Ask")]
        Ask = 1
    }
}
