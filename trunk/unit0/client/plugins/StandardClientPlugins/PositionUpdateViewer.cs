using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Client.API;
using System.Windows.Forms;

namespace StandardClientPlugins
{
    public class PositionUpdateViewer : Client.API.IClientPlugin, Client.API.IClientDebugDisplay
    {
        public void InitClientPlugin()
        {
            ClientAPI.RegisterDebugDisplay("MovementProjectionError", this);
        }

        public void UnloadClientPlugin()
        {

        }

        public string ClientPluinName()
        {
            return "PositionUpdateViewer";
        }

        public void CallDebugDisplay(string EventName, object Params)
        {
            ProjectionErrorData data = Params as ProjectionErrorData;
            if (data == null)
                return;

            PostionUpdateLogForm dlog = new PostionUpdateLogForm();
            dlog.Data = data;
            dlog.ShowDialog();
        }
    }
}
