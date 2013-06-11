using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO.Log;
using System.Text.RegularExpressions;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.WebBrowser.Model
{
    public class BrowserContainerModel
    {
        /// <summary>
        /// This method is used to return a Bitmap Image from an Drawing.Image value
        /// </summary>
        /// <param name="value">System.Drawing.Image image</param>
        /// <returns></returns>
        public BitmapImage getFavicon(System.Drawing.Image value)
        {
            try
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
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw ex;
            }
            
        }

        /// <summary>
        /// This method is used to get the favicon from a particular website
        /// </summary>
        /// <param name="u">string URL</param>
        /// <returns></returns>
        public System.Drawing.Image getImageSource(Uri url)
        {
            try
            {
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
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw ex;
            }
        }

        /// <summary>
        /// This method is used to get the title of a particular webpage
        /// </summary>
        /// <param name="url"></param>
        public string getWebPageTitle(string url)
        {
            string Title = "";
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
                            Title = m.Groups[1].Value.ToString();
                            break;
                        }
                        else if (contents.Contains("</head>"))
                        {
                            Title = "";
                            // reached end of head-block; no title found
                            break;
                        }
                    }
                }
                return Title;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw ex;
            }
        }
    }
}
