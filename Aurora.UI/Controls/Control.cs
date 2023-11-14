using Aurora.UI.Common;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;


namespace Aurora.UI.Controls
{

    public interface IXamlEventHandler
    {
        void MessageHandler(IInputMessage msg);
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
        void LayoutUpdate(Boolean updateChildren, Boolean force = false);
    }


    public interface IAttachable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentPosition">父对象的绝对坐标</param>
        void OnAttached();
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
            this.UserData = new PrivateUserData();
            this.HorizontalAlignment = XamlHorizontalAlignment.Left;
            this.VerticalAlignment = XamlVerticalAlignment.Top;

        }

        /// <summary>
        /// 布局更新 更新全局位置
        /// </summary>
        void ILayoutUpdatable.LayoutUpdate(Boolean updateChildren, Boolean force)
        {
            this.CalcAutoSize();
            this.CalcGlobalBounds();
        }


        /// <summary>
        /// 计算自动大小
        /// </summary>
        protected abstract void CalcAutoSize();



        protected virtual Boolean CalcGlobalBounds()
        {
            if (this.Parent != null)
            {
                var l = 0;
                var t = 0;
                var w = 0;
                var h = 0;
                // 横向对齐方式
                if (this.HorizontalAlignment == XamlHorizontalAlignment.Left)
                {
                    l = this.Parent.globalBounds.Left + this.margin.Left;
                    w = this.Width;
                }
                else if (this.HorizontalAlignment == XamlHorizontalAlignment.Right)
                {
                    l = this.Parent.globalBounds.Right - this.margin.Right - this.Width;
                    w = this.Width;
                }
                else if (this.HorizontalAlignment == XamlHorizontalAlignment.Center)
                {
                    l = this.Parent.globalBounds.Left + (this.margin.Left - this.margin.Right) + (this.Parent.globalBounds.Width - this.Width) / 2;
                    w = this.Width;
                }
                else if (this.HorizontalAlignment == XamlHorizontalAlignment.Stretch)
                {
                    // Stretch 时 width 无效
                    l = this.Parent.globalBounds.Left + this.margin.Left;
                    w = this.Parent.globalBounds.Width - this.margin.Left - this.margin.Right;
                }

                // 纵向对齐方式
                if (this.VerticalAlignment == XamlVerticalAlignment.Top)
                {
                    t = this.Parent.globalBounds.Top + this.margin.Top;
                    h = this.Height;
                }
                else if (this.VerticalAlignment == XamlVerticalAlignment.Bottom)
                {
                    t = this.Parent.globalBounds.Bottom - this.margin.Bottom - this.Height;
                    h = this.Height;
                }
                else if (this.VerticalAlignment == XamlVerticalAlignment.Center)
                {
                    t = this.Parent.globalBounds.Top + (this.margin.Top - this.margin.Bottom) + (this.Parent.globalBounds.Height - this.Height) / 2;
                    h = this.Height;
                }
                else if (this.VerticalAlignment == XamlVerticalAlignment.Stretch)
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



        void IXamlEventHandler.MessageHandler(IInputMessage msg)
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


            if (msg is IMouseMessage mouse)
            {
                if (msg.Message == WM_MESSAGE.MOUSE_MOVE)
                {
                    this.OnMouseMove(mouse);
                }

                if (msg.Message == WM_MESSAGE.MOUSE_DOWN)
                {
                    if (mouse.Button == MouseButtons.Left) this.IsPressed = true;
                    this.OnMouseDown(mouse);
                }

                if (msg.Message == WM_MESSAGE.MOUSE_UP)
                {
                    if (mouse.Button == MouseButtons.Left) this.IsPressed = false;
                    this.OnMouseUp(mouse);
                }

                if (msg.Message == WM_MESSAGE.MOUSE_WHEEL)
                {
                    this.OnMouseWheel(mouse);
                }
            }
            if (msg is IKeyboardMessage keyboard)
            {
                if (keyboard.Message == WM_MESSAGE.KEY_DOWN)
                {
                    this.OnKeyDown(keyboard);
                }
                else if (keyboard.Message == WM_MESSAGE.KEY_UP)
                {
                    this.OnKeyUp(keyboard);
                }
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
            // 已实现
            this.GotFocus?.Invoke(this);
        }

        protected virtual void OnLostFocus()
        {
            // 已实现
            this.LostFocus?.Invoke(this);
        }

        protected virtual void OnMouseEnter()
        {
            // 已实现
            this.MouseEnter?.Invoke(this);
        }

        protected virtual void OnMouseLeave()
        {
            // 已实现
            this.MouseLeave?.Invoke(this);
        }


        protected virtual void OnKeyDown(IKeyboardMessage args)
        {
            // 已实现
        }
        protected virtual void OnKeyUp(IKeyboardMessage args)
        {
            // 已实现
        }

        protected virtual void OnMouseDown(IMouseMessage args)
        {
            // 已实现
            this.MouseDown?.Invoke(this, args);
        }
        protected virtual void OnMouseUp(IMouseMessage args)
        {
            // 已实现
            this.MouseUp?.Invoke(this, args);
        }
        protected virtual void OnMouseMove(IMouseMessage args)
        {
            // 已实现
            this.MouseMove?.Invoke(this, args);
        }
        protected virtual void OnMouseWheel(IMouseMessage args)
        {
            // 已实现
            Trace.WriteLine($"Wheel {this.Name} {args.Wheel}");
        }

        protected virtual void OnRender(GameTime gameTime)
        {
            if (this.Background != null)
            {
                this.Background.Draw(Renderer, this.GlobalBounds, Color.White);
            }
        }

        /// <summary>
        /// 函数在渲染之前触发
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void OnUpdate(GameTime gameTime)
        {
        }


        #endregion

        #region IQuery
        public virtual Control this[string name]
        {
            get
            {
                return null;
            }
        }

        public virtual T Query<T>(string path) where T : Control
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


        #region Drag  Drop







        #endregion





        public String Font
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

        private String _font;


        public Single FontSize = 16;


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
        public String Name = String.Empty;
        public Boolean Visible = true;
        public Boolean Enabled = true;
        /// <summary>
        /// 控件是否可聚焦
        /// </summary>
        public Boolean Focusable = true;
        public Boolean IgnoreMouseEvents = false;
        public Boolean IgnoreKeyboardEvents = false;

        /// <summary>
        /// 允许拖放数据
        /// </summary>
        public Boolean AllowDrop = false;

        #region Events


        ///// <summary>
        ///// 开始拖拽
        ///// </summary>
        //public virtual event XamlEventHandler<Control> DragBegin;

        /// <summary>
        /// 本控件收到拖拽事件进入
        /// </summary>
        public virtual event XamlEventHandler<Control> DragEnter;

        /// <summary>
        /// 本控件收到拖拽事件离开
        /// </summary>
        public virtual event XamlEventHandler<Control> DragLeave;

        /// <summary>
        /// 本控件收到拖拽事件更新移动
        /// </summary>
        public virtual event XamlEventHandler<Control> DragOver;

        /// <summary>
        /// 处理拖放完成
        /// </summary>
        public virtual event XamlEventHandler<Control> Drop;




        public virtual event XamlEventHandler<Control> MouseEnter;
        public virtual event XamlEventHandler<Control> MouseLeave;
        public virtual event XamlEventHandler<Control> GotFocus;
        public virtual event XamlEventHandler<Control> LostFocus;




        public virtual event XamlEventHandler<Control, IMouseMessage> MouseDown;
        public virtual event XamlEventHandler<Control, IMouseMessage> MouseMove;
        public virtual event XamlEventHandler<Control, IMouseMessage> MouseUp;



        #endregion



        #region Position  Layout

        public XamlHorizontalAlignment HorizontalAlignment { get; set; }

        public XamlVerticalAlignment VerticalAlignment { get; set; }

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
        /// 用户数据
        /// </summary>
        public readonly IUserData UserData;


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
                    this.AutoWidth = true;
                    value.X = 0;
                }
                if (value.Y == Int32.MinValue)
                {
                    this.AutoHeight = true;
                    value.Y = 0;
                }
                this.globalBounds.Size = value;
            }
        }

        /// <summary>
        /// 控件宽度自动
        /// </summary>
        public Boolean AutoWidth { get; private set; }

        /// <summary>
        /// 控件高度自动
        /// </summary>
        public Boolean AutoHeight { get; private set; }


        /// <summary>
        /// 是否需要计算自动宽度
        /// </summary>
        protected Boolean NeedCalcAutoWidth
        {
            get
            {
                return this.HorizontalAlignment != XamlHorizontalAlignment.Stretch && this.AutoWidth;
            }
        }

        /// <summary>
        /// 是否需要计算自动高度
        /// </summary>
        protected Boolean NeedCalcAutoHeight
        {
            get
            {
                return this.VerticalAlignment != XamlVerticalAlignment.Stretch && this.AutoHeight;
            }
        }

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
        public Boolean IsPressed { get; protected set; }

        #endregion






    }
}
