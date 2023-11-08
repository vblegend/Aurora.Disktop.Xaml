using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Aurora.Disktop.Services
{

    public enum Cursors
    {
        /// <summary>
        /// 指针
        /// </summary>
        Pointer = 0,
        /// <summary>
        /// 手（超链接）
        /// </summary>
        Hand = 1,
        /// <summary>
        /// 十字
        /// </summary>
        Corss = 2,
        /// <summary>
        /// 帮助
        /// </summary>
        Help = 3,
        /// <summary>
        /// 文本
        /// </summary>
        Text = 4,
    }

    public enum CursorSource
    {
        System = 0,
        Custom = 1,
    }


    public interface ICursorService
    {
        void SetTextures(SimpleTexture[] textures);
        TimeSpan Interval { get; set; }

        CursorSource Source { get; set; }

    }



    public class CursorComponent : IGameComponent, IUpdateable, IDisposable, IDrawable, ICursorService
    {
        public CursorComponent()
        {
            this.currentIndex = 0;
            this.textures = new SimpleTexture[0];
            this.position = new Vector2(0, 0);
            this.Interval = new TimeSpan(0, 0, 0, 0, 50);

            this.window = AuroraState.Services.GetService<PlayWindow>();

        }
        public bool Enabled => window != null && !window.IsMouseVisible;
        public int UpdateOrder { get; set; }
        public int DrawOrder { get; set; }
        public bool Visible => window != null && !window.IsMouseVisible;


        private PlayWindow window { get; set; }
        private SimpleTexture[] textures { get; set; }
        private Int32 currentIndex { get; set; }

        private Vector2 position;
        private TimeSpan lastTicks { get; set; }
        public TimeSpan Interval { get; set; }



        public CursorSource Source {

            get
            {
                return window.IsMouseVisible ? CursorSource.System : CursorSource.Custom;
            }
            set
            {
                if (value == CursorSource.System)
                {
                    window.IsMouseVisible = true;
                }
                else
                {
                    window.IsMouseVisible = false;
                }
                EnabledChanged?.Invoke(this, EventArgs.Empty);
                VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public void Initialize()
        {

        }

        public void SetTextures(SimpleTexture[] textures)
        {
            this.textures = textures;
            this.currentIndex = 0;
        }



        public void Update(GameTime gameTime)
        {
            if (window.IsMouseVisible || this.textures.Length == 0) return;
            MouseState mouseState = Mouse.GetState(window.Window);
            this.position.X = (float)mouseState.X;
            this.position.Y = (float)mouseState.Y;
            if (gameTime.TotalGameTime.Subtract(lastTicks) > this.Interval)
            {
                this.currentIndex = ++this.currentIndex % this.textures.Length;
                lastTicks = gameTime.TotalGameTime;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (window.IsMouseVisible || this.textures.Length == 0) return;
            if (currentIndex >= 0 && currentIndex < this.textures.Length)
            {
                var texture = this.textures[currentIndex];
                this.window.GraphicContext.Draw(texture, this.position, Color.White);
            }
        }


        public void Dispose()
        {
            this.window = null;
            this.textures = null;
        }



    }
}
