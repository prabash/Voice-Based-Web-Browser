using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    class KeyboardCommandManager : ICommandManager
    {
        public Common.CommandType GetCommandType(List<string> commandSegments)
        {
            foreach (var commandSegment in commandSegments)
            {

            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// this method will acquire test files from the data folder
        /// </summary>
        private void AcquireTestFiles()
        {
            var testFiles = Directory.GetFiles("..//..//data//", "*.txt");
            foreach (var testFile in testFiles)
            {
                GetTestData(testFile);
            }
        }

        /// <summary>
        /// this method is used to acquire and assign test data to relevant test sets by passing the file path
        /// </summary>
        /// <param name="filepath">the path of the file</param>
        private void GetTestData(string filepath)
        {
            var tempList = new List<string>();
            using (var fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    tempList.Add(line.ToLower());
                }
            }

            var explicitFileName = filepath.Replace("..//..//data//", String.Empty);
            var prefix = explicitFileName.Substring(0, 3);
            //CheckFunctionalCategory(prefix, tempList);
        }
    }
}
