using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    public class ProcessorEngine
    {
        private List<string> FunctionalCommands { get; set; }
        private List<string> MouseCommands { get; set; }
        private List<string> KeyboardCommands { get; set; }

        private List<string> SpeechText { get; set; }

        #region First Level Probability Indeces
        private double _funcCommandProbabilityScore;
        private double _mouseCommandProbabilityScore;
        private double _keyboardCommandProbabilityScore;
        # endregion

        public ProcessorEngine()
        {
            FunctionalCommands = new List<string>();
            MouseCommands = new List<string>();
            KeyboardCommands = new List<string>();
            //LoadTrainedSets();
            AcquireTestFiles();
        }

        public void CalculateProbabilityOfCommand(string command)
        {
            SpeechSegmentation(command);
        }

        public void SpeechSegmentation(string phrase)
        {
            SpeechText = phrase.Split(' ').ToList();
            foreach (var segment in SpeechText)
            {
                CaluclateFirstLevelProbabilityBySegment(segment.ToLower());
            }
            CalculateSecondLevelProbability();
        }

        //private void LoadTrainedSets()
        //{
        //    AssignToSet(FunctionalCommands, "Func_Commands");
        //    AssignToSet(FunctionalCommands, "Nav_Func");
        //    AssignToSet(FunctionalCommands, "Mouse_Func");
        //    AssignToSet(FunctionalCommands, "Browser_Func");
        //    AssignToSet(MouseCommands, "Mouse_Commands");
        //    AssignToSet(KeyboardCommands, "Key_Commands");
        //}

        //private static void AssignToSet(List<string> setName, string fileName)
        //{
        //    var tempList = new List<string>();
        //    using (var fs = File.Open("..//..//data//" + fileName + ".txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //    using (var bs = new BufferedStream(fs))
        //    using (var sr = new StreamReader(bs))
        //    {
        //        string line;
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            tempList.Add(line);
        //        }
        //    }
        //    setName.AddRange(tempList);
        //}

        private void CaluclateFirstLevelProbabilityBySegment(string segment)
        {
            double funcComPob = 0;
            double mouseComPob = 0;
            double keyComPob = 0;

            var availableInFunc = CheckAvailabilityInSet(FunctionalCommands, segment); // Check whether the segment is available in the Functional Commands list
            var availableInMouse = CheckAvailabilityInSet(MouseCommands, segment); // Check whether the segment is available in the Mouse Commands list
            var availableInKey = CheckAvailabilityInSet(KeyboardCommands, segment); // Check whether the segment is available in the Key Commands list

            var noOfAvailabilities = Convert.ToInt32(availableInFunc) + Convert.ToInt32(availableInMouse) +
                                      Convert.ToInt32(availableInKey); //Calculate the Total number of availabilites
            var probabilityOfBelongness = noOfAvailabilities > 0 ? (1.0 / Convert.ToDouble(noOfAvailabilities)) : 0;

            if (availableInFunc) // If the command is available in functional Commands
                funcComPob = probabilityOfBelongness; // assign the probability of belogness to Functional Command
            if (availableInMouse) // If the command is available in mouse commands
                mouseComPob = probabilityOfBelongness; // assign the probability of belogness to the Mouse Command
            if (availableInKey) // if the command is available in keyboard commands
                keyComPob = probabilityOfBelongness; // assign the probability of belogness to the Keyboard Command

            _funcCommandProbabilityScore += funcComPob; // Concat the current probability of belogness to the probability of entire command belonging to a Functional Command
            _mouseCommandProbabilityScore += mouseComPob; // Concat the current probability of belogness to the probability of entire command belonging to a Mouse Command
            _keyboardCommandProbabilityScore += keyComPob; // Concat the current probability of belogness to the probability of entire command belonging to a Key Command
        }

        /// <summary>
        /// This method is used to check whether a particular object is available inside any give collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        private static bool CheckAvailabilityInSet(IEnumerable<string> collection, object segment)
        {
            return collection.Contains(segment);
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
            CheckFunctionalCategory(prefix, tempList);
        }

        /// <summary>
        /// This method will check the functional category of the test data according to the prefix and assign them to 
        /// relevant test sets
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="testData">Test data set acquired from the local folder</param>
        private void CheckFunctionalCategory(string prefix, IEnumerable<string>testData)
        {
            switch (prefix)
            {
                case "fnc":
                    AssignDataToTestSet(FunctionalCommands, testData);
                    break;
                case "mse":
                    AssignDataToTestSet(MouseCommands, testData);
                    break;
                case "key":
                    AssignDataToTestSet(KeyboardCommands, testData);
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

        private void CalculateSecondLevelProbability()
        {
            //var highestFirstLevelProbabilityIndex = GetHighestFirstLevelIndex(AssignFirstLevelIndecesToDictionary());
            //switch (highestFirstLevelProbabilityIndex)
            //{
            //    case FirstLevelProbabilityIndex.FunctionalCommand:
            //        break;
            //}
        }

        //private Dictionary<FirstLevelProbabilityIndex, double> AssignFirstLevelIndecesToDictionary()
        //{
        //    var firstLeveIndecesDictionary = new Dictionary<FirstLevelProbabilityIndex, double>
        //        {
        //            {FirstLevelProbabilityIndex.FunctionalCommand, _funcCommandProbabilityScore},
        //            {FirstLevelProbabilityIndex.KeyboardCommand, _keyboardCommandProbabilityScore},
        //            {FirstLevelProbabilityIndex.MouseCommand, _mouseCommandProbabilityScore},
        //        };
        //    return firstLeveIndecesDictionary;
        //}

        ///*Make this method return a list which contains the list of highest values*/
        //private static FirstLevelProbabilityIndex GetHighestFirstLevelIndex(
        //    Dictionary<FirstLevelProbabilityIndex, double> indexList)
        //{
        //    var currentHighestProbabilityIndex = FirstLevelProbabilityIndex.Default;
        //    double previousValue = 0;
        //    foreach (var item in indexList)
        //    {
        //        currentHighestProbabilityIndex = item.Value > previousValue
        //                                             ? item.Key
        //                                             : FirstLevelProbabilityIndex.Default;
        //        previousValue = item.Value;
        //    }
        //    return currentHighestProbabilityIndex;
        //}
    }
}
