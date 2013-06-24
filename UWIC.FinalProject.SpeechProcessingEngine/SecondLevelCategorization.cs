using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    public class SecondLevelCategorization
    {
        private List<string> Func_Browser_CommandList;
        private List<string> Func_Interface_CommandList;
        private List<string> Func_Mouse_CommandList;
        private List<string> Func_Page_CommandList;

        private readonly List<string> SpeechText;
        private SecondLevelCategorization SecondLvlCategorization { get; set; }
        private readonly FirstLevelProbabilityIndex _firstLevelProbabilityIndex;

        private SecondLevelCategorization(List<string> speechText, FirstLevelProbabilityIndex firstlevelProbabilityIndex)
        {
            SpeechText = speechText;
            _firstLevelProbabilityIndex = firstlevelProbabilityIndex;
        }

        public SecondLevelCategorization GetSecondLevelCategorization(List<string> speechText,
                                                                      FirstLevelProbabilityIndex
                                                                          firstlevelProbabilityIndex)
        {
            return
                SecondLvlCategorization != null
                    ? new SecondLevelCategorization(speechText, firstlevelProbabilityIndex)
                    : SecondLvlCategorization;
        }

        private void LoadSecondLevelCategorizations()
        {
            if (_firstLevelProbabilityIndex == FirstLevelProbabilityIndex.FunctionalCommand)
                AcquireTestFiles();
            else if (_firstLevelProbabilityIndex == FirstLevelProbabilityIndex.KeyboardCommand)
            {

            }

        }

        /// <summary>
        /// this method will acquire test files from the data folder
        /// </summary>
        private void AcquireTestFiles()
        {
            var testFiles = Directory.GetFiles("..//..//data//", "*.txt");
            foreach (var testFile in testFiles)
            {
                var explicitFileName = testFile.Replace("..//..//data//", String.Empty);
                var prefix = explicitFileName.Substring(0, 3);
                if (prefix == "fnc")
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
            var prefix = explicitFileName.Substring(0, 9);
            CheckFunctionalCategory(prefix, tempList);
        }

        /// <summary>
        /// This method will check the functional category of the test data according to the prefix and assign them to 
        /// relevant test sets
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="testData">Test data set acquired from the local folder</param>
        private void CheckFunctionalCategory(string prefix, IEnumerable<string> testData)
        {
            switch (prefix)
            {
                case "fnc_brwsr":
                    AssignDataToTestSet(Func_Browser_CommandList, testData);
                    break;
                case "fnc_intfc":
                    AssignDataToTestSet(Func_Interface_CommandList, testData);
                    break;
                case "fnc_mouse":
                    AssignDataToTestSet(Func_Mouse_CommandList, testData);
                    break;
                case "fnc_wpage":
                    AssignDataToTestSet(Func_Page_CommandList, testData);
                    break;
            }
        }

        /// <summary>
        /// This method is used to assign test data to each category without redundancy
        /// </summary>
        /// <param name="set">The training set to which the data should be added</param>
        /// <param name="currentList">the current data list acquired from the local file</param>
        private static void AssignDataToTestSet(ICollection<string> set, IEnumerable<string> currentList)
        {
            if (currentList != null)
                foreach (var item in currentList.Where(item => !set.Contains(item)))
                {
                    set.Add(item);
                }
        }

        public void GetSecondLevelProbabilityIndex()
        {
            switch (_firstLevelProbabilityIndex)
            {
                case FirstLevelProbabilityIndex.FunctionalCommand:
                    InitializeSecondLevelFunctionalCommandCalculation();
                    break;
            }   
        }

        private void InitializeSecondLevelFunctionalCommandCalculation()
        {
            foreach (var segment in SpeechText)
            {
                CalculateSecondLevelProbabilityBySegment(segment);
            }
        }

        private void CalculateSecondLevelProbabilityBySegment(string segment)
        {
           
        }
    }
}
