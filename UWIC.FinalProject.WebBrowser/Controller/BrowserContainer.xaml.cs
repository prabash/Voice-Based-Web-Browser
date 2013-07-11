using System;
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
using System.Speech.Synthesis;

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

        private Mode _commandMode;

        public Mode CommandMode
        {
            get { return _commandMode; }
            set
            {
                _commandMode = value;
                switch (_commandMode)
                {
                    case Mode.CommandMode:
                        {
                            var bc = new BrushConverter();
                            BottomBar.Background = (Brush)bc.ConvertFrom("#FF171717");
                            LabelCurrentMode.Content = "Command Mode";
                            break;
                        }
                    case Mode.DictationMode:
                        {
                            BottomBar.Background = Brushes.Crimson;
                            LabelCurrentMode.Content = "Dictation Mode";
                            break;
                        }
                    case Mode.GeneralSpellMode:
                        {
                            BottomBar.Background = Brushes.OrangeRed;
                            LabelCurrentMode.Content = "General Spelling Mode";
                            break;
                        }
                    case Mode.PasswordSpellMode:
                        {
                            BottomBar.Background = Brushes.SlateBlue;
                            LabelCurrentMode.Content = "Username/Password Mode";
                            break;
                        }
                    case Mode.WebsiteSpellMode:
                        {
                            BottomBar.Background = Brushes.ForestGreen;
                            LabelCurrentMode.Content = "Website Spelling Mode";
                            break;
                        }
                }
            }
        }

        public System.Speech.Recognition.SpeechRecognitionEngine SpeechRecognizerEngine { get; set; }

        public MessageBoxWindow MessageWindow { get; set; }

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

            CloseMessageBox();

            #region Background Image
            if (!UriParser.IsKnownScheme("pack"))
                UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);

            var dict = new ResourceDictionary();
            var uri = new Uri("/UWIC.FinalProject.WebBrowser;component/Resources/BackgroundImageResourceDictionary.xaml", UriKind.Relative);
            dict.Source = uri;
            Application.Current.Resources.MergedDictionaries.Add(dict);
            var backImageSource = (ImageSource)Application.Current.Resources["BackImage"];
            BImage.ImageSource = backImageSource;
            # endregion
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
            Favicon = new BrowserContainerModel().GetFavicon(new BrowserContainerModel().GetImageSource(Url));
            WebPageTitle = new BrowserContainerModel().GetWebPageTitle(Url.AbsoluteUri);
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
            speechEngine.InitializeEmulator(CommandMode);
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

        # region MessageBox

        private void CloseMessageBox()
        {
            try
            {
                var sb = (Storyboard)FindResource("MessageBoxClose");
                sb.Begin();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        private void OpenMessageBox()
        {
            try
            {
                var sb = (Storyboard)FindResource("MessageBoxOpen");
                sb.Begin();

                SpeakMessage();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        private void ShowMessageBoxDetails(string message, string messageTitle, Visibility yesButtonVisible, Visibility noButtonVisible, MessageBoxIcon icon)
        {
            SetImage(icon);
            LblTitle.Content = messageTitle;
            TxtMessage.Text = message;
            BtnYes.Visibility = yesButtonVisible;
            BtnNo.Visibility = noButtonVisible;

            OpenMessageBox();
        }

        private void ShowMessageBoxDetails(string message, string messageTitle, Visibility okButtonVisible, MessageBoxIcon icon)
        {
            SetImage(icon);
            LblTitle.Content = messageTitle;
            TxtMessage.Text = message;
            BtnOk.Visibility = okButtonVisible;

            OpenMessageBox();
        }

        private void SetImage(MessageBoxIcon icon)
        {
            var logo = new BitmapImage();
            logo.BeginInit();
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    logo.UriSource = new Uri("pack://application:,,,/UWIC.FinalProject.WebBrowser;component/Images/error-white.png");
                    break;
                case MessageBoxIcon.Information:
                    logo.UriSource = new Uri("pack://application:,,,/UWIC.FinalProject.WebBrowser;component/Images/info-white.png");
                    break;
            }
            logo.EndInit();
            ImgIcon.Source = logo;
        }

        private void SpeakMessage()
        {
            var voice = new SpeechSynthesizer();
            try
            {
                voice.Volume = 100;
                voice.Rate = 0;
                voice.SpeakAsync(TxtMessage.Text);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
            }
        }

        #endregion

        #region Voice Recognizer

        private void InitializeSpeechRecognizer()
        {
            var result = "";
            SpeechRecognizerEngine = new SpeechEngine().CreateSpeechEngine("en-GB", out result);
            SpeechRecognizerEngine.LoadGrammar(new DictationGrammar());
            SpeechRecognizerEngine.LoadGrammar(new SpeechEngine().GetSpellingGrammar());
            SpeechRecognizerEngine.LoadGrammar(new SpeechEngine().GetWebSiteNamesGrammar());
            SpeechRecognizerEngine.AudioLevelUpdated += SpeechRecognizerEngine_AudioLevelUpdated;
            SpeechRecognizerEngine.SpeechRecognized += SpeechRecognizerEngine_SpeechRecognized;

            // use the system's default microphone
            SpeechRecognizerEngine.SetInputToDefaultAudioDevice();

            // start listening
            SpeechRecognizerEngine.RecognizeAsync(RecognizeMode.Multiple);
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
                case Mode.PasswordSpellMode:
                    ExecutePasswordSpelling(resultDictionary);
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
                    case CommandType.startpasswordspelling:
                        {
                            CommandMode = Mode.PasswordSpellMode;
                            break;
                        }
                    case CommandType.startgeneralspelling:
                        {
                            CommandMode = Mode.GeneralSpellMode;
                            break;
                        }
                    case CommandType.yes:
                        {
                            ExecuteYesNoCommand(CommandType.yes);
                            break;
                        }
                    case CommandType.no:
                        {
                            ExecuteYesNoCommand(CommandType.no);
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

        private void ExecuteYesNoCommand(CommandType type)
        {
            switch (type)
            {
                case CommandType.yes:
                    if (MessageWindow != null) MessageWindow.Close();
                    break;
                case CommandType.no:
                    if (MessageWindow != null) MessageWindow.Close();
                    break;
            }
        }

        private void ExecuteDictationCommand(Dictionary<CommandType, object> dictationCommand)
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

        private void ExecuteSpellingCommand(bool webSiteSpelling, Dictionary<CommandType, object> dictationCommand)
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

        private void ExecutePasswordSpelling(Dictionary<CommandType, object> dictationCommand)
        {
            var credential = "";
            var caseState = CaseState.Default;
            try
            {
                foreach (var segments in dictationCommand.Select(pair => pair.Value.ToString()).Select(command => command.Split(' ')))
                {
                    foreach (var segment in segments)
                    {
                        switch(caseState)
                        {
                            case CaseState.Default:
                                {
                                    if (String.Equals(segment, "capital", StringComparison.OrdinalIgnoreCase))
                                        caseState = CaseState.UpperCase;
                                    else if (String.Equals(segment, "simple", StringComparison.OrdinalIgnoreCase))
                                        caseState = CaseState.LowerCase;
                                    else if (String.Equals(segment, "one", StringComparison.OrdinalIgnoreCase))
                                        credential += 1;
                                    else if (String.Equals(segment, "two", StringComparison.OrdinalIgnoreCase))
                                        credential += 2;
                                    else if (String.Equals(segment, "three", StringComparison.OrdinalIgnoreCase))
                                        credential += 3;
                                    else if (String.Equals(segment, "four", StringComparison.OrdinalIgnoreCase))
                                        credential += 4;
                                    else if (String.Equals(segment, "five", StringComparison.OrdinalIgnoreCase))
                                        credential += 5;
                                    else if (String.Equals(segment, "six", StringComparison.OrdinalIgnoreCase))
                                        credential += 6;
                                    else if (String.Equals(segment, "seven", StringComparison.OrdinalIgnoreCase))
                                        credential += 7;
                                    else if (String.Equals(segment, "eight", StringComparison.OrdinalIgnoreCase))
                                        credential += 8;
                                    else if (String.Equals(segment, "nine", StringComparison.OrdinalIgnoreCase))
                                        credential += 9;
                                    else if (String.Equals(segment, "zero", StringComparison.OrdinalIgnoreCase))
                                        credential += 0;
                                    else if (String.Equals(segment, "underscore", StringComparison.OrdinalIgnoreCase))
                                        credential += "_";
                                    else if (String.Equals(segment, "hash", StringComparison.OrdinalIgnoreCase))
                                        credential += "#";
                                    else if (String.Equals(segment, "dot", StringComparison.OrdinalIgnoreCase))
                                        credential += ".";
                                    else if (String.Equals(segment, "at", StringComparison.OrdinalIgnoreCase))
                                        credential += "@";
                                    else
                                        credential += segment.ToLower();
                                    break;
                                }
                            case CaseState.UpperCase:
                                {
                                    credential += segment.ToUpper();
                                    caseState = CaseState.Default;
                                    break;
                                }
                            case CaseState.LowerCase:
                                {
                                    credential += segment.ToLower();
                                    caseState = CaseState.Default;
                                    break;
                                }
                        }
                    }
                }
                InvokePostMessageService(credential);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        private void AppendWebsiteToTextFile(string website)
        {
            VbwFileManager.AppendToTextFile(VbwFileManager.FilePath() + "fnc_brwsr_websites" + VbwFileManager.FileExtension(), new List<string> { website.ToLower() });
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

        private void BtnTest_OnClick(object sender, RoutedEventArgs e)
        {
            ShowMessageBoxDetails("This is just a sample error message", "Sample Error Message", Visibility.Visible,
                                  MessageBoxIcon.Error);
        }

        private void BtnTest1_OnClick(object sender, RoutedEventArgs e)
        {
            CloseMessageBox();
        }

        private void ChangeBackground()
        {
            //BackgroundImage.ImageSource
        }
    }
}