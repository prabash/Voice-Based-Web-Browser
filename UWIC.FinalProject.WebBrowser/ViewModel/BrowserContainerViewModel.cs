using Microsoft.TeamFoundation.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UWIC.FinalProject.WebBrowser.Controller;
using UWIC.FinalProject.WebBrowser.Model;

namespace UWIC.FinalProject.WebBrowser.ViewModel
{
    public class BrowserContainerViewModel : MainViewModel
    {
        BrowserContainerModel browserContainerModel = new BrowserContainerModel();
        public BrowserContainerViewModel()
        {
            BrowserContainer.setViewModel(this);
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

        private Visibility _progressBarVisibility;
        public Visibility ProgressBarVisibility
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
            return Uri.TryCreate(uriString, UriKind.Relative, out uri);
        }

        private ICommand _goCommand;
        public ICommand GoCommand
        {
            get
            {
                if (_goCommand == null)
                {
                    _goCommand = new RelayCommand(NavigateToURL);
                }
                return _goCommand;
            }
        }

        private void NavigateToURL()
        {
            if (!String.IsNullOrEmpty(URLText))
            {
                Uri _tempUri;
                if (TryParseURL(URLText, out _tempUri))
                {
                    URL = _tempUri;
                    ProgressBarVisibility = Visibility.Visible;
                    ProgressBarState = Elysium.Controls.ProgressState.Indeterminate;
                }
            }
        }

        public void SetWebPageTitleNFavicon()
        {
            ProgressBarVisibility = Visibility.Collapsed;
            ProgressBarState = Elysium.Controls.ProgressState.Normal;
            Favicon = browserContainerModel.getFavicon(browserContainerModel.getImageSource(URL));
            WebPageTitle = browserContainerModel.getWebPageTitle(URL.AbsoluteUri.ToString());
        }
    }
}
