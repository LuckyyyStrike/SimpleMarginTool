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

        public static readonly string MarketLogPath;

        public FileSystemWatcher FileSystemWatcher { get; set; }

        static MarketLogWatcher()
        {
            MarketLogPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EVE\logs\Marketlogs\";
            //logPath = @"D:\";
        }

        public MarketLogWatcher()
        {
            FileSystemWatcher = new FileSystemWatcher(MarketLogPath);

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
