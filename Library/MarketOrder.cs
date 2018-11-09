namespace Library
{
    public class MarketOrder
    {
        public decimal Price { get; set; }

        public int VolumeRemaining { get; set; }
        public int Range { get; set; }
        public string OrderId { get; set; }
        public string StationId { get; set; }
        public int VolumeEntered { get; set; }
        public int MinVolume { get; set; }
        public int Jumps { get; set; }
        public bool IsBid { get; set; }
    }

}