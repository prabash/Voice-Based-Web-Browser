using System.Collections.Generic;
using System.Linq;

namespace UWIC.FinalProject.SpeechProcessingEngine.Managers
{
    public class DataManager
    {
        /// <summary>
        /// This method is used to assign test data to each category without redundancy
        /// </summary>
        /// <param name="set">The training set to which the data should be added</param>
        /// <param name="currentList">the current data list acquired from the local file</param>
        public static void AssignDataToTestSet(ICollection<string> set, IEnumerable<string> currentList)
        {
            if (currentList != null)
                foreach (var item in currentList.Where(item => !set.Contains(item)))
                {
                    set.Add(item);
                }
        }
    }
}
