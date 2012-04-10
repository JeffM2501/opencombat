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
using OpenTK.Graphics;

namespace Client
{
    public class ClientScripting
    {
        public static ClientScripting Script = new ClientScripting();

        ScriptEngine Engine = null;

        protected Dictionary<string, ScriptSource> CachedScripts = new Dictionary<string, ScriptSource>();

        public string ScriptPackName = string.Empty;

        ClientGame Game = null;

        public void SetState(ClientGame game)
        {
            Game = game;
        }

        public void Init(string scriptPath)
        {
            // TODO, put an app domain here that dosn't give access to the disk
            Engine = Python.CreateEngine();

            foreach (FileInfo script in new DirectoryInfo(scriptPath).GetFiles("*.py"))
            {
                ScriptSource source = Engine.CreateScriptSourceFromFile(script.FullName, Encoding.ASCII, SourceCodeKind.File);
                CachedScripts.Add(Path.GetFileNameWithoutExtension(script.Name), source);
            }

            ScriptPackName = Path.GetDirectoryName(scriptPath);
        }

        protected ScriptScope GetScope(ScriptSource source)
        {
            ScriptScope scope = Engine.CreateScope();

            scope.SetVariable("Game", Game);
            scope.SetVariable("State", Game.State);

            source.Execute(scope);
            return scope;
        }

        protected ScriptScope GetScope(string name)
        {
            if (!CachedScripts.ContainsKey(name))
                return null;

            return GetScope(CachedScripts[name]);
        }

        public bool InitGameScript( string gameType )
        {
            ScriptScope scope = GetScope("Game");
            if (scope == null)
                return false;

            dynamic func = scope.GetVariable("InitGame");
            return func(gameType);
        }
    }
}
