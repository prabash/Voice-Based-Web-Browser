using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
                img = favicon(webBrowserMain.Source.AbsoluteUri);
                imgIcon.Source = getImageSource(img);

            }
        }

        public System.Drawing.Image favicon(String u)
        {
            Uri url = new Uri(u);
            String iconurl = "http://" + url.Host + "/favicon.ico";

            WebRequest request = WebRequest.Create(iconurl);
            try
            {
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

        private void webBrowserMain_LoadingFrameComplete(object sender, Awesomium.Core.FrameEventArgs e)
        {
            if (e.IsMainFrame)
            {
                if (webBrowserMain.IsDocumentReady)
                {
                    pbWebPageLoad.State = Elysium.Controls.ProgressState.Normal;
                    pbWebPageLoad.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
    }
}
