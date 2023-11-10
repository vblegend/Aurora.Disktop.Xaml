using Aurora.UI.Graphics;
using Aurora.UI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.IMEHelper;


namespace Aurora.UI
{
    public abstract class PlayWindow : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }
        public SdlIMEHandler ImeHandler { get; private set; }
        public GraphicContext GraphicContext { get; private set; }
        public PlayScene Scene { get; private set; }
        public SimpleFpsCounter FpsCounter { get; private set; }
        // resources
        public String Font { get; set; }

        private MessageManager MessageManager;



        public PlayWindow()
        {
            this.Graphics = new GraphicsDeviceManager(this);
            this.FpsCounter = new SimpleFpsCounter();
            this.MessageManager = new MessageManager(this);

            // 
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = false;
            this.Graphics.IsFullScreen = false;
            this.Graphics.PreferredBackBufferWidth = 1440;
            this.Graphics.PreferredBackBufferHeight = 900;
            this.Graphics.SynchronizeWithVerticalRetrace = true;

            //AuroraState.Services.AddService(this.Cursor);
            //this.IsFixedTimeStep = false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); //60);
            //AuroraState.Services = this.Services;

            AuroraState.Services.AddService(this);
            AuroraState.Services.AddService(this.Window);
            AuroraState.Services.AddService(this.Graphics);
        }





        public abstract void OnAfterInitialize();



        protected sealed override void Initialize()
        {
            this.ImeHandler = new SdlIMEHandler(this, false);
            AuroraState.Services.AddService(this.ImeHandler);
            this.GraphicContext = new GraphicContext(this.GraphicsDevice);
            AuroraState.Services.AddService(this.GraphicsDevice);
            AuroraState.Services.AddService(this.GraphicContext);
            AuroraState.Services.AddService(this.Content);
            base.Initialize();
        }


        public async Task<T> LoadScene<T>() where T : PlayScene
        {
            var scene = this.Scene;
            if (this.Scene != null)
            {
                this.Scene = null;
                scene?.UnInitialize();
            }
            scene = (T)Activator.CreateInstance(typeof(T), new Object[] { this });
            if (scene != null)
            {
                await scene.Initialize();
                this.Scene = scene;
                this.MessageManager.SetHandler(this.Scene);
                return (T)scene;
            }
            return null;
        }



        protected override void LoadContent()
        {
            Effects.Disabled = Effects.LoadEffectFromShaders(this.GraphicsDevice, "disable");
            base.LoadContent();
            this.OnAfterInitialize();
        }




        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            this.MessageManager.Update(gameTime);
            this.Scene?.Update(gameTime);
            this.FpsCounter?.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicContext.ResetPerformance();
            GraphicsDevice.Clear(Color.Transparent);
            this.GraphicContext.Begin();
            this.Scene?.Draw(gameTime);
            base.Draw(gameTime);
            this.GraphicContext.DrawString("", 36, $"Fps：{this.FpsCounter.Fps} Called: {this.GraphicContext.CalledNumber}", new Vector2(5, 10), Color.White);
            this.GraphicContext.End();
        }


    }
}
