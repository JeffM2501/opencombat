using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using Game;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Game.Messages;

namespace GameInstance
{
    public class ServerScripting
    {
        public static ServerScripting Script = new ServerScripting();

        ScriptEngine Engine = null;

        protected Dictionary<string, ScriptSource> CachedScripts = new Dictionary<string, ScriptSource>();

        public string ScriptPackName = string.Empty;

        GameState State = null;
        GameServer Server = null;

        public void Init(string scriptPath, GameServer server)
        {
            // TODO, put an app domain here that doesn't give access to the disk
            Engine = Python.CreateEngine();

            foreach (FileInfo script in new DirectoryInfo(scriptPath).GetFiles("*.py"))
            {
                ScriptSource source = Engine.CreateScriptSourceFromFile(script.FullName, Encoding.ASCII, SourceCodeKind.File);
                CachedScripts.Add(Path.GetFileNameWithoutExtension(script.Name), source);
            }

            ScriptPackName = Path.GetDirectoryName(scriptPath);
            Server = server;
            State = Server.State;
        }

        protected bool ScriptExists(string name)
        {
            return CachedScripts.ContainsKey(name);
        }

        protected ScriptScope GetScope(ScriptSource source)
        {
            ScriptScope scope = Engine.CreateScope();

            scope.SetVariable("World", State.GameWorld);
            scope.SetVariable("State", State);

            source.Execute(scope);
            return scope;
        }

        protected ScriptScope GetScope(string name)
        {
            if (!CachedScripts.ContainsKey(name))
                return null;

            return GetScope(CachedScripts[name]);
        }

        public bool GetGameInfo(GameInfo info)
        {
            ScriptScope scope = GetScope("Game");
            if (scope == null || !scope.ContainsVariable("SetGameInfo"))
                return false;

            dynamic func = scope.GetVariable("SetGameInfo");
            return func(info);
        }

        public bool NewPlayer(Player player)
        {
            ScriptScope scope = GetScope("Game");
            if (scope == null || !scope.ContainsVariable("SetNewPlayerInfo"))
                return false;

            dynamic func = scope.GetVariable("SetNewPlayerInfo");
            return func(player);
        }

        public void PlayerParted(Player player)
        {
            ScriptScope scope = GetScope("Game");
            if (scope == null || !scope.ContainsVariable("PlayerParted"))
                return;

            dynamic func = scope.GetVariable("PlayerParted");
            func(player);
        }

        public Vector4 GetSpawn(Player player)
        {
            ScriptScope scope = GetScope("Game");
            if (scope == null || !scope.ContainsVariable("GetPlayerSpawn"))
                return Vector4.Zero;

            dynamic func = scope.GetVariable("GetPlayerSpawn");
            return func(player);
        }

        public void SetupRobots()
        {
            if (ScriptExists("Robots"))
            {
                ScriptScope scope = GetScope("Robots");
                if (scope == null || !scope.ContainsVariable("GetBotCount"))
                    return;

                dynamic func = scope.GetVariable("GetBotCount");
                int bots = func();

                if (!scope.ContainsVariable("NewRobot"))
                    return;

                func = scope.GetVariable("NewRobot");
                for (int i = 0; i < bots; i++)
                {
                    Player player = Server.NewPlayer(null);
                    player.UnhandledMessage +=new Player.UnhandleMessageEvent(player_UnhandledMessage);
                    func(player,i);
                    Server.AddPlayer(player);
                }
            }
        }

        void  player_UnhandledMessage(Player player, GameMessage message)
        {
             ScriptScope scope = GetScope("Robots");
                if (scope == null)
                    return;

 	        switch(message.Code)
            {
                case GameMessage.MessageCode.ChatText:
                    if (scope.ContainsVariable("HandleChat"))
                    {
                        dynamic func = scope.GetVariable("HandleChat");
                        func(player, message as ChatTextMessage, Server);
                    }
                    break;
            }
        }
    }
}
