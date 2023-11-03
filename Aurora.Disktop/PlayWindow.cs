using Aurora.Disktop.Graphics;
using Aurora.Disktop.Services;
using Aurora.Disktop.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.IMEHelper;
using SpriteFontPlus;

namespace Aurora.Disktop
{
    public abstract class PlayWindow : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }
        public SdlIMEHandler ImeHandler { get; private set; }
        public GraphicContext GraphicContext { get; private set; }
        public PlayScene? Scene { get; private set; }
        public SimpleFpsCounter FpsCounter { get; private set; }
        // resources
        public DynamicSpriteFont Font { get; set; }


        public PlayWindow()
        {
            this.Graphics = new GraphicsDeviceManager(this);
            this.ImeHandler = new SdlIMEHandler(this, false);
            this.FpsCounter = new SimpleFpsCounter();
            // 
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = false;
            this.Graphics.IsFullScreen = false;
            this.Graphics.PreferredBackBufferWidth = 1440;
            this.Graphics.PreferredBackBufferHeight = 900;
            this.Graphics.SynchronizeWithVerticalRetrace = true;

            //AuroraState.Services.AddService(this.Cursor);
            //this.IsFixedTimeStep = true;//false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); //60);
            //AuroraState.Services = this.Services;
            AuroraState.Services.AddService(this);
            AuroraState.Services.AddService(this.Window);
            AuroraState.Services.AddService(this.Graphics);
            var cursor = new CursorComponent();
            this.Components.Add(cursor);
 
            AuroraState.Services.AddService<ICursorService>(cursor);
            Window.KeyDown += Window_KeyDown;
            Window.KeyUp += Window_KeyUp;
            Window.TextInput += Window_TextInput;
        }

        private void Window_TextInput(object? sender, Microsoft.Xna.Framework.TextInputEventArgs e)
        {
            if (this.Scene is KeyboardEvent @event)
            {
                @event.OnTextInput(e);
            }
        }

        private void Window_KeyUp(object? sender, InputKeyEventArgs e)
        {
            if (this.Scene is KeyboardEvent @event)
            {
                @event.OnKeyDown(e);
            }
        }

        private void Window_KeyDown(object? sender, InputKeyEventArgs e)
        {
            if (this.Scene is KeyboardEvent @event)
            {
                @event.OnKeyUp(e);
            }
        }




        public abstract void OnAfterInitialize();



        protected sealed override void Initialize()
        {
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
            scene = (T?)Activator.CreateInstance(typeof(T), new Object[] { this });
            if (scene != null)
            {
                await scene.Initialize();
                this.Scene = scene;
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

            if (Keyboard.GetState().IsKeyDown(Keys.RightShift) && Keyboard.GetState().IsKeyDown(Keys.F12))
            {
                this.Graphics.IsFullScreen = !this.Graphics.IsFullScreen;
                this.Graphics.PreferredBackBufferWidth = 1440;
                this.Graphics.PreferredBackBufferHeight = 900;
                this.Graphics.ApplyChanges();
                Thread.Sleep(100);
            }
            this.Scene?.Update(gameTime);
            this.FpsCounter?.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);
            this.GraphicContext.Begin();
            this.Scene?.Draw(gameTime);
            base.Draw(gameTime);
            this.GraphicContext.End();
        }


    }
}
