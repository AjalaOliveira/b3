﻿using System.ComponentModel;

namespace B3.Worker.Shared.Enums
{
    public enum OrderType
    {
        [Description("Bid")]
        Bid = 0,
        [Description("Ask")]
        Ask = 1
    }
}