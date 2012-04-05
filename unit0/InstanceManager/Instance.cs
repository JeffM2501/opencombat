using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using InstanceConfig;

namespace InstanceManager
{
    internal class Instance
    {
        public Configuration Config = null;
        public Process Proc = null;

        public enum Status
        {
            Unknown,
            Connected,
            Ready,
            Inactive,
            Restarting,
            Disconnected,
        }

        public Status ConnectionStatus = Status.Unknown;

        public Instance(string configParams, UInt64 InstanceID)
        {
            Config = new Configuration();
            Config.InstanceID = InstanceID;
        }
    }
}
