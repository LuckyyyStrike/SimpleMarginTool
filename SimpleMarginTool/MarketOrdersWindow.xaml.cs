using Library;
using SimpleMarginTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleMarginTool
{
    /// <summary>
    /// Interaction logic for MarginToolWindow.xaml
    /// </summary>
    public partial class MarketOrdersWindow : Window
    {
        private MarginToolWindow _marginToolWindow;

        public MarketOrdersWindowViewModel ViewModel { get; set; }

        public MarketOrdersWindow()
        {
            InitializeComponent();
            ViewModel = new MarketOrdersWindowViewModel();
            this.DataContext = ViewModel;
            _marginToolWindow = new MarginToolWindow(ViewModel);
            _marginToolWindow.Show();
        }

        private void SellOrderViewSource_Filter(object sender, FilterEventArgs e)
        {
            // Wir lassen nur sell orders durch
            var order = (MarketOrder)e.Item;
            e.Accepted = !order.IsBid;
        }
        private void BuyOrderViewSource_Filter(object sender, FilterEventArgs e)
        {
            // Wir lassen nur buy orders durch
            var order = (MarketOrder)e.Item;
            e.Accepted = order.IsBid;
        }
    }
}
