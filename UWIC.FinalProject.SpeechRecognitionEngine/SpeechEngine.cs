using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public class SpeechEngine
    {
        public SpeechEngine(SpeechRecognitionMode mode)
        {
            switch (mode)
            {
                case SpeechRecognitionMode.Emulator:
                    {
                        initializeEmulator();
                        break;
                    }
                case SpeechRecognitionMode.Recognition:
                    {
                        break;
                    }
            }
        }
        
        static System.Speech.Recognition.SpeechRecognitionEngine recognizer = null;
        static ManualResetEvent manualResetEvent = null;

        private void initializeEmulator()
        {
            GrammarBuilder builder = new GrammarBuilder();
            builder.AppendDictation();

            recognizer = new System.Speech.Recognition.SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-GB"));
            recognizer.RequestRecognizerUpdate();
            //recognizer.LoadGrammar(new Grammar(GrammarManager.getNavigationGrammar())); // load speech grammar
            //recognizer.LoadGrammar(GrammarManager.getDictationGrammar());
            recognizer.LoadGrammar(new DictationGrammar());
            //recognizer.LoadGrammar(GrammarManager.getDictGrammarTest());
            recognizer.SpeechRecognized += recognizer_SpeechRecognized;
        }

        public void startEmulatorRecognition(string word)
        {
            recognizer.EmulateRecognizeAsync(word);
        }

        void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string val = e.Result.Text;
            RecognitionEngine.getNavigationCommand(val);
        }
    }
}
