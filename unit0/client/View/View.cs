using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using GridWorld;
using Game;
using Renderer;

using WorldDrawing;

namespace Client
{
    public partial class View
    {
        public GameWindow Window = null;
        protected GameState State = null;

        protected GridWorldRenderer renderer = null;
        protected SimpleCamera camera = null;
        protected FrustumCamera fCamera;

        public bool DrawDebugInfo = false;

        public delegate void ModifyCameraCB (SimpleCamera cam );
        public event ModifyCameraCB ModifyCamera;

        public Dictionary<string, string> DebugLogLines = new Dictionary<string, string>();

        protected HUD HudProcessor = null;

        protected ClientGame TheGame = null;

        public enum ViewStatus
        {
            New,
            Connecting,
            Loading,
            Playing,
            Errored,
        }

        protected ViewStatus _Status = ViewStatus.New;
        public ViewStatus Status { get { return _Status; } }

        public void SetStatus(ViewStatus status, string message)
        {
            _Status = status;
            HudProcessor.StatusChange(_Status);
            HudProcessor.StateMessage = message;
        }

        public View(GameWindow window, ClientGame game)
        {
            TheGame = game;
            Window = window;
            State = game.State;
            State.ActorCreated += new GameState.ActorEvent(state_ActorCreated);
            State.ActorDeleted += new GameState.ActorEvent(state_ActorDeleted);

            State.MapLoaded += new EventHandler<EventArgs>(state_MapLoaded);
            Window.RenderFrame += new EventHandler<FrameEventArgs>(Window_RenderFrame);
            Window.Resize += new EventHandler<EventArgs>(Window_Resize);
            Window.Unload += new EventHandler<EventArgs>(Window_Unload);
            Window.Load += new EventHandler<EventArgs>(Window_Load);
            Window.VisibleChanged += new EventHandler<EventArgs>(Window_VisibleChanged);

            HudProcessor = new HUD(this, game);

            PlayerRenderer.GetPlayerModel = new PlayerRenderer.GetPlayerModelCB(GetPlayerModel);
            PlayerRenderer.GetPlayerTexture = new PlayerRenderer.GraphicInfoCB(GetPlayerTexture);
        }

        protected PlayerModelDescriptor GetPlayerModel(UInt64 GUID)
        {
            return TheGame.PlayerModels[TheGame.MyModelID];
        }

        protected int GetPlayerTexture(UInt64 GUID)
        {
            if (TheGame.PlayerModels[TheGame.MyModelID].TeamSkin.Count == 1)
                return 0;

            int team = new Random().Next(TheGame.PlayerModels[TheGame.MyModelID].TeamSkin.Count-1);
            if (TheGame.MyTeamID >= 0 && TheGame.MyTeamID < TheGame.PlayerModels[TheGame.MyModelID].TeamSkin.Count)
                team = TheGame.MyTeamID;

            return team;
        }

        public void LinkChat(ChatProcessor chat, ClientGame game)
        {
            HudProcessor.LinkChat(chat);
            game.SystemMessage += new EventHandler<Client.StringEventArgs>(HudProcessor.SystemMessageSent);
        }

        void state_ActorDeleted(GameState sender, GameState.Actor actor)
        {
            if (actor.RenderTag as ActorRenderer != null)
                (actor.RenderTag as ActorRenderer).Dispose();
        }

        void state_ActorCreated(GameState sender, GameState.Actor actor)
        {
            if (actor.GetType().IsSubclassOf(typeof(PlayerActor)))
                actor.RenderTag = new PlayerRenderer(actor as PlayerActor);
            else if (actor.GetType().IsSubclassOf(typeof(GameState.BoundableActor)))
                actor.RenderTag = new ActorRenderer(actor as GameState.BoundableActor);
        }

        void state_MapLoaded(object sender, EventArgs e)
        {
            renderer = new GridWorldRenderer(State.GameWorld);
            renderer.StaticInit();
        }

        public void ScriptsLoaded(object sender, EventArgs args)
        {
            HudProcessor.ScriptsLoaded();
        }

        void Window_VisibleChanged(object sender, EventArgs e)
        {
			if (Window.Context == null)
				return;
			
            Window.MakeCurrent();
            if (!Window.Visible)
            {
                // flush the textures and lists
            }
        }

        void Window_Load(object sender, EventArgs e)
        {
            Window.MakeCurrent();
           
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Lighting);


            // setup light 0
            Vector4 lightInfo = new Vector4(0.25f, 0.25f, 0.25f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Ambient, lightInfo);

            lightInfo = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Diffuse, lightInfo);
            GL.Light(LightName.Light0, LightParameter.Specular, lightInfo);

            HudProcessor.Init();
            HudProcessor.StatusChange(Status);

            camera = new SimpleCamera();

            camera.PerspectiveNear = 0.001f;

            camera.OrthoFar = 250.0f;

            camera.ViewPosition = new Vector3(-5, -5, 9);
            camera.Spin = -45;
            camera.Tilt = -10;

            fCamera = new FrustumCamera();
        }

        void Window_Unload(object sender, EventArgs e)
        {
            Window.MakeCurrent();
            // unload the textures and lists and flush any graphics data
        }

        void Window_Resize(object sender, EventArgs e)
        {
            Window.MakeCurrent();
            GL.Viewport(0, 0, Window.Width, Window.Height);
            camera.Resize(Window.Width, Window.Height);
            fCamera.Resize(Window.Width, Window.Height);
            if (HudProcessor != null)
                HudProcessor.Resize();
        }

        void DrawActors()
        {
            foreach (GameState.BoundableActor actor in State.ActorsInFrustum(fCamera.ViewFrustum))
            {
                if (actor.RenderTag != null)
                    (actor.RenderTag as ActorRenderer).Draw();
            }
        }

        void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            HudProcessor.Update(Window);

            Window.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PushMatrix();

            if (camera != null)
            {
                if (Status == ViewStatus.Playing)
                    DrawPlayField();
                HudProcessor.Draw(fCamera);            
            }

            GL.PopMatrix();
            Window.SwapBuffers();
        }

        protected void DrawPlayField()
        {
            if (camera == null)
                return;

            if (ModifyCamera != null)
                ModifyCamera(camera);

            fCamera.set(camera.ViewPosition, (float)-camera.Tilt, (float)camera.Spin);

            fCamera.SetPersective();
            fCamera.Execute();

            GL.Enable(EnableCap.Texture2D);

            if (renderer != null)
                renderer.DrawSky(fCamera);

            // lighting
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            Vector4 lightInfo = new Vector4(State.GameWorld.Info.SunPosition.X, State.GameWorld.Info.SunPosition.Y, State.GameWorld.Info.SunPosition.Z, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Position, lightInfo);

            // draw world
            if (renderer != null)
            {
                renderer.DrawVisible(fCamera.ViewFrustum, DrawDebugInfo);//fCamera.SnapshotFrusum());

                DrawActors();
            }

            DrawDebugCrap();

            GL.PushMatrix();

            GL.Clear(ClearBufferMask.DepthBufferBit);
            DrawDebugPerspectiveGUI();

            fCamera.SetOrthographic();
            GL.LoadIdentity();

            DrawDebugOrthoGUI();
            // draw gui
            GL.PopMatrix();
        }
    }
}
