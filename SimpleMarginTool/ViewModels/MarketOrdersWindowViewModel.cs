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
using System.Windows.Input;
using System.Windows.Threading;

namespace SimpleMarginTool.ViewModels
{
    public class MarketOrdersWindowViewModel : ViewModelBase
    {
        private object _lock = new object();

        /// <summary>
        /// Occurs when a new <see cref="MarketLogEntry"/> is added to <see cref="MarketLogEntries"/>
        /// </summary>
        public event EventHandler MarketLogEntryAdded;

        /// <summary>
        /// The latest <see cref="MarketLogEntry"/> determined by <see cref="MarketLogEntry.CreationTime"/>
        /// </summary>
        public MarketLogEntry LatestLogEntry => MarketLogEntries.OrderByDescending(x => x.CreationTime).FirstOrDefault();

        private ObservableCollection<MarketLogEntry> _marketLogEntries;
        /// <summary>
        /// All market log entries
        /// </summary>
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
                        OnPropertyChanged(nameof(LatestLogEntry));
                        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                            MarketLogEntryAdded?.Invoke(this, EventArgs.Empty);
                    };
            }
        }

        public MarketLogWatcher Watcher { get; set; }

        public MarketOrdersWindowViewModel()
        {
            MarketLogEntries = new ObservableCollection<MarketLogEntry>();
            BindingOperations.EnableCollectionSynchronization(MarketLogEntries, _lock);
            AddInitialMarketLogEntry(MarketLogEntries);
            Watcher = new MarketLogWatcher();
            Watcher.FileChanged += Watcher_FileChanged;
        }

        /// <summary>
        /// Looks for the most recent market log file and adds it to <see cref="MarketLogEntries"/>
        /// </summary>
        /// <param name="marketLogEntries"></param>
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
