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
        /// <summary>
        /// Occurs, when <see cref="EnableAlwaysOnTop"/> changes.
        /// </summary>
        public event EventHandler EnableAlwaysOnTopChanged;

        public MarketOrdersWindowViewModel MarketOrdersWindowViewModel { get; set; }
        /// <summary>
        /// When true the best price for the selected order will be copied to the clipboard
        /// </summary>
        public bool EnableAutoCopy { get; set; } = true;
        /// <summary>
        /// When true this window will always be on top of other windows
        /// </summary>
        public bool EnableAlwaysOnTop { get; set; }

        /// <summary>
        /// The best sell order from the latest log entry
        /// </summary>
        public MarketOrder BestSellOrder => MarketOrdersWindowViewModel.LatestLogEntry?.MarketOrders.Where(x => !x.IsBid).OrderBy(x => x.Price).FirstOrDefault();
        /// <summary>
        /// The best buy order from the latest log entry
        /// </summary>
        public MarketOrder BestBuyOrder => MarketOrdersWindowViewModel.LatestLogEntry?.MarketOrders.Where(x => x.IsBid).OrderByDescending(x => x.Price).FirstOrDefault();
        /// <summary>
        /// The margin in isk between best buy and sell order
        /// </summary>
        public decimal? MarginInIsk
        {
            get
            {
                if (BestSellOrder == null || BestBuyOrder == null)
                    return null;
                return BestSellOrder.Price - BestBuyOrder.Price;
            }
        }
        /// <summary>
        /// The margin in percent between best buy and sell order
        /// </summary>
        public decimal? MarginInPercent
        {
            get
            {
                if (BestSellOrder == null || BestBuyOrder == null)
                    return null;
                return 1 - BestBuyOrder.Price / BestSellOrder.Price;
            }
        }
        /// <summary>
        /// Difference in Isk to be used in the calculation for the new best price
        /// </summary>
        public decimal? DeltaInIsk { get; set; } = new decimal(0.01);
        /// <summary>
        /// New best price determined by <see cref="IsBuyMode"/> selected, the best buy/sell order and the <see cref="DeltaInIsk"/>
        /// </summary>
        public decimal? NewBestPrice => IsBuyMode ? BestBuyOrder?.Price + DeltaInIsk : BestSellOrder?.Price + DeltaInIsk;
        /// <summary>
        /// Determines whether the <see cref="BestBuyOrder"/> or <see cref="BestSellOrder"/> will be used in the calculation for <see cref="NewBestPrice"/>
        /// </summary>
        public bool IsBuyMode { get; set; }

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
