using System.ComponentModel;

namespace B3.Worker.Shared.Enums
{
    public enum CurrencyPair
    {
        [Description("order_book_btcusd")]
        BtcUsd = 0,
        [Description("order_book_ethusd")]
        EthUsd = 1
    }
}