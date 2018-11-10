using Library;
using SimpleMarginTool.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace SimpleMarginTool.ViewModels
{
    public class MarketOrdersWindowViewModel : ViewModelBase
    {
        private object _lock = new object();

        public event EventHandler MarketLogEntryAdded;

        public MarketLogEntry LatestLogEntry => MarketLogEntries.OrderByDescending(x => x.CreationTime).FirstOrDefault();

        private ObservableCollection<MarketLogEntry> _marketLogEntries;

        public ObservableCollection<MarketLogEntry> MarketLogEntries
        {
            get { return _marketLogEntries; }
            set
            {
                _marketLogEntries = value;
                OnPropertyChanged();
                // Also Notify
                _marketLogEntries.CollectionChanged +=
                    (s, e) =>
                    {
                        OnPropertyChanged(nameof(MarketOrdersWindowViewModel.LatestLogEntry));
                        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                            MarketLogEntryAdded?.Invoke(this, EventArgs.Empty);
                    };
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
            AddInitialMarketLogEntry(MarketLogEntries);
            Watcher = new MarketLogWatcher();
            Watcher.FileChanged += Watcher_FileChanged;
        }

        private void AddInitialMarketLogEntry(ObservableCollection<MarketLogEntry> marketLogEntries)
        {
            var logDir = new DirectoryInfo(MarketLogWatcher.MarketLogPath);
            var files = logDir.GetFiles("*.txt");
            var lastLogFile = files.OrderByDescending(x => x.LastWriteTime).FirstOrDefault();
            if (lastLogFile == null)
                return;
            MarketLogEntries.Add(new MarketLogEntry(lastLogFile));
        }

        private void Watcher_FileChanged(object sender, MarketLogFileChangedEventArgs e)
        {
            Watcher.FileChanged -= Watcher_FileChanged;
            var fileInfo = new FileInfo(e.FileName);
            var filePath = fileInfo.FullName;
            var entry = new MarketLogEntry(fileInfo);
            MarketLogEntries.Add(entry);
            Watcher.FileChanged += Watcher_FileChanged;
        }
    }
}
