using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public class GrammarManager
    {
        public GrammarManager()
        {
            Settings.CultureInfo = "en-GB";
        }

        public GrammarBuilder GetSpellGrammar()
        {
            var dictationBuilder = new GrammarBuilder // creating a new grammar builder
            {
                Culture = new CultureInfo(Settings.CultureInfo)
            };
            dictationBuilder.AppendDictation(); // append dictation to the created grammar builder
            
            var dictaphoneGb = new GrammarBuilder {Culture = new CultureInfo(Settings.CultureInfo)};
            dictaphoneGb.Append(new Choices("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"));
            
            var dictation = new GrammarBuilder {Culture = new CultureInfo(Settings.CultureInfo)};
            dictation.Append(dictationBuilder, 0 /* minimum repeat */, 10 /* maximum repeat*/ );
            dictation.Append(dictaphoneGb, 0, 200);
            dictation.Append(dictationBuilder, 0 /* minimum repeat */, 10 /* maximum repeat*/ );
            dictation.Append(dictaphoneGb, 0, 200);
            dictation.Append(dictationBuilder, 0 /* minimum repeat */, 10 /* maximum repeat*/ );
            return dictation;
        }

        public GrammarBuilder GetWebsiteNamesGrammar()
        {
            Settings.CultureInfo = "en-GB";
            var webSiteNames = new List<string>();
            using (var fs = File.Open("..//..//data//fnc_brwsr_websites" + ".txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    webSiteNames.Add(line);
                }
            }

            var dictationBuilder = new GrammarBuilder // creating a new grammar builder
                {
                    Culture = new CultureInfo(Settings.CultureInfo)
                };
            dictationBuilder.AppendDictation(); // append dictation to the created grammar builder

            var dictaphoneGb = new GrammarBuilder {Culture = new CultureInfo(Settings.CultureInfo)};
            dictaphoneGb.Append(dictationBuilder, 0 /* minimum repeat */, 10 /* maximum repeat*/ );
            dictaphoneGb.Append(new Choices(webSiteNames.ToArray()));
            dictaphoneGb.Append(dictationBuilder, 0 /* minimum repeat */, 10 /* maximum repeat*/ );
            return dictaphoneGb;
        }
    }
}
