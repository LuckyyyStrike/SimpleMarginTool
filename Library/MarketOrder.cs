namespace Library
{
    /// <summary>
    /// Represents a single market order (buy or sell) inside a market log (the rows inside a market log file)
    /// </summary>
    public class MarketOrder
    {
        /// <summary>
        /// The price of the order in Isk
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Remaining quantity of items in the order
        /// </summary>
        public int VolumeRemaining { get; set; }
        /// <summary>
        /// Range of the order (irrelevant for sell orders)
        /// </summary>
        public int Range { get; set; }
        /// <summary>
        /// The id of the order? not sure tho
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// The id of the station the order sits at
        /// </summary>
        public string StationId { get; set; }
        /// <summary>
        /// The quantity already served to the order? not sure tho
        /// </summary>
        public int VolumeEntered { get; set; }
        /// <summary>
        /// The minimal quantity of items to serve to use this order.
        /// </summary>
        // TODO: Make this filterable to prevent scams
        public int MinVolume { get; set; }
        /// <summary>
        /// Distance in jumps from the station the log entry was exported from
        /// </summary>
        public int Jumps { get; set; }
        /// <summary>
        /// Whether this is a buy order or not
        /// </summary>
        public bool IsBid { get; set; }
    }

}