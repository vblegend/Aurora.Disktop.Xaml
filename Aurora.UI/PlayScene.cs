using Aurora.UI.Common;
using Aurora.UI.Components;
using Aurora.UI.Controls;

using Aurora.UI.Xaml;
using Microsoft.Xna.Framework;


namespace Aurora.UI
{







    public abstract class PlayScene : Panel
    {
        /// <summary>
        /// 获取场景的事件管理器
        /// </summary>
        public MessageHandler EventManager { get; private set; }

        public TimeSpan CurrentTimeSpan { get; private set; }

        public PlayScene(PlayWindow Window)
        {
            this.Root = this;
            this.Name = "ROOT";
            this.Window = Window;
            this.Cursor = new CursorComponent(Window);
            this.EventManager = new MessageHandler(this);
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
            this.CurrentTimeSpan = gameTime.TotalGameTime;
            this.OnUpdate(gameTime);
            this.Cursor.Update(gameTime);
            (this as IRenderable).ProcessUpdate(gameTime);
        }

        public void LoadXamlFromFile(String fileName)
        {
            var parser = new XamlUIParser(this, this);
            var xml = File.ReadAllText(fileName);
            parser.Parse(xml);
        }

        public Boolean Debuging;
        public String DefaultFont { get; set; }

        public CursorComponent Cursor { get; private set; }
    }
}
