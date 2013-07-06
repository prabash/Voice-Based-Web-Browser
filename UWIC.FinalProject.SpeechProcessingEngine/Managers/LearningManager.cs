using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine.Managers
{
    class LearningManager
    {
        public static List<string> UnIdentifiedWords { get; set; }

        /// <summary>
        /// This method will compare the command type and append unidnetified words to the respective text file
        /// </summary>
        /// <param name="commandType">identified command type</param>
        public static void AddUnidentifiedWordsToCommandText(CommandType commandType)
        {
            try
            {
                var testFiles = FileManager.GetTestFiles();
                var textFile = "";
                foreach (var testFile in testFiles.Where(testFile => testFile.Contains(commandType.ToString())))
                {
                    textFile = testFile;
                }
                DataManager.AppendToFile(textFile, UnIdentifiedWords);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
