using Aurora.Disktop.Common;
using Microsoft.Xna.Framework;
using System;


namespace Aurora.Disktop.Controls
{

    public class ContentControl : Control, ILayoutUpdatable, IRenderable, IQuery
    {
        public ContentControl()
        {
            this.TextColor=  Color.White;   
            this.HorizontalContentAlignment = HorizontalAlignment.Center;
            this.VerticalContentAlignment = VerticalAlignment.Center;
        }

        protected virtual void OnRender(GameTime gameTime)
        {
            if (this.Background != null)
            {
                this.Background.Draw(Renderer, this.GlobalBounds, Color.White);
            }
        }


        void IRenderable.ProcessUpdate(GameTime gameTime)
        {
            this.OnUpdate(gameTime);
            if (this.content is Control control  && control.Visible)
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
            this.OnRender(gameTime);
            if (this.content is Control control && control.Visible)
            {
                if (this.content is IRenderable renderable)
                {
                    renderable.ProcessRender(gameTime);
                }
            }
            else if(this.content != null)
            {
                DrawContentString();
            }
            this.Renderer.DrawRectangle(GlobalBounds, Color.Red, 1);
        }

        protected virtual void DrawContentString()
        {
            var content = this.content.ToString();
            var size = this.Renderer.MeasureString(this.Font, content);
            var offset = (this.GlobalBounds.Size.ToVector2() - size) / 2;
            var local = this.GlobalLocation.ToVector2() + offset;

            if (this.HorizontalContentAlignment == HorizontalAlignment.Left)
            {
                local.X = this.GlobalLocation.X;
            } else if (this.HorizontalContentAlignment == HorizontalAlignment.Right)
            {
                local.X = this.GlobalBounds.Right - size.X;
            }

            if (this.VerticalContentAlignment ==  VerticalAlignment.Top)
            {
                local.Y = this.GlobalLocation.Y;
            }
            else if (this.VerticalContentAlignment == VerticalAlignment.Bottom)
            {
                local.Y = this.GlobalBounds.Bottom - size.Y;
            }


            this.Renderer.DrawString(this.Font, content, local, this.TextColor);
        }




        void ILayoutUpdatable.LayoutUpdate(Boolean updateChildren)
        {
            if (this.Parent != null)
            {
                this.globalBounds.Location = this.Parent.GlobalLocation.Add(this.Location);
                this.extendBounds = this.globalBounds;
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
                return  padding;
            }
            set
            {
                padding =value;

            }
        }




        private Thickness padding;




        #region Content

        public Color TextColor { get; set; }


        public HorizontalAlignment HorizontalContentAlignment { get; set; }

        public VerticalAlignment VerticalContentAlignment { get; set; }


        public Object? Content
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
                }
                if (value is ILayoutUpdatable updatable)
                {
                    updatable.LayoutUpdate(true);
                }
            }
        }
        public Object? content;
        #endregion
    }
}
