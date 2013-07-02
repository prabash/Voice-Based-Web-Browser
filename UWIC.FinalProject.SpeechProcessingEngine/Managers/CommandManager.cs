using System;
using System.Collections.Generic;
using System.Linq;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine.Managers
{
    public class CommandManager : ICommandManager
    {
        private string Command { get; set; }
        private string Prefix { get; set; }
        private List<CategoryCollection> _commandCategories;
        private List<ProbabilityScoreIndex> _commandProbabilityScoreIndices;

        public CommandManager(string command, string prefix, List<CategoryCollection> commandCategories,
                              List<ProbabilityScoreIndex> commandProbabilityScoreIndices)
        {
            Command = command;
            Prefix = prefix;
            _commandCategories = commandCategories;
            _commandProbabilityScoreIndices = commandProbabilityScoreIndices;
        }

        public List<CommandType> GetCommand()
        {
            var probableCommandTypes = new List<CommandType>();
            try
            {
                _commandCategories = FileManager.GetCategoryInstances(FileManager.GetExplicitFileNamesByPrefix(FileManager.GetTestFiles(), Prefix).ToArray(), Prefix);
                DataManager.PopulateDataToCategories(_commandCategories, FileManager.GetTestFiles(), Prefix);
                new NaiveCommandCategorization(_commandCategories).CalculateProbabilityOfSegments(Command.Split(' ').ToList(), out _commandProbabilityScoreIndices);
                var highestProbabilityCategories = new NaiveCommandCategorization().GetHighestProbabilityScoreIndeces(_commandProbabilityScoreIndices);
                if (highestProbabilityCategories != null)
                {
                    if (highestProbabilityCategories.Count != 1)
                    {
                        throw new Exception("Command Identification Failed From the Final Level. There are " +
                                            highestProbabilityCategories.Count + " probable categories which are " +
                                            DataManager.GetHighestProbableCommandTypesForException<CommandType>(
                                                highestProbabilityCategories));
                    }
                    probableCommandTypes.AddRange(highestProbabilityCategories.Select(highestProbabilityCategory => Conversions.ConvertIntegerToEnum<CommandType>(highestProbabilityCategory.ReferenceId)));
                }
                return probableCommandTypes;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
