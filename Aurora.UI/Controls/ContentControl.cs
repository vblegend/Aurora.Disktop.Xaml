using Aurora.UI.Common;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Aurora.UI.Controls
{

    public abstract class ContentControl : Control, ILayoutUpdatable, IRenderable, IQuery
    {
        public ContentControl()
        {
            this.TextColor = Color.White;
            this.HorizontalContentAlignment = XamlHorizontalAlignment.Center;
            this.VerticalContentAlignment = XamlVerticalAlignment.Center;
        }

        protected override void OnRender(GameTime gameTime)
        {
            if (this.Background != null)
            {
                this.Background.Draw(Renderer, this.GlobalBounds, Color.White);
            }
        }


        void IRenderable.ProcessUpdate(GameTime gameTime)
        {
            this.OnUpdate(gameTime);
            if (this.content is Control control && control.Visible)
            {
                if (this.content is IRenderable renderable)
                {
                    renderable.ProcessUpdate(gameTime);
                }
            }
        }

        /// <summary>
        /// 控件渲染处理
        /// </summary>
        /// <param name="gameTime"></param>
        void IRenderable.ProcessRender(GameTime gameTime)
        {
            var effect = !this.Enabled ? Effects.Disabled : null;
            GraphicContext.ContextState? state = this.Renderer.SetState(effect);
            this.OnRender(gameTime);
            if (state.HasValue) this.Renderer.RestoreState(state.Value);
            if (this.content is Control control && control.Visible)
            {
                if (this.content is IRenderable renderable)
                {
                    renderable.ProcessRender(gameTime);
                }
            }
            else if (this.content != null)
            {
                DrawContentString();
            }
            this.DrawDebugBounds();
        }

        protected virtual void DrawContentString()
        {
            var content = this.content.ToString();
            var size = this.Renderer.MeasureString(this.Font, this.FontSize, content);
            var offset = (this.GlobalBounds.Size.ToVector2() - size) / 2;
            var local = this.GlobalLocation.ToVector2() + offset;

            if (this.HorizontalContentAlignment == XamlHorizontalAlignment.Left)
            {
                local.X = this.GlobalLocation.X;
            }
            else if (this.HorizontalContentAlignment == XamlHorizontalAlignment.Right)
            {
                local.X = this.GlobalBounds.Right - size.X;
            }

            if (this.VerticalContentAlignment == XamlVerticalAlignment.Top)
            {
                local.Y = this.GlobalLocation.Y;
            }
            else if (this.VerticalContentAlignment == XamlVerticalAlignment.Bottom)
            {
                local.Y = this.GlobalBounds.Bottom - size.Y;
            }


            this.Renderer.DrawString(this.Font, this.FontSize, content, local, this.TextColor);
        }


 


        void ILayoutUpdatable.LayoutUpdate(Boolean updateChildren, Boolean force)
        {
            this.CalcAutoSize();
            if (this.CalcGlobalBounds() || force)
            {
                if (updateChildren && this.content is ILayoutUpdatable updatable)
                {
                    updatable.LayoutUpdate(true);
                }
            }
        }



        public Thickness Padding
        {
            get
            {
                return padding;
            }
            set
            {
                padding = value;

            }
        }




        private Thickness padding;




        #region Content

        public Color TextColor { get; set; }


        public XamlHorizontalAlignment HorizontalContentAlignment { get; set; }

        public XamlVerticalAlignment VerticalContentAlignment { get; set; }


        public Object Content
        {
            get
            {
                return this.content;
            }
            set
            {
                this.content = value;
                if (value is Control control)
                {
                    control.Parent = this;
                    control.Root = this.Root;
                    if (control is IAttachable attachable) attachable.OnAttached();
                }
                if (value is ILayoutUpdatable updatable)
                {
                    updatable.LayoutUpdate(true);
                }
            }
        }
        public Object content;
        #endregion
    }
}
