using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public static class RecognitionEngine
    {
        public static void getNavigationCommand(string word)
        {
            Match match = Regex.Match(word, @"go to [a-zA-Z]*", RegexOptions.IgnoreCase);
        }
    }
}
