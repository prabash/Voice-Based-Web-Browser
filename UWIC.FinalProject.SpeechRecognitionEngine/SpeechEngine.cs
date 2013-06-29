using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public class SpeechEngine
    {
        #region Events

        public event EventHandler SpeechRecognized;

        #endregion

        # region Variables

        public Dictionary<CommandType, object> ResultDictionary { get; set; } 

        #endregion

        #region Objects

        public GrammarManager GrammarManager;
        static System.Speech.Recognition.SpeechRecognitionEngine _recognizer;

        #endregion

        public SpeechEngine(SpeechRecognitionMode mode)
        {
            GrammarManager = new GrammarManager();
            switch (mode)
            {
                case SpeechRecognitionMode.Emulator:
                {
                    InitializeEmulator();
                    break;
                }
                case SpeechRecognitionMode.Recognition:
                {
                    break;
                }
            }
        }

        private void InitializeEmulator()
        {
            var builder = new GrammarBuilder();
            builder.AppendDictation();

            _recognizer = new System.Speech.Recognition.SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-GB"));
            _recognizer.RequestRecognizerUpdate();
            LoadGrammars(_recognizer);
            _recognizer.SpeechRecognized += recognizer_SpeechRecognized;
        }

        private void LoadGrammars(System.Speech.Recognition.SpeechRecognitionEngine recognizer)
        {
            recognizer.LoadGrammar(new DictationGrammar());
            recognizer.LoadGrammar(new Grammar(GrammarManager.GetSpellGrammar()));
            recognizer.LoadGrammar(new Grammar(GrammarManager.GetWebsiteNamesGrammar()));
        }

        public void StartEmulatorRecognition(string word)
        {
            _recognizer.EmulateRecognizeAsync(word);
        }

        void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var val = e.Result.Text;
            //RecognizedWebsite = RecognitionEngine.getNavigationCommand(val);
            ResultDictionary =
                new FirstLevelCategorization().CalculateProbabilityOfCommand(RemoveAnomalies(val).ToLower().Trim());
            
            if (SpeechRecognized != null)
                SpeechRecognized(this, e); 
        }

        private static string RemoveAnomalies(string val)
        {
            return val.Replace("\t", " tab ")
                      .Replace("1", "one ")
                      .Replace("2", "two ")
                      .Replace("3", "three ")
                      .Replace("4", "four ")
                      .Replace("5", "five ")
                      .Replace("6", "six ")
                      .Replace("7", "seven ")
                      .Replace("8", "eight ")
                      .Replace("9", "nine ")
                      .Replace("0", "zero ");
        }
    }
}
