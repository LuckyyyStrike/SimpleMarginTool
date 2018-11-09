using Library;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SimpleMarginTool.ViewModels
{
    public class MarginToolWindowViewModel : ViewModelBase
    {
        public event EventHandler EnableAlwaysOnTopChanged;

        public MarketOrdersWindowViewModel MarketOrdersWindowViewModel { get; set; }
        public int TestInt { get; set; }
        public bool EnableAutoCopy { get; set; } = true;
        public bool EnableAlwaysOnTop { get; set; }

        public MarketOrder BestSellOrder => MarketOrdersWindowViewModel.LatestLogEntry?.MarketOrders.Where(x => !x.IsBid).OrderBy(x => x.Price).FirstOrDefault();
        public MarketOrder BestBuyOrder => MarketOrdersWindowViewModel.LatestLogEntry?.MarketOrders.Where(x => x.IsBid).OrderByDescending(x => x.Price).FirstOrDefault();
        public decimal? MarginInIsk
        {
            get
            {
                if (BestSellOrder == null || BestBuyOrder == null)
                    return null;
                return BestSellOrder.Price - BestBuyOrder.Price;
            }
        }
        public decimal? MarginInPercent
        {
            get
            {
                if (BestSellOrder == null || BestBuyOrder == null)
                    return null;
                return 1 - BestBuyOrder.Price / BestSellOrder.Price;
            }
        }
        public decimal? DeltaInIsk { get; set; } = new decimal(0.01);
        public decimal? NewBestPrice => IsBuyMode ? BestBuyOrder?.Price + DeltaInIsk : BestSellOrder?.Price + DeltaInIsk;
        public bool IsBuyMode { get; set; }
        //public bool IsSellMode { get; set; } = true;

        public MarginToolWindowViewModel(MarketOrdersWindowViewModel viewModel)
        {
            MarketOrdersWindowViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            MarketOrdersWindowViewModel.PropertyChanged += (s, e) =>
            {
                if (!e.PropertyName.Equals(nameof(MarketOrdersWindowViewModel.LatestLogEntry)))
                    return;

                OnPropertyChanged(nameof(BestSellOrder));
                OnPropertyChanged(nameof(BestBuyOrder));
                OnPropertyChanged(nameof(MarginInIsk));
                OnPropertyChanged(nameof(MarginInPercent));
                OnPropertyChanged(nameof(NewBestPrice));
                    CopyBestPriceToClipboard();
            };
            MarketOrdersWindowViewModel.MarketLogEntryAdded += (s, e) => { if (EnableAutoCopy) CopyBestPriceToClipboard(); };
        }

        private void OnEnableAlwaysOnTopChanged()
        {
            EnableAlwaysOnTopChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnIsBuyModeChanged()
        {
            CopyBestPriceToClipboard();
        }

        private void CopyBestPriceToClipboard()
        {
            if (NewBestPrice == null || NewBestPrice == 0 || !EnableAutoCopy)
                return;
            Application.Current.Dispatcher.Invoke(() => { Clipboard.SetText(NewBestPrice.ToString()); });            
        }
    }
}
