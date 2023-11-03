using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Aurora.Disktop
{
    public class Cursor
    {
        private PlayWindow window { get; set; }
        private SimpleTexture[] textures { get; set; }
        private Int32 currentIndex { get; set; }

        private Vector2 position;

        private TimeSpan lastTicks { get; set; }
        public TimeSpan Interval { get; set; }

        public Cursor(PlayWindow window)
        {
            this.window = window;
            this.currentIndex = 0;
            this.textures = new SimpleTexture[0];
            this.position = new Vector2(0, 0);
            this.Interval = new TimeSpan(0, 0, 0, 0, 50);
        }

        public void SetTextures(SimpleTexture[] textures)
        {
            this.textures = textures;
            this.currentIndex = 0;
        }



        internal void Update(GameTime gameTime)
        {
            if (window.IsMouseVisible) return;
            MouseState mouseState = Mouse.GetState(window.Window);
            this.position.X = (float)mouseState.X;
            this.position.Y = (float)mouseState.Y;
            if (gameTime.TotalGameTime.Subtract(lastTicks) > this.Interval)
            {
                this.currentIndex = ++this.currentIndex % this.textures.Length;
                lastTicks = gameTime.TotalGameTime;
            }
        }

        internal void Draw(GameTime gameTime)
        {
            if (window.IsMouseVisible) return;
            if (currentIndex >= 0 && currentIndex < this.textures.Length)
            {
                var texture = this.textures[currentIndex];
                this.window.GraphicContext.SetState();
                this.window.GraphicContext.Draw(texture, this.position, Color.White);
            }
        }
    }
}
