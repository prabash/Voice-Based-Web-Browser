using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        }
        
        Uri address;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Uri.TryCreate(txtURL.Text, UriKind.RelativeOrAbsolute, out address))
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
        
    }
}
