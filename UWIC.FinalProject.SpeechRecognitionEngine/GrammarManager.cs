using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public class GrammarManager
    {
        public GrammarManager()
        {
            Settings.CultureInfo = "en-GB";
        }

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

            GrammarBuilder dictation = new GrammarBuilder((GrammarBuilder)dictaphoneGB, 1, 200);
            dictation.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);

            Grammar grammar = new Grammar(dictation);

            return grammar;
        }

        public static GrammarBuilder getAlphabet()
        {
            Settings.CultureInfo = "en-GB";

            GrammarBuilder dictaphoneGB = new GrammarBuilder();
            dictaphoneGB.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);
            dictaphoneGB.Append(new Choices("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"));
            
            return dictaphoneGB;
        }

        public static GrammarBuilder getWebsitenames()
        {
            Settings.CultureInfo = "en-GB";
            List<string> webSiteNames = new List<string>();

            using (FileStream fs = File.Open("..//..//data//Websites" + ".txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    webSiteNames.Add(line);
                }
            }

            GrammarBuilder dictaphoneGB = new GrammarBuilder();
            dictaphoneGB.Culture = new System.Globalization.CultureInfo(Settings.CultureInfo);
            dictaphoneGB.Append(new Choices("go", "move"));
            dictaphoneGB.Append(new Choices("to"));
            dictaphoneGB.Append(new Choices(webSiteNames.ToArray()));
            return dictaphoneGB;
        }
    }
}
