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

namespace UWIC.FinalProject.WebBrowser.View
{
    /// <summary>
    /// Interaction logic for TestEmulator.xaml
    /// </summary>
    public partial class TestEmulator : Elysium.Controls.Window
    {
        SpeechRecognitionEngine.SpeechEngine speechEngine { get; set; }
        public TestEmulator()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            speechEngine = new SpeechRecognitionEngine.SpeechEngine(SpeechRecognitionEngine.SpeechRecognitionMode.Emulator);
            string val = txtSpeech.Text;
            speechEngine.StartEmulatorRecognition(val);
        }
    }
}
