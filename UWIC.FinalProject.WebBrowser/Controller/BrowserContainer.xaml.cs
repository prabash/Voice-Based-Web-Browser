using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.TeamFoundation.MVVM;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.WebBrowser.ViewModel;
using UWIC.FinalProject.WebBrowser.svcSendKeys;

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
            webBrowserMain.PreviewKeyDown += webBrowserMain_PreviewKeyDown;
            AcquireStoryboardAnimation();
        }

        void webBrowserMain_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.LeftCtrl)
            //    EmulateTextInput();
        }

        public static void setViewModel(BrowserContainerViewModel vm)
        {
            ViewModel = vm;
        }

        Timer timer = new Timer(10000);
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.SetView(this);
            closeEmulator();

            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //var svcClient = new SendKeysServiceClient();
            //svcClient.PostMessage("post message");
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

        # region Web Browser Basic Navigation

        public void RefreshBrowser()
        {
            webBrowserMain.Reload(false);
            pbWebPageLoad.Visibility = Visibility.Visible;
            pbWebPageLoad.State = Elysium.Controls.ProgressState.Indeterminate;
        }

        public void MoveBackward()
        {
            if (webBrowserMain.CanGoBack())
                webBrowserMain.GoBack();
        }

        public void MoveForward()
        {
            if (webBrowserMain.CanGoForward())
                webBrowserMain.GoForward();
        }

        public void StopBrowser()
        {
            webBrowserMain.Stop();
            pbWebPageLoad.Visibility = Visibility.Hidden;
            pbWebPageLoad.State = Elysium.Controls.ProgressState.Normal;
        }

        #endregion

        # region Web Browser Events & Methods

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
            //ViewModel.URL = webBrowserMain.Source;
            txtURL.Text = webBrowserMain.Source.AbsoluteUri;
        }

        /// <summary>
        /// This method will set the DownAnimation Storyboard on the ViewModel
        /// </summary>
        public void AcquireStoryboardAnimation()
        {
            Storyboard sb = (Storyboard)FindResource("DownAnimation");
            ViewModel.DownAnimation = sb;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                openEmulator();
            }
            else if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                closeEmulator();
            }
        }

        private void openEmulator()
        {
            Storyboard sb = (Storyboard)FindResource("EmulatorOpen");
            sb.Begin();
        }

        private void closeEmulator()
        {
            Storyboard sb = (Storyboard)FindResource("EmulatorClose");
            sb.Begin();
        }

        #endregion

        #region Commands

        public ICommand FuncCommand;
        public ICommand FunctionCommand
        {
            get
            {
                return FuncCommand ??
                       (FuncCommand = new RelayCommand(param => ExecuteFunction(param.ToString())));
            }
        }

        private void ExecuteFunction(string command)
        {
            FunctionalCommandType commandType;
            if (!Enum.TryParse(command, out commandType)) return;
            switch (commandType)
            {
                case FunctionalCommandType.Backward:
                    {
                        MoveBackward();
                        break;
                    }
                case FunctionalCommandType.Forward:
                    {
                        MoveForward();
                        break;
                    }
                case FunctionalCommandType.Refresh:
                    {
                        RefreshBrowser();
                        break;
                    }
                case FunctionalCommandType.Stop:
                    {
                        StopBrowser();
                        break;
                    }
            }
        }

        #endregion
    }
}
