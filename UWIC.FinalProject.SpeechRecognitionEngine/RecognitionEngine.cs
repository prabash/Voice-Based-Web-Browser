using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public class RecognitionEngine
    {
        public static void getNavigationCommand(string word)
        {
            var websiteName = "";
            Match match = null;
            match = Regex.Match(word, @"go to [a-zA-Z]*", RegexOptions.IgnoreCase);
            if (match == null)
            {
                match = Regex.Match(word, @"move to [a-zA-Z]*", RegexOptions.IgnoreCase);
                var words = match.Value.ToString();
                websiteName = words.Replace("move to ", String.Empty);
            }
            else
            {
                var words = match.Value.ToString();
                websiteName = words.Replace("go to ", String.Empty);
            }
        }
    }
}
