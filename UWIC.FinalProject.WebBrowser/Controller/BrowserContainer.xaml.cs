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

        public BitmapImage Favicon { get; set; }
        public string PageTitle { get; set; }

        # endregion

        public BrowserContainer()
        {
            InitializeComponent();
            webBrowserMain.PreviewMouseMove += webBrowserMain_PreviewMouseMove; // Initialize Preview Mouse Move Event
        }

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
        
        Uri address;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            navigateToPage(txtURL.Text);
        }

        private void navigateToPage(string URL)
        {
            if (Uri.TryCreate(URL, UriKind.RelativeOrAbsolute, out address))
            {
                pbWebPageLoad.Visibility = System.Windows.Visibility.Visible;
                pbWebPageLoad.State = Elysium.Controls.ProgressState.Indeterminate;
                webBrowserMain.Source = address;
                System.Drawing.Image img = null;
                img = getFavicon(webBrowserMain.Source.AbsoluteUri);
                Favicon = getImageSource(img);
            }
        }

        /// <summary>
        /// This method is used to get the favicon from a particular website
        /// </summary>
        /// <param name="u">string URL</param>
        /// <returns></returns>
        public System.Drawing.Image getFavicon(String u)
        {
            Uri url = new Uri(u);
            String iconurl = "http://" + url.Host + "/favicon.ico";
            
            try
            {
                WebRequest request = WebRequest.Create(iconurl);
                WebResponse response = request.GetResponse();
                Stream s = response.GetResponseStream();
                return System.Drawing.Image.FromStream(s);
            }
            catch (Exception)
            {
                //return Properties.Resources.LargeGlobe;
                return null;
            }
        }

        /// <summary>
        /// This method is used to return a Bitmap Image from an Drawing.Image value
        /// </summary>
        /// <param name="value">System.Drawing.Image image</param>
        /// <returns></returns>
        private BitmapImage getImageSource(System.Drawing.Image value)
        {
            if (value == null) { return null; }

            var image = (System.Drawing.Image)value;
            // Winforms Image we want to get the WPF Image from...
            var bitmap = new System.Windows.Media.Imaging.BitmapImage();
            bitmap.BeginInit();
            MemoryStream memoryStream = new MemoryStream();
            // Save to a memory stream...
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            // Rewind the stream...
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();
            return bitmap;
        }

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
                    getWebPageTitle(txtURL.Text.ToString());

                    //if (this.PageLoadCompleted != null)
                    //    this.PageLoadCompleted(this, e); 
                    //SetCursorPos(36, 120);
                    LeftMouseClick(36, 120);
                }
            }
        }

        /// <summary>
        /// This method is used to get the title of a particular webpage
        /// </summary>
        /// <param name="url"></param>
        private void getWebPageTitle(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest.Create(url) as HttpWebRequest);
                HttpWebResponse response = (request.GetResponse() as HttpWebResponse);

                using (Stream stream = response.GetResponseStream())
                {
                    // compiled regex to check for <title></title> block
                    Regex titleCheck = new Regex(@"<title>\s*(.+?)\s*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    int bytesToRead = 8092;
                    byte[] buffer = new byte[bytesToRead];
                    string contents = "";
                    int length = 0;
                    while ((length = stream.Read(buffer, 0, bytesToRead)) > 0)
                    {
                        // convert the byte-array to a string and add it to the rest of the
                        // contents that have been downloaded so far
                        contents += Encoding.UTF8.GetString(buffer, 0, length);

                        Match m = titleCheck.Match(contents);
                        if (m.Success)
                        {
                            // we found a <title></title> match =]
                            PageTitle = m.Groups[1].Value.ToString();
                            break;
                        }
                        else if (contents.Contains("</head>"))
                        {
                            // reached end of head-block; no title found =[
                            break;
                        }
                    }
                }
                SetHeaderAndIcon();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetHeaderAndIcon()
        {
            var parent = (UIElement)this.Parent;
            var _parent = (TabItem)parent;
            if (!String.IsNullOrEmpty(PageTitle))
            {
                _parent.Header = new TabItemHeader(Favicon, PageTitle);
            }
            else
            {
                _parent.Header = new TabItemHeader(Favicon, webBrowserMain.Source.Host);
            }
        }

        /// <summary>
        /// This event will fire when the address of the web control has been changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowserMain_AddressChanged(object sender, Awesomium.Core.UrlEventArgs e)
        {
            if (!String.IsNullOrEmpty(webBrowserMain.Source.AbsoluteUri.ToString())) 
                txtURL.Text = webBrowserMain.Source.AbsoluteUri.ToString();
        }

        # region Bookmark Buttons

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

        #endregion

    }
}
