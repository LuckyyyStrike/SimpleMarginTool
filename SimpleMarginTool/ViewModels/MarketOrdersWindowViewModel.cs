using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace SimpleMarginTool.ViewModels
{
    public class MarketOrdersWindowViewModel : ViewModelBase
    {
        private object _lock = new object();

        public event EventHandler MarketLogEntryAdded;

        public MarketLogEntry LatestLogEntry => MarketLogEntries.OrderByDescending(x => x.CreationTime).FirstOrDefault();

        private ObservableCollection<MarketLogEntry> _marketLogEntries;

        public ObservableCollection<MarketLogEntry> MarketLogEntries// { get; set; }
        {
            get { return _marketLogEntries; }
            set
            {
                _marketLogEntries = value;
                OnPropertyChanged();
                // Also Notify
                _marketLogEntries.CollectionChanged +=
                    (s, e) => OnPropertyChanged(nameof(MarketOrdersWindowViewModel.LatestLogEntry));
            }
        }



        //private MarketLogWatcher _watcher;

        public MarketLogWatcher Watcher { get; set; }
        //{
        //    get { return _watcher; }
        //    set { _watcher = value; OnPropertyChanged(); }
        //}

        public MarketOrdersWindowViewModel()
        {
            MarketLogEntries = new ObservableCollection<MarketLogEntry>();
            BindingOperations.EnableCollectionSynchronization(MarketLogEntries, _lock);
            Watcher = new MarketLogWatcher();
            Watcher.FileChanged += Watcher_FileChanged;
        }

        private void Watcher_FileChanged(object sender, MarketLogFileChangedEventArgs e)
        {
            Watcher.FileChanged -= Watcher_FileChanged;
            var fileInfo = new FileInfo(e.FileName);
            var filePath = fileInfo.FullName;
            var entry = new MarketLogEntry() { ItemName = filePath.Split('-')[1].Trim(), CreationTime = fileInfo.CreationTime, MarketOrders = new List<MarketOrder>() };

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
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

                        entry.MarketOrders.Add(order);
                    }
                }
            }
            MarketLogEntries.Add(entry);
            MarketLogEntryAdded?.Invoke(this, EventArgs.Empty);
            Watcher.FileChanged += Watcher_FileChanged;
        }
    }
}
