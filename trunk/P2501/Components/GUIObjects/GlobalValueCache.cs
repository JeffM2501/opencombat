using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUIObjects
{
    public class GlobalValue
    {
        public string Name = string.Empty;
        public string Value = string.Empty;
        public object Tag = null;

        public static GlobalValue Empty = new GlobalValue();
    }

    public class GlobalValueCache
    {
        public static Dictionary<string, GlobalValue> Values = new Dictionary<string, GlobalValue>();

        public static GlobalValue FindValue ( string name )
        {
            if (Values.ContainsKey(name))
                return Values[name];
            return null;
        }
    }
}
