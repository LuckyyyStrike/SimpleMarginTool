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
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace SimpleMarginTool
{
    /// <summary>
    /// Interaction logic for MarginToolWindow.xaml
    /// </summary>
    public partial class MarginToolWindow : Window
    {
        // Parameter für "SetWindowPos(...)"
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-setwindowpos
        /// Sets position of the window with the given handle.
        /// </summary>
        /// <returns>Not Zero, when successful</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public MarketOrdersWindowViewModel MarketOrdersWindowViewModel { get; set; }
        public MarginToolWindowViewModel ViewModel { get; set; }

        public MarginToolWindow(MarketOrdersWindowViewModel viewModel)
        {
            InitializeComponent();
            MarketOrdersWindowViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            ViewModel = new MarginToolWindowViewModel(viewModel);
            DataContext = ViewModel;

            // Wenn dieses Fenster geschlossen wird, schließen wir die gesamte Anwendung
            Closing += (s, e) => Application.Current.MainWindow.Close();
            // Settings WindowStyle to "None" removes the draggability of the Windows. This adds it back in.
            MouseLeftButtonDown += (s, e) => 
            {
                base.OnMouseLeftButtonDown(e);
                // Begin dragging the window
                DragMove();
            };
            // Whenever this Window is being moved to the background, we will use the imported "SetWindowPos" function to force it to top-most
            Deactivated += (s, e) => { if (ViewModel.EnableAlwaysOnTop) SetWindowPos(new WindowInteropHelper(this).Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS); };
            // Whenever "Always on top" is set to false, we use "SetWindowPos" to 
            ViewModel.EnableAlwaysOnTopChanged += (s, e) => { if (!ViewModel.EnableAlwaysOnTop) SetWindowPos(new WindowInteropHelper(this).Handle, HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS); };
        }
    }
}
