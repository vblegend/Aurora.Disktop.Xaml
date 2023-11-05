using Aurora.Disktop.Common;
using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using System.Diagnostics;

namespace Aurora.Disktop.Controls
{

    public interface IXamlEventHandler
    {
        void MessageHandler(EventMessage msg);
    }

    public interface IRenderable
    {
        void ProcessUpdate(GameTime gameTime);
        void ProcessRender(GameTime gameTime);
    }

    public interface IPointHitTestable
    {
        Boolean HitTest(Point position);
    }


    public interface ILayoutUpdatable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentPosition">父对象的绝对坐标</param>
        void LayoutUpdate(Boolean updateChildren,Boolean force = false);
    }


    public abstract class Control : IQuery, IXamlEventHandler, IRenderable, IXamlControl, ILayoutUpdatable, IPointHitTestable
    {

        public GraphicContext Renderer { get; private set; }

        protected Control()
        {
            this.Name = string.Empty;
            this.Renderer = AuroraState.Services.GetService<GraphicContext>();
            this.Enabled = true;
            this.Visible = true;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;

        }

        /// <summary>
        /// 布局更新 更新全局位置
        /// </summary>
        void ILayoutUpdatable.LayoutUpdate(Boolean updateChildren, Boolean force)
        {
            this.CalcGlobalBounds();
        }


        protected Boolean CalcGlobalBounds()
        {
            if (this.Parent != null)
            {
                var l = 0;
                var t = 0;
                var w = 0;
                var h = 0;
                // 横向对齐方式
                if (this.HorizontalAlignment == HorizontalAlignment.Left)
                {
                    l = this.Parent.globalBounds.Left + this.margin.Left;
                    w = this.Width;
                }
                else if (this.HorizontalAlignment == HorizontalAlignment.Right)
                {
                    l = this.Parent.globalBounds.Right - this.margin.Right - this.Width;
                    w = this.Width;
                }
                else if (this.HorizontalAlignment == HorizontalAlignment.Center)
                {
                    l = this.Parent.globalBounds.Left + (this.margin.Left - this.margin.Right) + (this.Parent.globalBounds.Width - this.Width) / 2;
                    w = this.Width;
                }
                else if (this.HorizontalAlignment == HorizontalAlignment.Stretch)
                {
                    // Stretch 时 width 无效
                    l = this.Parent.globalBounds.Left + this.margin.Left;
                    w = this.Parent.globalBounds.Width - this.margin.Left - this.margin.Right;
                }

                // 纵向对齐方式
                if (this.VerticalAlignment == VerticalAlignment.Top)
                {
                    t = this.Parent.globalBounds.Top + this.margin.Top;
                    h = this.Height;
                }
                else if (this.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    t = this.Parent.globalBounds.Bottom - this.margin.Bottom - this.Height;
                    h = this.Height;
                }
                else if (this.VerticalAlignment == VerticalAlignment.Center)
                {
                    t = this.Parent.globalBounds.Top + (this.margin.Top - this.margin.Bottom) + (this.Parent.globalBounds.Height - this.Height) / 2;
                    h = this.Height;
                }
                else if (this.VerticalAlignment == VerticalAlignment.Stretch)
                {
                    // Stretch 时 width 无效
                    t = this.Parent.globalBounds.Top + this.margin.Top;
                    h = this.Parent.globalBounds.Height - this.margin.Top - this.margin.Bottom;
                }
                this.globalBounds = new Rectangle(l, t, w, h);
                this.extendBounds = this.globalBounds;
                return true;
            }
            return false;   
        }







        void IRenderable.ProcessUpdate(GameTime gameTime)
        {
            this.OnUpdate(gameTime);
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
            this.DrawDebugBounds();
        }


        protected void DrawDebugBounds()
        {
            if (!this.Root.Debuging) return;
            if (this.IsFocus)
            {
                this.Renderer.DrawRectangle(GlobalBounds, Color.Green, 1);
            }
            else
            {
                this.Renderer.DrawRectangle(GlobalBounds, Color.Red, 1);
            }
        }



        void IXamlEventHandler.MessageHandler(EventMessage msg)
        {
            if (msg.Message == WM_MESSAGE.MOUSE_ENTER)
            {
                this.IsHover = true;
                this.OnMouseEnter();
            }

            if (msg.Message == WM_MESSAGE.MOUSE_LEAVE)
            {
                this.IsHover = false;
                this.OnMouseLeave();
            }

            if (msg.Message == WM_MESSAGE.MOUSE_MOVE)
            {
                this.OnMouseMove(msg.Location);
            }

            if (msg.Message == WM_MESSAGE.MOUSE_DOWN)
            {
                if (msg.Button == MouseButtons.Left) this.IsPressed = true;
                this.OnMouseDown(msg.Button, msg.Location);
            }

            if (msg.Message == WM_MESSAGE.MOUSE_UP)
            {
                if (msg.Button == MouseButtons.Left) this.IsPressed = false;
                this.OnMouseUp(msg.Button, msg.Location);
            }

            if (msg.Message == WM_MESSAGE.MOUSE_WHEEL)
            {
                this.OnMouseWheel(msg.Location, msg.Wheel);
            }


            if (msg.Message == WM_MESSAGE.GOTFOCUS)
            {
                this.IsFocus = true;
                this.OnGotFocus();
            }

            if (msg.Message == WM_MESSAGE.LOSTFOCUS)
            {
                this.IsFocus = false;
                this.OnLostFocus();
            }

        }


        #region Virtual Function

        protected virtual void OnGotFocus()
        {

        }

        protected virtual void OnLostFocus()
        {

        }

        protected virtual void OnMouseEnter()
        {
            // 已实现
        }

        protected virtual void OnMouseLeave()
        {
            // 已实现
        }
        protected virtual void OnMouseDown(MouseButtons button, Point point)
        {
            // 已实现
        }
        protected virtual void OnMouseUp(MouseButtons button, Point point)
        {
            // 已实现
        }
        protected virtual void OnMouseMove(Point point)
        {
            // 已实现
        }
        protected virtual void OnMouseWheel(Point point, Int32 wheel)
        {
            // 已实现
            Trace.WriteLine($"Wheel {this.Name} {wheel}");
        }

        protected virtual void OnRender(GameTime gameTime)
        {
            if (this.Background != null)
            {

                this.Background.Draw(Renderer, this.GlobalBounds, Color.White);
            }
        }


        protected virtual void OnUpdate(GameTime gameTime)
        {

        }


        #endregion





        #region IQuery
        public Control this[string name]
        {
            get
            {
                return null;
            }
        }

        public T Query<T>(string path) where T : Control
        {
            var paths = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            Control control = this;
            foreach (var item in paths)
            {
                control = control[item];
                if (control == null)
                {
                    return null;
                }
            }
            return control as T;
        }

        public virtual bool HitTest(Point position)
        {
            return this.GlobalBounds.Contains(position);
        }



        #endregion

        public DynamicSpriteFont Font
        {
            get
            {
                return this._font != null ? this._font : this.Root != null ? this.Root.DefaultFont : null;
            }
            set
            {
                this._font = value;
            }
        }

        private DynamicSpriteFont _font;



        /// <summary>
        /// 父控件对象
        /// </summary>
        public Control Parent;

        /// <summary>
        /// 根对象
        /// </summary>
        public PlayScene Root;

        public IXamlBrush Background;

        /// <summary>
        /// 控件名字
        /// </summary>
        public String Name  = String.Empty;
        public Boolean Visible = true;
        public Boolean Enabled = true;
        /// <summary>
        /// 控件是否可聚焦
        /// </summary>
        public Boolean Focusable = true;
        public Boolean IgnoreMouseEvents = false;
        public Boolean IgnoreKeyboardEvents = false;






        #region Position  Layout

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public Thickness Margin
        {

            get
            {
                return this.margin;
            }
            set
            {
                margin = value;
                (this as ILayoutUpdatable).LayoutUpdate(true);
            }
        }







        private Thickness margin;



        /// <summary>
        /// 获取对象全局坐标
        /// </summary>
        public Point GlobalLocation
        {
            get
            {
                return this.globalBounds.Location;
            }
        }

        /// <summary>
        /// 获取对象全局边界框
        /// </summary>
        public Rectangle GlobalBounds
        {

            get
            {
                return this.globalBounds;
            }
        }

        /// <summary>
        /// 获取控件扩展边界框（包含子控件边界）
        /// </summary>
        public Rectangle ExtendBounds
        {

            get
            {
                return this.extendBounds;
            }
        }




        protected Rectangle globalBounds;

        protected Rectangle extendBounds;


        /// <summary>
        /// 获取控件大小
        /// </summary>
        public Point Size
        {
            get
            {
                return this.globalBounds.Size;
            }
            set
            {
                if (value.X == Int32.MinValue)
                {

                }

                this.globalBounds.Size = value;

            }
        }

        public Boolean AutoWidth { get; private set; }
        public Boolean AutoHeight { get; private set; }



        /// <summary>
        /// 获取控件宽度
        /// </summary>
        public Int32 Width
        {
            get
            {
                return this.globalBounds.Width;
            }
            set
            {
                this.globalBounds.Width = value;
            }
        }

        /// <summary>
        /// 获取控件高度
        /// </summary>
        public Int32 Height
        {
            get
            {
                return this.globalBounds.Height;
            }
            set
            {
                this.globalBounds.Height = value;
            }
        }

        #endregion


        #region State
        /// <summary>
        /// 获取鼠标是否悬停在控件上方
        /// </summary>
        public Boolean IsHover { get; private set; }

        /// <summary>
        /// 获取控件是否获得焦点
        /// </summary>
        public Boolean IsFocus { get; private set; }



        /// <summary>
        /// 获取鼠标是否在控件按下
        /// </summary>
        public Boolean IsPressed { get; private set; }

        #endregion






    }
}
