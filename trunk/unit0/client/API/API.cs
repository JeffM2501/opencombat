using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Client.API
{
    public interface IClientPlugin
    {
        void InitClientPlugin();
        void UnloadClientPlugin();
        string ClientPluinName();
    }

    public partial class ClientAPI
    {
      
    }
}
