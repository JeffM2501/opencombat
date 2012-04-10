using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics;
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

        protected ClientGame Game = null;

        public MainWindow()
        {
            Window = new GameWindow(1024, 800, GraphicsMode.Default, "Unit 0", GameWindowFlags.Default);
            Window.VSync = VSyncMode.Adaptive;

            InputTracker = new InputSystem(Window);
            Game = new ClientGame(InputTracker);
            Game.ToggleDrawing += new EventHandler<EventArgs>(ToggleDebugDrawing_Changed);
            Game.AddDebugLogItem += new ClientGame.DebugValueCallback(DebugValueCallback);

            Game.StatusChanged += new EventHandler<EventArgs>(Game_StatusChanged);
           
            Window.UpdateFrame += new EventHandler<FrameEventArgs>(Window_UpdateFrame);
            Window.Closed += new EventHandler<EventArgs>(Window_Closed);

            GameView = new View(Window, Game.State);
            GameView.ModifyCamera += new View.ModifyCameraCB(GameView_ModifyCamera);

            // so it will hopefully bet called after the view has had it's time to load the window
            Window.Load += new EventHandler<EventArgs>(Window_Load);
        }

        void Game_StatusChanged(object sender, EventArgs e)
        {
            switch(Game.Status)
            {
                case ServerConnection.ConnectionStatus.New:
                    GameView.Status = View.ViewStatus.New;
                    break;
                case ServerConnection.ConnectionStatus.Connecting:
                    GameView.Status = View.ViewStatus.Connecting;
                    break;
                case ServerConnection.ConnectionStatus.Loading:
                    GameView.Status = View.ViewStatus.Loading;
                    break;

                case ServerConnection.ConnectionStatus.WaitOptions:
                case ServerConnection.ConnectionStatus.Playing:
                    GameView.Status = View.ViewStatus.Playing;
                    if (Game.Status == ServerConnection.ConnectionStatus.WaitOptions)
                    {
                        // show some dialog shit!
                    }
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
            if (sender == Game.ToggleDebugDrawing && Game.ToggleDebugDrawing.Down)
            {
                GridWorldRenderer.DrawDebugLines = !GridWorldRenderer.DrawDebugLines;
                Renderer.DisplayList.FlushGL();
            }
        }

        void GameView_ModifyCamera(Renderer.SimpleCamera cam)
        {
            GameState.BoundableActor.Location loc = Game.PlayerActor.GetLocation();

            cam.Spin = loc.Rotation.Z;
            cam.Tilt = Game.TiltAxis.Value;

            if (cam.Tilt > 85)
                cam.Tilt = 85;
            if (cam.Tilt < -85)
                cam.Tilt = -85;

            cam.ViewPosition = loc.Position;// camPos;// SetCameraZ(camPos);
            cam.ViewPosition.Z += 1;

            float offset = 2;
            Vector2 rot = VectorHelper2.FromAngle(cam.Spin);
            cam.ViewPosition.X -= rot.X * offset;
            cam.ViewPosition.Y -= rot.Y * offset;

        }

        void Window_Load(object sender, EventArgs e)
        {
            Game.Load();
        }

        void Window_Closed(object sender, EventArgs e)
        {
            QuitOnExit = true;
        }

        void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            if (InputTracker != null)
                InputTracker.UpdateAxes();

            Game.Update();

            if (Game.IsDone())
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
