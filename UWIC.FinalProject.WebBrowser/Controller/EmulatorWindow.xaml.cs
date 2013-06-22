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

        private string _command;
        public string EmulatorCommand 
        {
            get { return _command; }
            set { _command = value; }
        }

        [CanBeNull] private ICommand _functionCommand;
        public ICommand FunctionCommand
        {
            get { return _functionCommand ?? (_functionCommand = new RelayCommand(ExecuteFunction)); }
        }

        public static void SetBrowserContainerViewModel(BrowserContainerViewModel ob)
        {
            BcViewModel = ob;
        }

        private void ExecuteFunction()
        {
            var speechEngine = new SpeechEngine(SpeechRecognitionMode.Emulator);
            if (String.IsNullOrEmpty(EmulatorCommand)) return;
            speechEngine.StartEmulatorRecognition(EmulatorCommand);
            speechEngine.SpeechRecognized += speechEngine_SpeechRecognized;
        }

        void speechEngine_SpeechRecognized(object sender, EventArgs e)
        {
            var speechEngine = (SpeechEngine)sender;
            var recognizedWebsite = speechEngine.RecognizedWebsite;
            if (String.IsNullOrEmpty(recognizedWebsite)) return;
            Uri url;
            if (Uri.TryCreate("http://www."+recognizedWebsite+".com", UriKind.RelativeOrAbsolute, out url))
            {
                BcViewModel.NavigateToURL(url);
            }
        }
    }
}
