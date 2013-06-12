using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public static class GrammarManager
    {
        public static GrammarBuilder getNavigationGrammar()
        {
            Settings.CultureInfo = "en-GB";
            GrammarBuilder _builder = new GrammarBuilder();
            _builder.Append(new Choices("Go", "Move"));
            _builder.Append(new Choices("to"));
            _builder.Append(new Choices("Google"));
            _builder.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);
            return _builder;
        }

        public static Grammar getDictationGrammar()
        {
            Settings.CultureInfo = "en-GB";

            GrammarBuilder dictaphoneGB = new GrammarBuilder();
            dictaphoneGB.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);
            GrammarBuilder dictation = new GrammarBuilder();
            dictation.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);
            dictation.AppendDictation();

            dictaphoneGB.Append(new SemanticResultKey("StartDictation", new SemanticResultValue("Start Dictation", true)));
            dictaphoneGB.Append(new SemanticResultKey("DictationInput", dictation));
            dictaphoneGB.Append(new SemanticResultKey("EndDictation", new SemanticResultValue("Stop Dictation", false)));

            GrammarBuilder spelling = new GrammarBuilder();
            spelling.AppendDictation("spelling");
            spelling.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);
            GrammarBuilder spellingGB = new GrammarBuilder();
            spellingGB.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);
            spellingGB.Append(new SemanticResultKey("StartSpelling", new SemanticResultValue("Start Spelling", true)));
            spellingGB.Append(new SemanticResultKey("spellingInput", spelling));
            spellingGB.Append(new SemanticResultKey("StopSpelling", new SemanticResultValue("Stop Spelling", true)));

            GrammarBuilder both = GrammarBuilder.Add(dictaphoneGB, spellingGB);
            both.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);

            Grammar grammar = new Grammar(both);
            grammar.Enabled = true;
            grammar.Name = "Dictaphone and Spelling ";

            return grammar;
        }

        public static Grammar getDictGrammarTest()
        {
            Settings.CultureInfo = "en-GB";

            GrammarBuilder dictaphoneGB = new GrammarBuilder();
            dictaphoneGB.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);
            dictaphoneGB.Append(new Choices("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"));
            GrammarBuilder dictation = new GrammarBuilder();
            dictation.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);
            dictation.AppendDictation();

            GrammarBuilder both = GrammarBuilder.Add(dictaphoneGB, dictation);
            both.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);

            Grammar grammar = new Grammar(both);

            return grammar;
        }
    }
}
