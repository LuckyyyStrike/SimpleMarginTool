using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Library
{
    public class MarketLogEntry
    {
        public string ItemName { get; set; }
        public DateTime CreationTime { get; set; }
        public List<MarketOrder> MarketOrders { get; set; }

        public MarketLogEntry(string filePath)
        {
            CreateMarketLogEntry(new FileInfo(filePath));
        }

        public MarketLogEntry(FileInfo fileInfo)
        {
            CreateMarketLogEntry(fileInfo);
        }

        private void CreateMarketLogEntry(FileInfo fileInfo)
        {
            var fileNameSplit = fileInfo.FullName.Split('-');
            if (fileNameSplit.Length != 3)
                return;
            ItemName = fileNameSplit[1].Trim();
            //CreationTime = DateTime.Parse(fileNameSplit[2].Replace(".txt", string.Empty));
            CreationTime = fileInfo.LastWriteTime;
            MarketOrders = new List<MarketOrder>();

            using (var fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs))
                {

                    int counter = 0;
                    while (!sr.EndOfStream)
                    {
                        counter++;
                        var order = new MarketOrder();
                        var row = sr.ReadLine();

                        // The first row contains the column headers, so we skip it 
                        if (counter == 1)
                            continue;

                        var columns = row.Split(',');
                        order.Price = decimal.Parse(columns[0], CultureInfo.InvariantCulture);
                        order.VolumeRemaining = (int)double.Parse(columns[1], CultureInfo.InvariantCulture);
                        order.Range = int.Parse(columns[3]);
                        order.OrderId = columns[4];
                        order.VolumeEntered = int.Parse(columns[5]);
                        order.MinVolume = int.Parse(columns[6]);
                        order.IsBid = bool.Parse(columns[7]);
                        order.StationId = columns[10];
                        order.Jumps = int.Parse(columns[13]);

                        this.MarketOrders.Add(order);
                    }
                }
            }
        }
    }
}