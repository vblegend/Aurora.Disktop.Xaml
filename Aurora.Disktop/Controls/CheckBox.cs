using Aurora.Disktop.Common;
using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;


namespace Aurora.Disktop.Controls
{
    public class CheckBox : Button
    {












        protected override void OnRender(GameTime gameTime)
        {
            var dest = new Rectangle(this.GlobalBounds.Left + this.Padding.Left, this.GlobalBounds.Top + this.Padding.Top, this.GlobalBounds.Height - this.Padding.Bottom, this.GlobalBounds.Height - this.Padding.Bottom);

            this.RenderButton(dest);
            if (this.Value && this.Icon != null)
            {
                var iconDest = new Rectangle(this.GlobalBounds.Location, new Point(this.GlobalBounds.Height));
                this.Renderer.Draw(this.Icon, iconDest, Color.White);
            }

        }



        protected override void DrawContentString()
        {
            var content = this.content.ToString();
            var fontSize = (this.Height - this.Padding.Top - this.Padding.Bottom) / this.Font.Size ;

            var size = this.Renderer.MeasureString(this.Font, content) * fontSize;
            var offset = (this.GlobalBounds.Size.ToVector2() - size) / 2;
            offset.X = this.Padding.Left + this.Height + 0;
            var local = this.GlobalLocation.ToVector2() + offset;
            if (fontSize < 1) local.Y++;

            if (this.Enabled && this.IsPressed) local += new Vector2(1, 1);
            this.Renderer.DrawString(this.Font, content, local, this.Enabled ? this.TextColor : Color.Gray, new Vector2(fontSize));
        }



        protected override void OnMouseUp(MouseButtons button, Point point)
        {
            if (button == MouseButtons.Left)
            {
                if (this.GlobalBounds.Contains(point) && this.Enabled)
                {
                    this.Value = !this.Value;
                }
            }
            base.OnMouseUp(button, point);
        }



        public new XamlClickEventHandler<CheckBox> Click;

        public SimpleTexture Icon;

        public Boolean Value;
    }
}
