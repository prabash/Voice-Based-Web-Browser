using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public class SpeechEngine
    {
        #region Events

        public event EventHandler SpeechProcessed;

        #endregion

        # region Variables

        public Dictionary<CommandType, object> ResultDictionary { get; set; }

        public Mode CommandMode { get; set; }

        #endregion

        #region Objects

        public GrammarManager GrammarManager;
        static System.Speech.Recognition.SpeechRecognitionEngine _recognizer;

        #endregion

        # region Emulator

        public void InitializeEmulator()
        {
            var builder = new GrammarBuilder();
            builder.AppendDictation();

            _recognizer = new System.Speech.Recognition.SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-GB"));
            _recognizer.RequestRecognizerUpdate();
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.LoadGrammar(GetSpellingGrammar());
            _recognizer.LoadGrammar(GetWebSiteNamesGrammar());
            _recognizer.SpeechRecognized += recognizer_SpeechRecognized;
        }

        public void StartEmulatorRecognition(string word)
        {
            _recognizer.EmulateRecognizeAsync(word);
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var value = e.Result.Text;
            InitializeSpeechProcessing(value);
        }

        # endregion

        # region Grammar

        public Grammar GetSpellingGrammar()
        {
            GrammarManager = new GrammarManager();
            return new Grammar(GrammarManager.GetSpellGrammar());
        }

        public Grammar GetWebSiteNamesGrammar()
        {
            GrammarManager = new GrammarManager();
            return new Grammar(GrammarManager.GetWebsiteNamesGrammar());
        }

        #endregion

        # region Speech Processing

        public Dictionary<CommandType, object> InitializeSpeechProcessing(string val)
        {
            try
            {
                //RecognizedWebsite = RecognitionEngine.getNavigationCommand(val);
                switch (CommandMode)
                {
                    case Mode.CommandMode:
                        ResultDictionary =
                            new FirstLevelCategorization().CalculateProbabilityOfCommand(RemoveAnomalies(val).ToLower().Trim());
                        break;
                    case Mode.WebsiteSpellMode:
                    case Mode.GeneralSpellMode:
                    case Mode.DictationMode:
                        ResultDictionary = new Dictionary<CommandType, object> { { CommandType.alter, val } };
                        break;
                }

                var e = new SpeechProcessedEventArgs
                    {
                    Dictionary = ResultDictionary
                };

                if (SpeechProcessed != null)
                    SpeechProcessed(this, e);
                return ResultDictionary;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
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

        #endregion

        #region Voice Recognizer

        /// <summary>
        /// Creates the speech engine.
        /// </summary>
        /// <param name="preferredCulture">The preferred culture.</param>
        /// <param name="result">returns a result</param>
        /// <returns></returns>
        public System.Speech.Recognition.SpeechRecognitionEngine CreateSpeechEngine(string preferredCulture, out string result)
        {
            var speechRecognitionEngine = (from config in System.Speech.Recognition.SpeechRecognitionEngine.InstalledRecognizers() where config.Culture.ToString() == preferredCulture select new System.Speech.Recognition.SpeechRecognitionEngine(config)).FirstOrDefault();
            result = "Success";
            // if the desired culture is not found, then load default
            if (speechRecognitionEngine == null)
            {
                speechRecognitionEngine = new System.Speech.Recognition.SpeechRecognitionEngine(System.Speech.Recognition.SpeechRecognitionEngine.InstalledRecognizers()[0]);
                result = "The desired culture is not installed on this machine, the speech-engine will continue using "
                                    +
                                    System.Speech.Recognition.SpeechRecognitionEngine.InstalledRecognizers()[0].Culture +
                                    " as the default culture. " +
                                    "Culture " + preferredCulture + " not found!";
            }

            return speechRecognitionEngine;
        }

        #endregion
    }

    public class SpeechProcessedEventArgs : EventArgs
    {
        public Dictionary<CommandType, object> Dictionary { get; set; }
    }
}
