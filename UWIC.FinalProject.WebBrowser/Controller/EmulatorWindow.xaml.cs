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
using UWIC.FinalProject.WebBrowser.ViewModel;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for EmulatorWindow.xaml
    /// </summary>
    public partial class EmulatorWindow : UserControl
    {
        public static BrowserContainerViewModel _bcViewModel;

        public EmulatorWindow()
        {
            InitializeComponent();
        }

        public string _Command;
        public string EmulatorCommand 
        {
            get { return _Command; }
            set { _Command = value; }
        }

        public ICommand _functionCommand;
        public ICommand FunctionCommand
        {
            get
            {
                if (_functionCommand == null)
                {
                    _functionCommand = new RelayCommand(ExecuteFunction);
                }
                return _functionCommand;
            }
        }

        public static void SetBrowserContainerViewModel(BrowserContainerViewModel ob)
        {
            _bcViewModel = ob;
        }

        private void ExecuteFunction()
        {
            var speechEngine = new SpeechEngine(SpeechRecognitionMode.Emulator);
            if (!String.IsNullOrEmpty(EmulatorCommand))
            {
                speechEngine.startEmulatorRecognition(EmulatorCommand);
                speechEngine.SpeechRecognized += speechEngine_SpeechRecognized;
            }
        }

        void speechEngine_SpeechRecognized(object sender, EventArgs e)
        {
            var speechEngine = (SpeechEngine)sender;
            var recognizedWebsite = speechEngine.RecognizedWebsite;
            if (!String.IsNullOrEmpty(recognizedWebsite))
            {
                Uri url;
                if (Uri.TryCreate("http://www."+recognizedWebsite+".com", UriKind.RelativeOrAbsolute, out url))
                {
                    _bcViewModel.NavigateToURL(url);
                }
            }
        }
    }
}
