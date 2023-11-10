using Aurora.UI.Common;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;


namespace Aurora.UI.Controls
{
    public class CheckBox : Button
    {
        public CheckBox()
        {
            this.Size = new Point(Int32.MinValue, 20);
        }

        protected override void OnRender(GameTime gameTime)
        {
            var dest = new Rectangle(this.GlobalBounds.Left + this.Padding.Left, this.GlobalBounds.Top + this.Padding.Top, this.GlobalBounds.Height - this.Padding.Bottom, this.GlobalBounds.Height - this.Padding.Bottom);
            this.RenderButton(dest);
            if (this.Value && this.Icon != null)
            {
                var iconDest = new Rectangle(this.GlobalBounds.Location, new Point(this.GlobalBounds.Height));
                if (this.IsPressed)
                {
                    iconDest.Location += new Point(1, 1);
                }
                this.Renderer.Draw(this.Icon, iconDest, Color.White);
            }
        }



        protected override void DrawContentString()
        {
            //var content = this.content.ToString();
            //var fontSize = (this.Height - this.Padding.Top - this.Padding.Bottom) / this.FontSize;
            //var size = this.Renderer.MeasureString(this.Font, this.FontSize, content) * fontSize;
            //var offset = (this.GlobalBounds.Size.ToVector2() - size) / 2;
            //offset.X = this.Padding.Left + this.Height + 0;
            //var local = this.GlobalLocation.ToVector2() + offset;
            //if (fontSize < 1) local.Y++;
            //if (this.Enabled && this.IsPressed) local += new Vector2(1, 1);
            //this.Renderer.DrawString(this.Font, this.FontSize, content, local, this.Enabled ? this.TextColor : Color.Gray, new Vector2(fontSize));
            var content = this.content.ToString();
            var size = new Vector2(0, this.FontSize);
            var offset = (this.GlobalBounds.Size.ToVector2() - size) / 2;
            offset.X = this.Padding.Left + this.Height + (Int32)(this.FontSize * 0.2);
            var local = this.GlobalLocation.ToVector2() + offset;
            if (this.Enabled && this.IsPressed) local += new Vector2(1, 1);
            this.Renderer.DrawString(this.Font, this.FontSize, content, local, this.Enabled ? this.TextColor : Color.Gray);
        }





        /// <summary>
        /// 计算自动大小
        /// </summary>
        /// <returns></returns>
        protected override void CalcAutoSize()
        {
            if (this.NeedCalcAutoHeight)
            {
                this.globalBounds.Height = 20;
            }
            if (this.NeedCalcAutoWidth && this.Font != null)
            {
                var content = this.content.ToString();
                //var fontSize = (this.Height - this.Padding.Top - this.Padding.Bottom) / this.FontSize;
                var size = this.Renderer.MeasureString(this.Font, this.FontSize, content);
                var px = size + new Vector2(this.Padding.Left + this.Padding.Right + this.Height + (Int32)(this.FontSize * 0.2), 0);
                this.globalBounds.Width = (Int32)px.X;
            }
        }

        protected override void OnMouseUp(IMouseMessage args)
        {
            if (args.Button == MouseButtons.Left)
            {
                if (this.GlobalBounds.Contains(args.Location) && this.Enabled)
                {
                    this.Value = !this.Value;
                    this.Click?.Invoke(this);
                }
            }
            base.OnMouseUp(args);
        }


        protected override void OnKeyDown(IKeyboardMessage args)
        {
            if (!this.Enabled) return;
            if (args.Key == Microsoft.Xna.Framework.Input.Keys.Space)
            {
                this.IsPressed = true;
            }
        }


        protected override void OnKeyUp(IKeyboardMessage args)
        {
            if (!this.Enabled) return;
            if (args.Key == Microsoft.Xna.Framework.Input.Keys.Space)
            {
                this.Value = !this.Value;
                this.Click?.Invoke(this);
                this.IsPressed = false;
            }
        }



        public event XamlClickEventHandler<CheckBox> Click;

        public SimpleTexture Icon;

        public Boolean Value;
    }
}
