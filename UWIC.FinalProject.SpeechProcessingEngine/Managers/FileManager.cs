using System.IO;

namespace UWIC.FinalProject.SpeechProcessingEngine.Managers
{
    public class FileManager
    {
        /// <summary>
        /// this method will acquire test files from the data folder
        /// </summary>
        public static string[] GetTestFiles()
        {
            return Directory.GetFiles("..//..//data//", "*.txt");
        }
    }
}
