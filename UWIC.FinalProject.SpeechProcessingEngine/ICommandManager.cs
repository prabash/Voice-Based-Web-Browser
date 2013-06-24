using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    public interface ICommandManager
    {
        CommandType GetCommandType(List<string> commandSegments);
    }
}