﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

        private static TabItemViewModel TabItemViewModel { get; set; }

        public static Mode CommandMode { get; set; }

        public System.Speech.Recognition.SpeechRecognitionEngine speechRecognizerEngine { get; set; }

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
            CommandMode = Mode.CommandMode;
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
            Thread.Sleep(TimeSpan.FromSeconds(3));
            SetCursorPos(a, b);
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02; /* left button down */
        public const int MOUSEEVENTF_LEFTUP = 0x04; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */

        //This simulates a left mouse click
        private void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        private void LeftMouseClick()
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));
            //Call the imported function with the cursor's current position
            var currentPosition = GetMousePosition();
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Convert.ToInt32(currentPosition.X), Convert.ToInt32(currentPosition.Y), 0, 0);
        }

        private void RightMouseClick()
        {
            //Thread.Sleep(TimeSpan.FromSeconds(3));
            //var currentPosition = GetMousePosition();
            //mouse_event(MOUSEEVENTF_RIGHTDOWN, Convert.ToInt32(currentPosition.X), Convert.ToInt32(currentPosition.Y), 0, 0);
        }

        private void DoubleClick()
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));
            var currentPosition = GetMousePosition();
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Convert.ToInt32(currentPosition.X), Convert.ToInt32(currentPosition.Y), 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Convert.ToInt32(currentPosition.X), Convert.ToInt32(currentPosition.Y), 0, 0);
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
            var speechEngine = new SpeechEngine();
            speechEngine.InitializeEmulator();
            if (String.IsNullOrEmpty(CommandText)) return;
            speechEngine.StartEmulatorRecognition(CommandText);
            speechEngine.SpeechProcessed += SpeechEngine_SpeechProcessed;
        }

        void SpeechEngine_SpeechProcessed(object sender, EventArgs e)
        {
            var speechEngine = (SpeechEngine)sender;
            var resultDictionary = speechEngine.ResultDictionary;
            StartCommandExecution(resultDictionary);
        }

        #endregion

        #region Voice Recognizer

        private void InitializeSpeechRecognizer()
        {
            var result = "";
            speechRecognizerEngine = new SpeechEngine().CreateSpeechEngine("en-GB", out result);
            speechRecognizerEngine.LoadGrammar(new DictationGrammar());
            speechRecognizerEngine.LoadGrammar(new SpeechEngine().GetSpellingGrammar());
            speechRecognizerEngine.LoadGrammar(new SpeechEngine().GetWebSiteNamesGrammar());
            speechRecognizerEngine.AudioLevelUpdated += SpeechRecognizerEngine_AudioLevelUpdated;
            speechRecognizerEngine.SpeechRecognized += SpeechRecognizerEngine_SpeechRecognized;

            // use the system's default microphone
            speechRecognizerEngine.SetInputToDefaultAudioDevice();

            // start listening
            speechRecognizerEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void SpeechRecognizerEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.7)
            {
                var resultDictionary = new SpeechEngine().InitializeSpeechProcessing(e.Result.Text);
                StartCommandExecution(resultDictionary);
            }
            else
            {
                MessageBox.Show("Your words cannot be recognized properly!");
            }
        }

        private void SpeechRecognizerEngine_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            PbAudioLevel.Value = e.AudioLevel;
        }

        #endregion

        # region Tab Item

        public static void SetTabItemViewModel(TabItemViewModel tabItemViewModel)
        {
            TabItemViewModel = tabItemViewModel;
        }

        private void AddNewTab()
        {
            TabItemViewModel.AddTabItem();
        }

        private void RemoveTabByIndex(int index)
        {
            TabItemViewModel.RemoveTabItemByIndex(index);
        }

        private void GoToTabByIndex(int index)
        {
            TabItemViewModel.SetFocusOnTabItem(index);
        }

        private void RemoveCurrentTab()
        {
            TabItemViewModel.RemoveCurrentTabItem();
        }

        # endregion

        # region Command Execution

        private void StartCommandExecution(Dictionary<CommandType, object> resultDictionary)
        {
            switch (CommandMode)
            {
                case Mode.CommandMode:
                    ExecuteCommand(resultDictionary);
                    break;
                case Mode.DictationMode:
                    ExecuteDictationCommand(resultDictionary);
                    CommandMode = Mode.CommandMode;
                    break;
                case Mode.WebsiteSpellMode:
                    ExecuteSpellingCommand(true, resultDictionary);
                    CommandMode = Mode.CommandMode;
                    break;
                case Mode.GeneralSpellMode:
                    ExecuteSpellingCommand(false, resultDictionary);
                    CommandMode = Mode.CommandMode;
                    break;
            }
        }

        public void ExecuteCommand(Dictionary<CommandType, object> identifiedCommand)
        {
            try
            {
                var command = identifiedCommand.First();
                switch (command.Key)
                {
                    case CommandType.go:
                        {
                            var identifiedWebSite = command.Value.ToString();
                            if (!identifiedWebSite.Contains(".com"))
                                identifiedWebSite += ".com";
                            var websiteName = "http://www." + identifiedWebSite;
                            NavigateToUrl(websiteName);
                            break;
                        }
                    case CommandType.back:
                        {
                            MoveBackward();
                            break;
                        }
                    case CommandType.forth:
                        {
                            MoveForward();
                            break;
                        }
                    case CommandType.refresh:
                        {
                            RefreshBrowser();
                            break;
                        }
                    case CommandType.stop:
                        {
                            StopBrowser();
                            break;
                        }
                    case CommandType.alter:
                        {
                            InvokePostMessageService("%");
                            break;
                        }
                    case CommandType.backspace:
                        {
                            InvokePostMessageService("{BS}");
                            break;
                        }
                    case CommandType.capslock:
                        {
                            InvokePostMessageService("{CAPSLOCK}");
                            break;
                        }
                    case CommandType.control:
                        {
                            InvokePostMessageService("^");
                            break;
                        }
                    case CommandType.downarrow:
                        {
                            InvokePostMessageService("{DOWN}");
                            break;
                        }
                    case CommandType.enter:
                        {
                            InvokePostMessageService("{ENTER}");
                            break;
                        }
                    case CommandType.f5:
                        {
                            InvokePostMessageService("{F5}");
                            break;
                        }
                    case CommandType.leftarrow:
                        {
                            InvokePostMessageService("{LEFT}");
                            break;
                        }
                    case CommandType.rightarrow:
                        {
                            InvokePostMessageService("{RIGHT}");
                            break;
                        }
                    case CommandType.space:
                        {
                            InvokePostMessageService(" ");
                            break;
                        }
                    case CommandType.tab:
                        {
                            InvokePostMessageService("{TAB}");
                            break;
                        }
                    case CommandType.uparrow:
                        {
                            InvokePostMessageService("{UP}");
                            break;
                        }
                    case CommandType.scrollup:
                        {
                            InvokePostMessageService("{PGUP}");
                            break;
                        }
                    case CommandType.scrolldown:
                        {
                            InvokePostMessageService("{PGDN}");
                            break;
                        }
                    case CommandType.scrollleft:
                        {
                            InvokePostMessageService("{LEFT}");
                            break;
                        }
                    case CommandType.scrollright:
                        {
                            InvokePostMessageService("{RIGHT}");
                            break;
                        }
                    case CommandType.move:
                        {
                            var coordinates = command.Value.ToString();
                            var seperatedCoordinates = coordinates.Split(',').ToList();
                            SetPosition(Convert.ToInt32(seperatedCoordinates.First()), Convert.ToInt32(seperatedCoordinates.Last()));
                            break;
                        }
                    case CommandType.click:
                        {
                            LeftMouseClick();
                            break;
                        }
                    case CommandType.rightclick:
                        {
                            RightMouseClick();
                            break;
                        }
                    case CommandType.doubleclick:
                        {
                            DoubleClick();
                            break;
                        }
                    case CommandType.opennewtab:
                        {
                            AddNewTab();
                            break;
                        }
                    case CommandType.gototab:
                        {
                            var index = command.Value.ToString();
                            GoToTabByIndex(Convert.ToInt32(index));
                            break;
                        }
                    case CommandType.closetab:
                        {
                            RemoveCurrentTab();
                            break;
                        }
                    case CommandType.startdictationmode:
                        {
                            CommandMode = Mode.DictationMode;
                            break;
                        }
                    case CommandType.startwebsitespelling:
                        {
                            CommandMode = Mode.WebsiteSpellMode;
                            break;
                        }
                    case CommandType.startgeneralspelling:
                        {
                            CommandMode = Mode.GeneralSpellMode;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        public void ExecuteDictationCommand(Dictionary<CommandType, object> dictationCommand)
        {
            try
            {
                foreach (var pair in dictationCommand)
                {
                    InvokePostMessageService(pair.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        public void ExecuteSpellingCommand(bool webSiteSpelling, Dictionary<CommandType, object> dictationCommand)
        {
            try
            {
                var firstPair = dictationCommand.First();
                var word = AcquireSpelledWord(firstPair.Value.ToString());
                if (webSiteSpelling)
                    AppendWebsiteToTextFile(word);
                else
                    InvokePostMessageService(word);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        public void AppendWebsiteToTextFile(string website)
        {
            TextFileManager.AppendToTextFile("..//..//data//fnc_brwsr_websites" + ".txt", new List<string> { website.ToLower() });
        }

        private static void InvokePostMessageService(string message)
        {
            SendKeysServiceClient svcClient = null;
            try
            {
                svcClient = new SendKeysServiceClient();
                svcClient.PostMessage(message, 3);
            }
            catch (Exception ex)
            {
                if (svcClient != null) svcClient.Close();
                Log.ErrorLog(ex);
                throw;
            }
            finally
            {
                if (svcClient != null) svcClient.Abort();
            }
        }

        private static string AcquireSpelledWord(string spelledWord)
        {
            var letters = spelledWord.Split(' ');
            return letters.Aggregate("", (current, letter) => current + letter);
        }

        # endregion
    }
}