using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public delegate void MarketLogFileChangedEventHandler(object sender, MarketLogFileChangedEventArgs args);
    public class MarketLogWatcher
    {
        public event MarketLogFileChangedEventHandler FileChanged;

        private static readonly string logPath;

        public FileSystemWatcher FileSystemWatcher { get; set; }

        static MarketLogWatcher()
        {
            logPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EVE\logs\Marketlogs\";
            //logPath = @"D:\";
        }

        public MarketLogWatcher()
        {
            FileSystemWatcher = new FileSystemWatcher(logPath);

            //FileSystemWatcher.Changed += FileSystemWatcherChanged;
            //FileSystemWatcher.Created += FileSystemWatcherChanged;
            //FileSystemWatcher.Deleted += FileSystemWatcherChanged;
            //FileSystemWatcher.Renamed += FileSystemWatcherChanged;

            FileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess |
              NotifyFilters.LastWrite | NotifyFilters.FileName;
            FileSystemWatcher.Filter = "*.txt";


            //FileSystemWatcher.Created += WatcherChanged;
            FileSystemWatcher.Changed += (s,e) => { if(e.ChangeType == WatcherChangeTypes.Changed) OnFileChanged(e.FullPath); };
            FileSystemWatcher.Created += (s,e) => { if (e.ChangeType == WatcherChangeTypes.Changed) OnFileChanged(e.FullPath); };
            //fileSystemWatcher.Changed += (s, e) => { };
            FileSystemWatcher.EnableRaisingEvents = true;
        }

        private void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            var fileInfo = new FileInfo(e.FullPath);
            var filePath = fileInfo.FullName;
            var entry = new MarketLogEntry() { ItemName = filePath.Split('-')[1].Trim(), CreationTime = fileInfo.CreationTime, MarketOrders = new List<MarketOrder>() };

            using(var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using(var sr = new StreamReader(fs))
                {
                    var order = new MarketOrder();

                    int i = 0;
                    while (!sr.EndOfStream)
                    {
                        var row = sr.ReadLine();

                        // The first row contains the column headers, so we skip it 
                        if (i == 0)
                            continue;

                        var columns = row.Split(',');
                        order.Price = decimal.Parse(columns[0]);
                        //order.Quantity = int.Parse(columns[1]);
                        order.Range = int.Parse(columns[3]);
                        order.IsBid = bool.Parse(columns[6]);
                        i++;
                    }
                    entry.MarketOrders.Add(order);
                }
            }

             //MarketLogEntries.Add(entry);
        }

        private void OnFileChanged(string fileName)
        {
            FileChanged?.Invoke(this, new MarketLogFileChangedEventArgs(fileName));
        }
    }

    public class MarketLogFileChangedEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public MarketLogFileChangedEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }
}
