using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    public class FirstLevelCategorization
    {
        private List<CategoryCollection> _categoryCollection;
        private List<ProbabilityScoreIndex> _probabilityScoreIndices; 

        /// <summary>
        /// Constructor
        /// </summary>
        public FirstLevelCategorization()
        {
            DefinePrimaryCategories();
            AcquireTestFiles();
        }

        /// <summary>
        /// This method will define the first level Categories for the Naive Command Categorization
        /// </summary>
        private void DefinePrimaryCategories()
        {
            _categoryCollection = new List<CategoryCollection>
                {
                    new CategoryCollection
                        {
                            Category = FirstLevelCategory.Functionalcommand,
                            Id = 1,
                            List = new List<string>(),
                            Name = "FunctionalCommandList"
                        },
                    new CategoryCollection
                        {
                            Category = FirstLevelCategory.Keyboardcommand,
                            Id = 2,
                            List = new List<string>(),
                            Name = "KeyboardCommandList"
                        },
                    new CategoryCollection
                        {
                            Category = FirstLevelCategory.Mousecommand,
                            Id = 3,
                            List = new List<string>(),
                            Name = "MouseCommandList"
                        }
                };
        }

        /// <summary>
        /// this method will acquire test files from the data folder
        /// </summary>
        private void AcquireTestFiles()
        {
            var testFiles = Directory.GetFiles("..//..//data//", "*.txt");
            foreach (var testFile in testFiles)
            {
                GetTestSet(testFile);
            }
        }

        /// <summary>
        /// this method is used to acquire and assign test data to relevant test sets by passing the file path
        /// </summary>
        /// <param name="filepath">the path of the file</param>
        private void GetTestSet(string filepath)
        {
            try
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
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
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
                case "fnc":
                    var funcCommand = _categoryCollection.FirstOrDefault(rec => rec.Id == 1);
                    if (funcCommand != null)
                        AssignDataToTestSet(funcCommand.List, testData);
                    break;
                case "key":
                    var keyCommand = _categoryCollection.FirstOrDefault(rec => rec.Id == 2);
                    if (keyCommand != null)
                        AssignDataToTestSet(keyCommand.List, testData);
                    break;
                case "mse":
                    var mouseCommand = _categoryCollection.FirstOrDefault(rec => rec.Id == 2);
                    if (mouseCommand != null)
                        AssignDataToTestSet(mouseCommand.List, testData);
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

        /// <summary>
        /// This method is used to calculate the First level of probability of a given command
        /// </summary>
        /// <param name="command"></param>
        public void CalculateProbabilityOfCommand(string command)
        {
            new NaiveCommandCategorization(_categoryCollection).CalculateProbabilityOfSegments(command.Split(' ').ToList(), out _probabilityScoreIndices);
            if(_probabilityScoreIndices != null)
                if(_probabilityScoreIndices.Count > 0)
                    foreach (var probabilityScoreIndex in _probabilityScoreIndices)
                    {
                        var firstOrDefault = _categoryCollection.FirstOrDefault(rec => rec.Id == probabilityScoreIndex.ReferenceId);
                        if (firstOrDefault !=
                            null)
                            Console.WriteLine(probabilityScoreIndex.ReferenceId + " - " + firstOrDefault.Name + " - " +probabilityScoreIndex.ProbabilityScore);
                    }
        }   
    }
}
