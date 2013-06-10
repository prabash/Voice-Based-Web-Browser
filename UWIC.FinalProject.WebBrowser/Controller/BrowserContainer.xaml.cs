using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UWIC.FinalProject.WebBrowser.ViewModel;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for BrowserContainer.xaml
    /// </summary>
    public partial class BrowserContainer : UserControl
    {
        # region Events

        public event EventHandler PageLoadCompleted;

        # endregion

        # region Public Variables

        public static BrowserContainerViewModel ViewModel { get; set; }

        # endregion

        # region Main Page Events & Methods

        public BrowserContainer()
        {
            InitializeComponent();
            webBrowserMain.PreviewMouseMove += webBrowserMain_PreviewMouseMove; // Initialize Preview Mouse Move Event
        }

        public static void setViewModel(BrowserContainerViewModel vm)
        {
            ViewModel = vm;
        }

        #endregion

        # region Mouse Move Events & Methods
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        /// <summary>
        /// This event will fire when the mouse moves over the web browser container
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void webBrowserMain_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point point = GetMousePosition();
            xyPosition.Content = "X Position = " + point.X.ToString() + "; Y Position = " + point.Y.ToString() + ";";
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private void SetPosition(int a, int b)
        {
            SetCursorPos(a, b);
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        private void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }
        # endregion
        
        /// <summary>
        /// This event will fire once a parituclar page has been loaded successfully
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowserMain_LoadingFrameComplete(object sender, Awesomium.Core.FrameEventArgs e)
        {
            if (e.IsMainFrame)
            {
                if (webBrowserMain.IsDocumentReady)
                {
                    pbWebPageLoad.State = Elysium.Controls.ProgressState.Normal;
                    pbWebPageLoad.Visibility = System.Windows.Visibility.Collapsed;
                    ViewModel.SetWebPageTitleNFavicon();
                    SetHeaderAndIcon();
                    //if (this.PageLoadCompleted != null)
                    //    this.PageLoadCompleted(this, e); 
                    //SetCursorPos(36, 120);
                    //LeftMouseClick(36, 120);
                }
            }
        }

        /// <summary>
        /// This method is used to set the Webpage Icon and the Title to the parent tab item
        /// </summary>
        private void SetHeaderAndIcon()
        {
            var parent = (UIElement)this.Parent;
            var _parent = (TabItem)parent;
            if (!String.IsNullOrEmpty(ViewModel.WebPageTitle))
            {
                _parent.Header = new TabItemHeader(ViewModel.Favicon, ViewModel.WebPageTitle);
            }
            else
            {
                _parent.Header = new TabItemHeader(ViewModel.Favicon, webBrowserMain.Source.Host);
            }
        }

        /// <summary>
        /// This event will fire once the Address of the web browser has been changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowserMain_AddressChanged(object sender, Awesomium.Core.UrlEventArgs e)
        {
            ViewModel.URL = webBrowserMain.Source;
        }

        # region Bookmark Buttons

        /*
        private void btnGoogle_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.google.com");
        }

        private void btnGooglePlus_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("plus.google.com");
        }

        private void btnGmail_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("mail.google.com");
        }

        private void btnYoutube_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.youtube.com");
        }

        private void btnFacebook_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.facebook.com");
        }

        private void btnTwitter_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.twitter.com");
        }

        private void btnLinkedIn_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.linkedin.com");
        }

        private void btnImdb_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.imdb.com");
        }

        private void btnMicrosoft_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.microsoft.com");
        }

        private void btnMSN_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.msn.com");
        }

        private void btnYahoo_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.yahoo.com");
        }

        private void btnDropbox_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.dropbox.com");
        }

        private void btnEbay_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.ebay.com");
        }

        private void btnAmazon_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.amazon.com");
        }

        private void btnApple_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.apple.com");
        }

        private void btnWikipedia_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage("www.wikipedia.com");
        }
        */
        #endregion
    }
}
