using System;
using System.Collections.Generic;
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
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for MessageBoxWindow.xaml
    /// </summary>
    public partial class MessageBoxWindow : Elysium.Controls.Window
    {
        private string Message { get; set; }
        private string MessageTitle { get; set; }
        private Visibility OkButtonVisibility { get; set; }
        private Visibility YesButtonVisibility { get; set; }
        private Visibility NoButtonVisibility { get; set; }
        private MessageBoxIcon MessageIcon { get; set; }

        public MessageBoxWindow()
        {
            InitializeComponent();
        }

        public MessageBoxWindow(string message, string messageTitle, Visibility yesButtonVisible, Visibility noButtonVisible, MessageBoxIcon icon)
        {
        }

        public MessageBoxWindow(string message, string messageTitle, Visibility okButtonVisible, MessageBoxIcon icon)
        {
        }
    }
}
