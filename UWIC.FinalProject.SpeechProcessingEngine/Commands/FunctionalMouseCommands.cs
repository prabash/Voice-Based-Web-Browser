using System.Collections.Generic;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine.Commands
{
    public class FunctionalMouseCommands
    {
        private readonly List<CategoryCollection> _functionalMouseCommandCategories;
        private readonly List<ProbabilityScoreIndex> _functionalMouseCommandProbabilityScoreIndices;
        private readonly string _command;

        public FunctionalMouseCommands(string command)
        {
            _command = command;
            _functionalMouseCommandCategories = new List<CategoryCollection>();
            _functionalMouseCommandProbabilityScoreIndices = new List<ProbabilityScoreIndex>();
        }

        public List<CommandType> GetCommand()
        {
            return new CommandManager(_command, "fnc_mouse", _functionalMouseCommandCategories, _functionalMouseCommandProbabilityScoreIndices)
                .GetCommand();
        }
    }
}
