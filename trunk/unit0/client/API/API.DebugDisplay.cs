using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.API
{
    public interface IClientDebugDisplay
    {
        void CallDebugDisplay(string EventName, object Params);
    }

    public class ProjectionErrorData
    {
        public List<string> FinalPositionLogBeforeTruncation = null;
        public List<string> FinalPositionLogAfterTruncation = null;
        public List<string> TruncationPositionLogBeforeRemoval = null;
        public List<string> TruncationPositionLogAfterRemoval = null;

        public double Now = 0;
        public double TruncationTime = 0;
    }

    public partial class ClientAPI
    {
        public static Dictionary<string, List<IClientDebugDisplay>> DebugDisplays = new Dictionary<string, List<IClientDebugDisplay>>();

        public static void RegisterDebugDisplay(string eventName, IClientDebugDisplay displayer)
        {
            if (!DebugDisplays.ContainsKey(eventName))
                DebugDisplays.Add(eventName, new List<IClientDebugDisplay>());

            DebugDisplays[eventName].Add(displayer);
        }

        public static void CallDebugDisplay(string eventName, object paramters)
        {
            if (!DebugDisplays.ContainsKey(eventName))
                return;
            foreach (IClientDebugDisplay display in DebugDisplays[eventName])
                display.CallDebugDisplay(eventName, paramters);
        }
    }
}
