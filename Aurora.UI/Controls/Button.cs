using Aurora.UI.Common;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Aurora.UI.Controls
{




    public class Button : ContentControl
    {
        public Button()
        {
            this.SpriteIndex = ButtonIndexs.Default;
        }

        protected override void OnMouseDown(IMouseMessage args)
        {
            if (args.Button == MouseButtons.Left)
            {
                this.SpriteIndex = ButtonIndexs.Pressed;
            }
        }

        protected override void OnMouseUp(IMouseMessage args)
        {
            if (args.Button == MouseButtons.Left)
            {
                this.SpriteIndex = this.IsHover ? ButtonIndexs.Hover : ButtonIndexs.Default;
                if (this.GlobalBounds.Contains(args.Location) && this.Enabled)
                {
                    this.Click?.Invoke(this);
                }
            }
        }


        protected override void OnMouseEnter()
        {
            if (this.SpriteIndex == ButtonIndexs.Default)
            {
                this.SpriteIndex = ButtonIndexs.Hover;
            }
            base.OnMouseEnter();
        }

        protected override void OnMouseLeave()
        {
            if (this.SpriteIndex == ButtonIndexs.Hover)
            {
                this.SpriteIndex = ButtonIndexs.Default;
            }
            base.OnMouseLeave();
        }


        protected override void OnRender(GameTime gameTime)
        {
            var dest = new Rectangle(this.GlobalBounds.Left + this.Padding.Left, this.GlobalBounds.Top + this.Padding.Top, this.GlobalBounds.Width - this.Padding.Left - this.Padding.Right, this.GlobalBounds.Height - this.Padding.Top - this.Padding.Bottom);
            this.RenderButton(dest);
            var offset = new Vector2((this.Enabled && this.SpriteIndex == ButtonIndexs.Pressed) ? 1 : 0);
            this.OnDrawContent(gameTime, offset);
        }


        protected void RenderButton(Rectangle rectangle)
        {
            if (!this.Enabled) this.SpriteIndex = ButtonIndexs.Disabled;

            // 渲染背景，如果有
            if (this.Background != null)
            {
                this.Background.Draw(Renderer, rectangle, Color.White);
            }
            // 渲染按钮状态
            if (this.sprite != null)
            {
                var sourceRect = sprite.GetFrameRectangle((Int32)this.SpriteIndex);
                this.Renderer.Draw(sprite.Texture, rectangle, sourceRect, Color.White);
            }
        }




        protected override void OnKeyDown(IKeyboardMessage args)
        {
            if (!this.Enabled) return;
            if (args.Key == Microsoft.Xna.Framework.Input.Keys.Space)
            {
                this.SpriteIndex = ButtonIndexs.Pressed;
            }
        }


        protected override void OnKeyUp(IKeyboardMessage args)
        {
            if (!this.Enabled) return;
            if (args.Key == Microsoft.Xna.Framework.Input.Keys.Space)
            {
                this.Click?.Invoke(this);
                this.SpriteIndex = ButtonIndexs.Default;
            }
        }


        /// <summary>
        /// 计算自动大小
        /// </summary>
        /// <returns></returns>
        protected override void CalcAutoSize()
        {
            if (this.NeedCalcAutoHeight && this.Image != null)
            {
                this.globalBounds.Height = Image.Height + this.Padding.Left + this.Padding.Right;
            }
            if (this.NeedCalcAutoWidth && this.Image != null)
            {
                this.globalBounds.Width = Image.Width + this.Padding.Top + this.Padding.Bottom;
            }
        }



        public SpriteObject Image
        {
            get
            {
                return this.sprite;
            }
            set
            {
                this.sprite = value;
                if (this.Size.Equals(Point.Zero) && this.sprite != null)
                {
                    this.Size = new Point(this.sprite.Width, this.sprite.Height);
                }
            }

        }





        // Declare the event.
        public virtual event XamlClickEventHandler<Button> Click;
        protected ButtonIndexs SpriteIndex;
        private SpriteObject sprite;
    }
}
