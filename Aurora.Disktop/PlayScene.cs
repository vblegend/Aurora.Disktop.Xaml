using Aurora.Disktop.Components;
using Aurora.Disktop.Controls;

using Aurora.Disktop.Xaml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus;


namespace Aurora.Disktop
{

    public interface KeyboardEvent
    {

        void OnKeyUp(InputKeyEventArgs e);

        void OnTextInput(TextInputEventArgs e);

        void OnKeyDown(InputKeyEventArgs e);

    }







    public abstract class PlayScene : Panel, KeyboardEvent
    {

        private EventProcManager eventManager;

        public PlayScene(PlayWindow Window)
        {
            this.Root = this;
            this.Name = "ROOT";
            this.Window = Window;
            this.Cursor = new CursorComponent(Window);
            this.eventManager = new EventProcManager(this);
            this.Window.Window.ClientSizeChanged += Window_ClientSizeChanged;
            this.Size = new Point(this.Window.Graphics.PreferredBackBufferWidth, this.Window.Graphics.PreferredBackBufferHeight);
        }



        public PlayWindow Window { get; private set; }

        protected abstract Task OnInitialize();
        protected abstract Task OnUnInitialize();


        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {

        }

        internal async Task Initialize()
        {
            await this.OnInitialize();
        }

        internal async Task UnInitialize()
        {
            await this.OnUnInitialize();
        }

        internal void Draw(GameTime gameTime)
        {
            (this as IRenderable).ProcessRender(gameTime);
            this.Cursor.Draw(gameTime);
        }


        internal void Update(GameTime gameTime)
        {
            this.OnUpdate(gameTime);
            this.Cursor.Update(gameTime);
            var currentMouseState = Mouse.GetState(this.Window.Window);

            if (currentMouseState.X > 0 && currentMouseState.Y > 0)
            {
                if (this.Window.IsActive) this.eventManager.Update(gameTime, currentMouseState);
            }

            (this as IRenderable).ProcessUpdate(gameTime);
        }





        public void LoadXamlFromFile(String fileName)
        {
            var parser = new XamlUIParser(this, this);
            var xml = File.ReadAllText(fileName);
            parser.Parse(xml);
        }


        void KeyboardEvent.OnKeyUp(InputKeyEventArgs e)
        {

        }

        void KeyboardEvent.OnKeyDown(InputKeyEventArgs e)
        {

        }

        void KeyboardEvent.OnTextInput(TextInputEventArgs e)
        {

        }


        public Boolean Debuging;
        public String DefaultFont { get; set; }

        public CursorComponent Cursor { get; private set; }
    }
}
