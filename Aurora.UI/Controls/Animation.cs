using Aurora.UI.Common;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;



namespace Aurora.UI.Controls
{
    public class Animation : Control
    {
        private TimeSpan lastTicks;
        public TimeSpan Interval;

        public Int32 currentIndex;


        public Animation()
        {
            this.textures = new ITexture[0];
            this.Interval = new TimeSpan(0, 0, 1);
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
            }
            
        }

        protected override void CalcAutoSize()
        {
            if (this.NeedCalcAutoHeight && this.textures != null && this.textures.Length > 0)
            {
                this.globalBounds.Height = this.textures[0].Height;
            }
            if (this.NeedCalcAutoWidth && this.textures != null && this.textures.Length > 0)
            {
                this.globalBounds.Width = this.textures[0].Width;
            }
        }

        public ITexture[] Textures
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


        protected override void OnMouseUp(IMouseMessage args)
        {
            if (args.Button == MouseButtons.Left)
            {
                if (this.GlobalBounds.Contains(args.Location) && this.Enabled)
                {
                    this.Click?.Invoke(this);
                }

            }
        }


        // Declare the event.
        public event XamlClickEventHandler<Animation> Click;

        private ITexture[] textures;

        public FillMode FillMode { get; set; }
    }
}
