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

    /// <summary>
    /// Watches Eve's market log directory for new market log files
    /// </summary>
    public class MarketLogWatcher
    {
        /// <summary>
        /// Raised when a market log file changes
        /// </summary>
        public event MarketLogFileChangedEventHandler FileChanged;

        /// <summary>
        /// Path to Eve's market log directory
        /// </summary>
        public static readonly string MarketLogPath;

        private FileSystemWatcher FileSystemWatcher { get; set; }

        static MarketLogWatcher()
        {
            MarketLogPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EVE\logs\Marketlogs\";
        }

        public MarketLogWatcher()
        {
            FileSystemWatcher = new FileSystemWatcher(MarketLogPath);
            FileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess |
              NotifyFilters.LastWrite | NotifyFilters.FileName;
            FileSystemWatcher.Filter = "*.txt";
            FileSystemWatcher.Changed += (s,e) => { if(e.ChangeType == WatcherChangeTypes.Changed) OnFileChanged(e.FullPath); };
            FileSystemWatcher.Created += (s,e) => { if (e.ChangeType == WatcherChangeTypes.Changed) OnFileChanged(e.FullPath); };
            FileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnFileChanged(string fileName)
        {
            FileChanged?.Invoke(this, new MarketLogFileChangedEventArgs(fileName));
        }
    }

    public class MarketLogFileChangedEventArgs : EventArgs
    {
        /// <summary>
        /// File name of the market log file that changed
        /// </summary>
        public string FileName { get; set; }

        public MarketLogFileChangedEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }
}
