using System;
using System.Collections.Generic;

namespace Library
{
    public class MarketLogEntry
    {
        public string ItemName { get; set; }
        public DateTime CreationTime { get; set; }
        public List<MarketOrder> MarketOrders { get; set; }
    }
}