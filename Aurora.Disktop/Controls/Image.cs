﻿using Aurora.Disktop.Common;
using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;


namespace Aurora.Disktop.Controls
{
    public class Image : Control
    {
        public Image()
        {
            this.FillMode = FillMode.None;
        }


        protected override void OnRender(GameTime gameTime)
        {
            base.OnRender(gameTime);

            if (this.texture != null)
            {
                if (this.FillMode == FillMode.None)
                {
                    this.Renderer.Draw(this.texture, this.GlobalBounds.Location.ToVector2(), Color.White);
                }
                else if (this.FillMode == FillMode.Stretch)
                {
                    this.Renderer.Draw(this.texture, this.GlobalBounds, Color.White);
                }
                else if (this.FillMode == FillMode.Center)
                {
                    var local = this.GlobalBounds.Location;
                    this.Renderer.Draw(this.texture, new Vector2(local.X + (this.Width - this.texture.Width) / 2, local.Y + (this.Height - this.texture.Height) / 2), Color.White);
                }
                else if (this.FillMode == FillMode.Tile)
                {
                    this.Renderer.DrawTitle(this.texture, this.GlobalBounds, Color.White);
                }
            }
        }


        public SimpleTexture Texture
        {
            get
            {
                return this.texture;
            }
            set
            {
                this.texture = value;
                if (this.Size.Equals(Point.Zero) && this.texture != null)
                {
                    this.Size = new Point(this.texture.Width, this.texture.Height);
                }
            }

        }





        // Declare the event.
        public event XamlClickEventHandler<Button> Click;

        private SimpleTexture texture;

        public FillMode FillMode { get; set; }
    }
}
