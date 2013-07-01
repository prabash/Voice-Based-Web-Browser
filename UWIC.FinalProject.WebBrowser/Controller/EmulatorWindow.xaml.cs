using System.ComponentModel;
using JetBrains.Annotations;
using Microsoft.TeamFoundation.MVVM;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UWIC.FinalProject.SpeechRecognitionEngine;
using UWIC.FinalProject.WebBrowser.Model;
using UWIC.FinalProject.WebBrowser.ViewModel;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for EmulatorWindow.xaml
    /// </summary>
    public partial class EmulatorWindow : UserControl
    {
        public static BrowserContainerViewModel BcViewModel;

        public EmulatorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The image displayed by the button.
        /// </summary>
        /// <remarks>The image is specified in XAML as an absolute or relative path.</remarks>
        [Description("The image displayed by the button"), Category("Common Properties")]
        public string CommandText
        {
            get { return (string)GetValue(CommandTxt); }
            set { SetValue(CommandTxt, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty CommandTxt = DependencyProperty.Register("CommandText", typeof(string), typeof(BookmarkButton), new UIPropertyMetadata(null));

        [Description("The image displayed by the button"), Category("Common Properties")]
        public ICommand EmulatorCommand
        {
            get { return (ICommand)GetValue(EmulatorCmd); }
            set { SetValue(EmulatorCmd, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty EmulatorCmd = DependencyProperty.Register("EmulatorCommand", typeof(ICommand), typeof(BookmarkButton), new UIPropertyMetadata(null));

        //public static void SetBrowserContainerViewModel(BrowserContainerViewModel ob)
        //{
        //    BcViewModel = ob;
        //}

        

        //void speechEngine_SpeechRecognized(object sender, EventArgs e)
        //{
        //    var speechEngine = (SpeechEngine)sender;
        //    var resultDictionary = speechEngine.ResultDictionary;
        //    new CommandExecutionManager(BcViewModel).ExecuteCommand(resultDictionary);
        //}
    }
}
