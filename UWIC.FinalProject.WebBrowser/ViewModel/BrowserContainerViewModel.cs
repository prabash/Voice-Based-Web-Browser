using JetBrains.Annotations;
using Microsoft.TeamFoundation.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.WebBrowser.Controller;
using UWIC.FinalProject.WebBrowser.Model;

namespace UWIC.FinalProject.WebBrowser.ViewModel
{
    public class BrowserContainerViewModel : MainViewModel
    {
        BrowserContainerModel browserContainerModel = new BrowserContainerModel();

        public bool WebBrowserVisible { get; set; }
        public static BrowserContainer _browserContainer;

        public BrowserContainerViewModel()
        {
            WebBrowserVisible = false;
            BrowserContainer.setViewModel(this);
        }

        void DownAnimation_Completed(object sender, EventArgs e)
        {
            WebBrowserVisible = true;
        }

        public void SetView(BrowserContainer bc)
        {
            _browserContainer = bc;
        }

        public Storyboard _downAnimation;
        public Storyboard DownAnimation
        {
            get
            {
                return _downAnimation;
            }
            set
            {
                _downAnimation = value;
                DownAnimation.Completed += DownAnimation_Completed;
            }
        }

        private Uri _url;
        public Uri URL
        {
            get { return _url; }
            set
            {
                _url = value;
                URLText = _url.ToString();
                OnPropertyChanged("URL");
            }
        }

        private string _urlText;
        public string URLText
        {
            get { return _urlText; }
            set
            {
                _urlText = value;
                OnPropertyChanged("URLText");
            }
        }

        private BitmapImage _faviCon;
        public BitmapImage Favicon
        {
            get { return _faviCon; }
            set
            {
                _faviCon = value;
                OnPropertyChanged("Favicon");
            }
        }

        private string _webPageTitle;
        public string WebPageTitle
        {
            get { return _webPageTitle; }
            set
            {
                _webPageTitle = value;
                OnPropertyChanged("WebPageTitle");
            }
        }

        private Elysium.Controls.ProgressState _progressBarState;
        public Elysium.Controls.ProgressState ProgressBarState
        {
            get { return _progressBarState; }
            set
            {
                _progressBarState = value;
                OnPropertyChanged("ProgressBarState");
            }
        }

        private bool _progressBarVisibility;
        public bool ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                OnPropertyChanged("ProgressBarVisibility");
            }
        }

        private bool TryParseURL(string uriString, out Uri uri)
        {
            return Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri);
        }

        public void NavigateToURL(string _URL = null)
        {
            Uri _tempUri;
            if (_URL != null)
                TryParseURL(_URL, out _tempUri);
            else
                TryParseURL(URLText, out _tempUri);

            if (!WebBrowserVisible)
            {
                DownAnimation.Begin();
            }
            URL = _tempUri;
            ProgressBarVisibility = true;
            ProgressBarState = Elysium.Controls.ProgressState.Indeterminate;
        }

        public void SetWebPageTitleNFavicon()
        {
            ProgressBarVisibility = false;
            ProgressBarState = Elysium.Controls.ProgressState.Normal;
            Favicon = browserContainerModel.getFavicon(browserContainerModel.getImageSource(URL));
            WebPageTitle = browserContainerModel.getWebPageTitle(URL.AbsoluteUri);
        }

        public void RefreshBrowserWindow()
        {
            _browserContainer.RefreshBrowser();
        }

        public void MoveBackward()
        {
            _browserContainer.MoveBackward();
        }

        public void MoveForward()
        {
            _browserContainer.MoveForward();
        }

        public void StopBrowser()
        {
            _browserContainer.StopBrowser();
        }

        public ICommand BmCommand;
        public ICommand BookmarkCommand
        {
            get { return BmCommand ?? (BmCommand = new RelayCommand(param => this.NavigateToURL(param.ToString()))); }
        }

        public ICommand NavCommand;
        public ICommand NavigateCommand
        {
            get {
                return NavCommand ??
                       (NavCommand = new RelayCommand(param => ExecuteFunction(param.ToString())));
            }
        }

        private void ExecuteFunction(string command)
        {
            FunctionalCommandType commandType;
            if (!Enum.TryParse(command, out commandType)) return;
            switch (commandType)
            {
                case FunctionalCommandType.Go:
                    {
                        NavigateToURL();
                        break;
                    }
            }
        }
    }
}
