using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public class ProcessorEngine
    {
        private List<string> FunctionalCommands { get; set; }
        private List<string> MouseCommands { get; set; }
        private List<string> KeyboardCommands { get; set; }

        #region Main Probability Indeces
        private double FuncCommandProbability = 0;
        private double MouseCommandProbability = 0;
        private double KeyboardCommandProbability = 0;
        # endregion

        public ProcessorEngine()
        {
            FunctionalCommands = new List<string>();
            MouseCommands = new List<string>();
            KeyboardCommands = new List<string>();
            LoadTrainedSets();
        }

        public void CalculateProbabilityOfCommand(string command)
        {
            SpeechSegmentation(command);
        }

        public void SpeechSegmentation(string phrase)
        {
            var segments = phrase.Split(' ');
            foreach (var segment in segments)
            {
                CaluclateProbabilityBySegment(segment);
            }
        }

        private void LoadTrainedSets()
        {
            AssignToSet(FunctionalCommands, "Func_Commands");
            AssignToSet(FunctionalCommands, "Nav_Func");
            AssignToSet(FunctionalCommands, "Mouse_Func");
            AssignToSet(FunctionalCommands, "Browser_Func");
            AssignToSet(MouseCommands, "Mouse_Commands");
            AssignToSet(KeyboardCommands, "Key_Commands");
        }

        private static void AssignToSet(List<string> setName, string fileName)
        {
            Settings.CultureInfo = "en-GB";
            var tempList = new List<string>();
            using (var fs = File.Open("..//..//data//" + fileName + ".txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    tempList.Add(line);
                }
            }
            setName.AddRange(tempList);
        }

        private void CaluclateProbabilityBySegment(string segment)
        {
            double funcComPob = 0;
            double mouseComPob = 0;
            double keyComPob = 0;
            double probabilityOfBelongness;

            var availableInFunc = CheckAvailabilityInSet(FunctionalCommands, segment); // Check whether the segment is available in the Functional Commands list
            var availableInMouse = CheckAvailabilityInSet(MouseCommands, segment); // Check whether the segment is available in the Mouse Commands list
            var availableInKey = CheckAvailabilityInSet(KeyboardCommands, segment); // Check whether the segment is available in the Key Commands list

            var noOfAvailabilities = Convert.ToInt32(availableInFunc) + Convert.ToInt32(availableInMouse) +
                                      Convert.ToInt32(availableInKey); //Calculate the Total number of availabilites
            if (noOfAvailabilities != 0)
                probabilityOfBelongness = 1/noOfAvailabilities;
            else
                probabilityOfBelongness = 0;

            if (availableInFunc) // If the command is available in functional Commands
                funcComPob = probabilityOfBelongness; // assign the probability of belogness to Functional Command
            if (availableInMouse) // If the command is available in mouse commands
                mouseComPob = probabilityOfBelongness; // assign the probability of belogness to the Mouse Command
            if (availableInKey) // if the command is available in keyboard commands
                keyComPob = probabilityOfBelongness; // assign the probability of belogness to the Keyboard Command

            FuncCommandProbability += funcComPob; // Concat the current probability of belogness to the probability of entire command belonging to a Functional Command
            MouseCommandProbability += mouseComPob; // Concat the current probability of belogness to the probability of entire command belonging to a Mouse Command
            KeyboardCommandProbability += keyComPob; // Concat the current probability of belogness to the probability of entire command belonging to a Key Command
        }

        /// <summary>
        /// This method is used to check whether a particular object is available inside any give collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        private static bool CheckAvailabilityInSet(IEnumerable<string> collection,object segment)
        {
            return collection.Contains(segment);
        }
    }
}
