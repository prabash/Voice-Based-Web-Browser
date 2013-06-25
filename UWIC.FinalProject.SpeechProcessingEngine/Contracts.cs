using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    public class CategoryCollection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Category { get; set; }
        public List<string> List { get; set; }
    }

    public class BooleanProbability
    {
        public int ReferenceId { get; set; }
        public bool Available { get; set; }
    }

    public class ProbabilityScoreIndex
    {
        public int ReferenceId { get; set; }
        public double ProbabilityScore { get; set; }
    }
}
