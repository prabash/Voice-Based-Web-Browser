using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Elysium.Controls;
using UWIC.FinalProject.WebBrowser.Controller;
using UWIC.FinalProject.WebBrowser.ViewModel;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.WebBrowser.View
{
    /// <summary>
    /// Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Elysium.Controls.Window
    {
        public BrowserWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string url;
            if (new UpdateManager().CheckForUpdates(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, out url))
                System.Diagnostics.Process.Start(url);
        }
    }
}