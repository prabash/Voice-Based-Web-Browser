﻿using System;
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
using UWIC.FinalProject.SpeechRecognitionEngine;
using UWIC.FinalProject.WebBrowser.Model;
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

        public string CommandText { get; set; }

        public string UrlText { get; set; }

        public Storyboard DownAnimation { get; set; }

        public BitmapImage Favicon { get; set; }

        public string WebPageTitle { get; set; }

        public Uri Url { get; set; }

        public bool WebBrowserVisible { get; set; }

        # endregion

        # region Main Page Events & Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BrowserContainer()
        {
            InitializeComponent();
            webBrowserMain.PreviewMouseMove += webBrowserMain_PreviewMouseMove; // Initialize Preview Mouse Move Event
        }

        /// <summary>
        /// This event will fire once the user control has been executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CloseEmulator();
            AcquireStoryboardAnimation();
            //timer.Elapsed += timer_Elapsed;
            //timer.Start();
        }

        /// <summary>
        /// This method will set the DownAnimation Storyboard on the ViewModel
        /// </summary>
        public void AcquireStoryboardAnimation()
        {
            var storyBoard = (Storyboard)FindResource("DownAnimation");
            DownAnimation = storyBoard;
            DownAnimation.Completed += DownAnimation_Completed;
        }

        /// <summary>
        /// This event will fire once the Down animation has been completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DownAnimation_Completed(object sender, EventArgs e)
        {
            WebBrowserVisible = true;
        }

        #region Timer
        //Timer timer = new Timer(10000);
        

        //void timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    //var svcClient = new SendKeysServiceClient();
        //    //svcClient.PostMessage("post message");
        //}
        #endregion

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

        private void RefreshBrowser()
        {
            webBrowserMain.Reload(false);
            SwitchToBusyState();
        }

        private void MoveBackward()
        {
            if (webBrowserMain.CanGoBack())
            {
                webBrowserMain.GoBack();
                SwitchToBusyState();
            }
        }

        private void MoveForward()
        {
            if (webBrowserMain.CanGoForward())
            {
                webBrowserMain.GoForward();
                SwitchToBusyState();
            }
        }

        private void StopBrowser()
        {
            webBrowserMain.Stop();
            SwitchToNormalState();
        }

        private void NavigateToUrl(string url = null)
        {
            Uri tempUri;
            if (url != null)
                TryParseUrl(url, out tempUri);
            else
                TryParseUrl(UrlText, out tempUri);
            if (!WebBrowserVisible)
                DownAnimation.Begin();

            webBrowserMain.Source = tempUri;
            SwitchToBusyState();
        }

        private static bool TryParseUrl(string uriString, out Uri uri)
        {
            return Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri);
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
                case FunctionalCommandType.Go:
                    {
                        NavigateToUrl();
                        break;
                    }
            }
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
                    pbWebPageLoad.Visibility = Visibility.Collapsed;
                    pbWebPageLoad.State = Elysium.Controls.ProgressState.Normal;

                    Url = webBrowserMain.Source;
                    SetWebPageTitleNFavicon();
                    SetHeaderAndIcon();
                    //if (this.PageLoadCompleted != null)
                    //    this.PageLoadCompleted(this, e); 
                    //SetCursorPos(36, 120);
                    //LeftMouseClick(36, 120);
                }
            }
        }
        
        /// <summary>
        /// Set the Web page title and Favicon to the Tab Item header
        /// </summary>
        public void SetWebPageTitleNFavicon()
        {
            Favicon = new BrowserContainerModel().getFavicon(new BrowserContainerModel().getImageSource(Url));
            WebPageTitle = new BrowserContainerModel().getWebPageTitle(Url.AbsoluteUri);
        }

        /// <summary>
        /// This method is used to set the Webpage Icon and the Title to the parent tab item
        /// </summary>
        private void SetHeaderAndIcon()
        {
            var parent = (UIElement)Parent;
            var _parent = (TabItem)parent;
            if (!String.IsNullOrEmpty(WebPageTitle))
            {
                _parent.Header = new TabItemHeader(Favicon, WebPageTitle);
            }
            else
            {
                _parent.Header = new TabItemHeader(Favicon, webBrowserMain.Source.Host);
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
        /// this method will switch the progress bar to an indeterminate mode
        /// </summary>
        private void SwitchToBusyState()
        {
            pbWebPageLoad.Visibility = Visibility.Visible;
            pbWebPageLoad.State = Elysium.Controls.ProgressState.Indeterminate;
        }

        /// <summary>
        /// This method will switch the progress bar to normal state and hide it
        /// </summary>
        private void SwitchToNormalState()
        {
            pbWebPageLoad.Visibility = Visibility.Collapsed;
            pbWebPageLoad.State = Elysium.Controls.ProgressState.Normal;
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

        public ICommand EmulCommand;
        public ICommand EmulatorCmd
        {
            get
            {
                return EmulCommand ??
                       (EmulCommand = new RelayCommand(ExecuteEmulator));
            }
        }

        public ICommand BmCommand;
        public ICommand BookmarkCommand
        {
            get { return BmCommand ?? (BmCommand = new RelayCommand(param => NavigateToUrl(param.ToString()))); }
        }

        #endregion

        #region Emulator

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                OpenEmulator();
            }
            else if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                CloseEmulator();
            }
        }

        private void OpenEmulator()
        {
            var sb = (Storyboard)FindResource("EmulatorOpen");
            sb.Begin();
        }

        private void CloseEmulator()
        {
            var sb = (Storyboard)FindResource("EmulatorClose");
            sb.Begin();
        }

        private void ExecuteEmulator()
        {
            var speechEngine = new SpeechEngine(SpeechRecognitionMode.Emulator);
            if (String.IsNullOrEmpty(CommandText)) return;
            speechEngine.StartEmulatorRecognition(CommandText);
            speechEngine.SpeechRecognized += SpeechEngine_SpeechRecognized;
        }

        void SpeechEngine_SpeechRecognized(object sender, EventArgs e)
        {
            var speechEngine = (SpeechEngine)sender;
            var resultDictionary = speechEngine.ResultDictionary;
            new CommandExecutionManager().ExecuteCommand(resultDictionary);
        }

        #endregion
    }
}
