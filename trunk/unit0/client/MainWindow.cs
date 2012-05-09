using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using Game;
using GridWorld;
using WorldDrawing;
using Math3D;

namespace Client
{
    class MainWindow : IDisposable
    {
        public bool QuitOnExit = false;

        protected GameWindow Window = null;
        protected View GameView = null;
      
        protected InputSystem InputTracker = null;

        protected ClientGame TheGame = null;

        public MainWindow(Launcher launcher)
        {
            GameWindowFlags flags = GameWindowFlags.Default;
            if (ClientConfig.Config.FullScreen)
                flags |= GameWindowFlags.Fullscreen;
            Window = new GameWindow(ClientConfig.Config.WindowSize.Width, ClientConfig.Config.WindowSize.Height, GraphicsMode.Default, "Unit 0", flags);
            Window.VSync = VSyncMode.Adaptive;

            InputTracker = new InputSystem(Window);
            TheGame = new ClientGame(launcher,InputTracker);
            TheGame.ToggleDrawing += new EventHandler<EventArgs>(ToggleDebugDrawing_Changed);
            TheGame.AddDebugLogItem += new ClientGame.DebugValueCallback(DebugValueCallback);
            TheGame.StatusChanged += new EventHandler<EventArgs>(Game_StatusChanged);
           
            Window.UpdateFrame += new EventHandler<FrameEventArgs>(Window_UpdateFrame);
            Window.Closed += new EventHandler<EventArgs>(Window_Closed);

            GameView = new View(Window, TheGame);
            GameView.ModifyCamera += new View.ModifyCameraCB(GameView_ModifyCamera);

            // so it will hopefully bet called after the view has had it's time to load the window
            Window.Load += new EventHandler<EventArgs>(Window_Load);

            // link up the view to the game
            TheGame.ScriptsLoaded += new EventHandler<EventArgs>(GameView.ScriptsLoaded);
        }

        void Game_StatusChanged(object sender, EventArgs e)
        {
            switch(TheGame.Status)
            {
                case ServerConnection.ConnectionStatus.New:
                    GameView.SetStatus(View.ViewStatus.New, "Please Wait");
                    break;
                case ServerConnection.ConnectionStatus.Connecting:
                    GameView.SetStatus(View.ViewStatus.Connecting, "Some_SERVER");
                    break;
                case ServerConnection.ConnectionStatus.Loading:
                    GameView.SetStatus(View.ViewStatus.Loading, "Checking Resources");
                    break;

                case ServerConnection.ConnectionStatus.WaitOptions:
                case ServerConnection.ConnectionStatus.Playing:
                    GameView.SetStatus(View.ViewStatus.Playing, string.Empty);
                    if (TheGame.Status == ServerConnection.ConnectionStatus.WaitOptions)
                    {
                        // show some dialog shit!
                    }
                    break;

                case ServerConnection.ConnectionStatus.Disconnected:
                    GameView.SetStatus(View.ViewStatus.Errored, TheGame.GetLastError());
                    break;
            }
        }

        void DebugValueCallback(string name, string value)
        {
            if (GameView != null)
                GameView.AddDebugLogItem(name, value);
        }

        void ToggleDebugDrawing_Changed(object sender, EventArgs args)
        {
            if (sender == TheGame.ToggleDebugDrawing && TheGame.ToggleDebugDrawing.Down)
            {
                GridWorldRenderer.DrawDebugLines = !GridWorldRenderer.DrawDebugLines;
                Renderer.DisplayList.FlushGL();
            }
        }

        void GameView_ModifyCamera(Renderer.SimpleCamera cam)
        {
            if (TheGame.PlayerActor != null)
            {
                GameState.BoundableActor.Location loc = TheGame.PlayerActor.GetLocation();

                cam.Spin = loc.Rotation.Z + TheGame.SpinAxis.Value;
                cam.Tilt = TheGame.TiltAxis.Value;

                if (cam.Tilt > 85)
                    cam.Tilt = 85;
                if (cam.Tilt < -85)
                    cam.Tilt = -85;

                cam.ViewPosition = loc.Position;// camPos;// SetCameraZ(camPos);
                cam.ViewPosition.Z += 1;

                float offset = 2;
                Vector2 rot = VectorHelper2.FromAngle(loc.Rotation.Z + TheGame.SpinAxis.Value);
                cam.ViewPosition.X -= rot.X * offset;
                cam.ViewPosition.Y -= rot.Y * offset;
            }
            else
            {
                cam.ViewPosition.X = 0;
                cam.ViewPosition.Y = 0;
                cam.ViewPosition.Z = 18;

                cam.Spin = -TheGame.SpinAxis.Value;
                cam.Tilt = TheGame.TiltAxis.Value;
            }
           
        }

        void Window_Load(object sender, EventArgs e)
        {
            GameView.SetStatus(View.ViewStatus.New, "Please Wait");

            TheGame.Connect();

            GameView.SetStatus(View.ViewStatus.Connecting, TheGame.Host + ":" + TheGame.Port.ToString());

            GameView.LinkChat(TheGame.Connection.Chat,TheGame);
        }

        void Window_Closed(object sender, EventArgs e)
        {
            QuitOnExit = true;
            if (TheGame != null)
                TheGame.Kill();
        }

        void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            if (InputTracker != null)
                InputTracker.Update();

            TheGame.Update();

            if (TheGame.IsDone())
                Window.Exit();
        }

        public void Run()
        {
            if (Window != null)
                Window.Run();
        }

        public void Dispose()
        {
            if (Window != null)
                Window.Dispose();
        }
    }
}
