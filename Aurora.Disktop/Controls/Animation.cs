using Aurora.Disktop.Common;
using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;


namespace Aurora.Disktop.Controls
{
    public class Animation : Control
    {
        private TimeSpan lastTicks;
        public TimeSpan Interval;

        public Int32 currentIndex;


        public Animation()
        {
            this.textures = new SimpleTexture[0];
            this.Interval = new TimeSpan(0,0,1);
            this.FillMode = FillMode.None;
            this.IgnoreMouseEvents = true;
            this.currentIndex = 0;
        }


        protected override void OnUpdate(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Subtract(lastTicks) > this.Interval)
            {
                this.currentIndex = ++this.currentIndex % this.textures.Length;
                lastTicks = gameTime.TotalGameTime;
            }
        }


        protected override void OnRender(GameTime gameTime)
        {
            base.OnRender(gameTime);
            if (this.textures != null)
            {
                var state = this.Renderer.PushState(blendState: Microsoft.Xna.Framework.Graphics.BlendState.Additive);
                var texture = this.textures[this.currentIndex];
                if (this.FillMode == FillMode.None)
                {
                    this.Renderer.Draw(texture, this.GlobalBounds.Location.ToVector2(), Color.White);
                }
                else if (this.FillMode == FillMode.Stretch)
                {
                    this.Renderer.Draw(texture, this.GlobalBounds, Color.White);
                }
                else if (this.FillMode == FillMode.Center)
                {
                    var local = this.GlobalBounds.Location;
                    this.Renderer.Draw(texture, new Vector2(local.X + (this.Width - texture.Width) / 2, local.Y + (this.Height - texture.Height) / 2), Color.White);
                }
                else if (this.FillMode == FillMode.Tile)
                {
                    this.Renderer.DrawTitle(texture, this.GlobalBounds, Color.White);
                }
                if (state) this.Renderer.PopState();
            }
        }



        public SimpleTexture[] Textures
        {
            get
            {
                return this.textures;
            }
            set
            {
                this.textures = value;
                if (this.Size.Equals(Point.Zero) && this.textures != null)
                {
                    var maxWidth = this.textures.Max(x => x.Width);
                    var maxHeight = this.textures.Max(y => y.Height);
                    this.Size = new Point(maxWidth, maxHeight);
                }
            }

        }





        // Declare the event.
        public event XamlClickEventHandler<Button> Click;

        private SimpleTexture[] textures;

        public FillMode FillMode { get; set; }
    }
}
