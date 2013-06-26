using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    public class SecondLevelCategorization
    {
        private List<CategoryCollection> _secondLevelCategoryCollection;
        private List<ProbabilityScoreIndex> _secondLevelProbabilityScoreIndices; 

        /// <summary>
        /// Constructor
        /// </summary>
        public SecondLevelCategorization()
        {
            DefineSecondaryCategories();
            AcquireTestFiles();
        }

        /// <summary>
        /// This method will define the Second level Categories for the Naive Command Categorization
        /// </summary>
        private void DefineSecondaryCategories()
        {
            _secondLevelCategoryCollection = new List<CategoryCollection>
                {
                    new CategoryCollection
                        {
                            Category = SecondLevelCategory.BrowserCommand,
                            Id = 1,
                            List = new List<string>(),
                            Name = "BrowserCommandList"
                        },
                    new CategoryCollection
                        {
                            Category = SecondLevelCategory.InterfaceCommand,
                            Id = 2,
                            List = new List<string>(),
                            Name = "InterfaceCommandList"
                        },
                    new CategoryCollection
                        {
                            Category = SecondLevelCategory.MouseCommand,
                            Id = 3,
                            List = new List<string>(),
                            Name = "MouseCommandList"
                        },
                    new CategoryCollection
                        {
                            Category = SecondLevelCategory.WebPageCommand,
                            Id = 4,
                            List = new List<string>(),
                            Name = "WebPageCommand"
                        }
                };
        }

        /// <summary>
        /// this method will acquire test files from the data folder
        /// </summary>
        private void AcquireTestFiles()
        {
            var testFiles = FileManager.GetTestFiles();
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
                var explicitFileName = filepath.Replace("..//..//data//", String.Empty);
                if (explicitFileName.Substring(0, 3) != "fnc") return;
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
                var prefix = explicitFileName.Substring(4, 5);
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
                case "brwsr":
                    var browserCommand = _secondLevelCategoryCollection.FirstOrDefault(rec => rec.Category == SecondLevelCategory.BrowserCommand);
                    if (browserCommand != null)
                        DataManager.AssignDataToTestSet(browserCommand.List, testData);
                    break;
                case "intfc":
                    var interfaceCommand = _secondLevelCategoryCollection.FirstOrDefault(rec => rec.Category == SecondLevelCategory.InterfaceCommand);
                    if (interfaceCommand != null)
                        DataManager.AssignDataToTestSet(interfaceCommand.List, testData);
                    break;
                case "mouse":
                    var mouseCommand = _secondLevelCategoryCollection.FirstOrDefault(rec => rec.Category == SecondLevelCategory.MouseCommand);
                    if (mouseCommand != null)
                        DataManager.AssignDataToTestSet(mouseCommand.List, testData);
                    break;
                case "wpage":
                    var webPageCommand = _secondLevelCategoryCollection.FirstOrDefault(rec => rec.Category == SecondLevelCategory.WebPageCommand);
                    if (webPageCommand != null)
                        DataManager.AssignDataToTestSet(webPageCommand.List, testData);
                    break;
            }
        }
        
        /// <summary>
        /// This method is used to calculate the Second level of probability of a given command
        /// </summary>
        /// <param name="command"></param>
        public void CalculateSecondLevelProbabilityOfCommand(string command)
        {
            new NaiveCommandCategorization(_secondLevelCategoryCollection).CalculateProbabilityOfSegments(command.Split(' ').ToList(), out _secondLevelProbabilityScoreIndices);
            if (_secondLevelProbabilityScoreIndices == null) return;
            var highestProbabilityCategories = new NaiveCommandCategorization().GetHighestProbabilityScoreIndeces(_secondLevelProbabilityScoreIndices);
            if (highestProbabilityCategories != null)
            {
                foreach (var highestProbabilityCategory in highestProbabilityCategories)
                {
                    Console.WriteLine(highestProbabilityCategory.ReferenceId + " - " + highestProbabilityCategory.ProbabilityScore);
                }
            }
        }
    }
}
