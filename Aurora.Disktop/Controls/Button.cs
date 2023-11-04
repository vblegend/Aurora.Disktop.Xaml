using Aurora.Disktop.Common;
using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Aurora.Disktop.Controls
{
    public class Button : ContentControl
    {

        private const Int32 BUTTON_SPRITE_DEFAULT_INDEX = 0;

        private const Int32 BUTTON_SPRITE_HOVER_INDEX = 1;

        private const Int32 BUTTON_SPRITE_PRESSED_INDEX = 2;

        private const Int32 BUTTON_SPRITE_DISABLED_INDEX = 0;

        public Button()
        {
            this.spriteIndex = BUTTON_SPRITE_DEFAULT_INDEX;
            Console.WriteLine(this.Renderer);
        }




        protected override void OnMouseDown(MouseButtons button, Point point)
        {
            if (button == MouseButtons.Left)
            {
                this.spriteIndex = BUTTON_SPRITE_PRESSED_INDEX;
            }

        }

        protected override void OnMouseUp(MouseButtons button, Point point)
        {
            if (button == MouseButtons.Left)
            {
                this.spriteIndex = this.IsHover ? BUTTON_SPRITE_HOVER_INDEX : BUTTON_SPRITE_DEFAULT_INDEX;
                if (this.GlobalBounds.Contains(point) && this.Enabled)
                {
                    this.Click?.Invoke(this);
                }

            }
        }


        protected override void OnMouseEnter()
        {
            if (this.spriteIndex == BUTTON_SPRITE_DEFAULT_INDEX)
            {
                this.spriteIndex = BUTTON_SPRITE_HOVER_INDEX;
            }
        }

        protected override void OnMouseLeave()
        {
            if (this.spriteIndex == BUTTON_SPRITE_HOVER_INDEX)
            {
                this.spriteIndex = BUTTON_SPRITE_DEFAULT_INDEX;
            }
        }


        protected override void OnRender(GameTime gameTime)
        {
            GraphicContext.ContextState? state = null;
            if (!this.Enabled)
            {
                this.spriteIndex = BUTTON_SPRITE_DISABLED_INDEX;
                state = this.Renderer.SetState(effect: Effects.Disabled);
            }
            // 渲染背景，如果有
            if (this.Background != null)
            {
                this.Background.Draw(Renderer, this.GlobalBounds, Color.White);
            }
            // 渲染按钮状态
            if (this.sprite != null)
            {
                var sourceRect = sprite.GetFrameRectangle(this.spriteIndex);
                this.Renderer.Draw(sprite.Texture, this.GlobalBounds, sourceRect, Color.White);
            }
            if (state.HasValue) this.Renderer.RestoreState(state.Value);

        }
        protected override void DrawContentString()
        {
            var content = this.content.ToString();
            var size = this.Renderer.MeasureString(this.Font, content);
            var offset = (this.GlobalBounds.Size.ToVector2() - size) / 2;
            var local = this.GlobalLocation.ToVector2() + offset;
            if (this.Enabled && this.IsPressed) local += new Vector2(1, 1);
            this.Renderer.DrawString(this.Font, content, local, this.Enabled ? this.TextColor : Color.Gray);
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
        public event XamlClickEventHandler<Button> Click;
        private Int32 spriteIndex;
        private SpriteObject sprite;
    }
}
