﻿using Aurora.Disktop.Common;
using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Aurora.Disktop.Controls
{

    public interface IXamlEventHandler
    {
        void MessageHandler(EventMessage msg);
    }

    public interface IRenderable
    {
        void ProcessRender(GameTime gameTime);
    }

    public interface ILayoutUpdatable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentPosition">父对象的绝对坐标</param>
        void LayoutUpdate();
    }


    public abstract class Control : IQuery, IXamlEventHandler, IRenderable, IControl, ILayoutUpdatable
    {

        public GraphicContext Renderer { get; private set; }

        protected Control()
        {
            this.Name = string.Empty;
            this.Renderer = AuroraState.Services.GetService<GraphicContext>();
            this.Enabled = true;
            this.Visible = true;
        }

        /// <summary>
        /// 布局更新 更新全局位置
        /// </summary>
        void ILayoutUpdatable.LayoutUpdate()
        {
            if (this.Parent != null)
            {
                this.globalBounds.Location = this.Parent.GlobalLocation.Add(this.Location);
            }
        }

        /// <summary>
        /// 控件渲染处理
        /// </summary>
        /// <param name="gameTime"></param>
        void IRenderable.ProcessRender(GameTime gameTime)
        {
            this.OnRender(gameTime);
            this.Renderer.DrawRectangle(GlobalBounds, Color.Red, 1);
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

        #endregion





        #region IQuery
        public Control? this[string name]
        {
            get
            {
                //for (int i = 0; i < Children.Count; i++)
                //{
                //    if (Children[i].Name == name) return Children[i];
                //}
                return null;
            }
        }

        public T? Query<T>(string path) where T : Control
        {
            var paths = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            Control? control = this;
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



        #endregion


        /// <summary>
        /// 父控件对象
        /// </summary>
        public virtual Control? Parent { get; set; }

        public IXamlBrush? Background { get; set; }

        /// <summary>
        /// 控件名字
        /// </summary>
        public String Name { get; set; }
        public Boolean Visible { get; set; }
        public Boolean Enabled { get; set; }
        public Boolean IgnoreMouseEvents { get; set; }
        public Boolean IgnoreKeyboardEvents { get; set; }
        public Boolean IsLayoutChanged { get; set; }

        #region Position


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


        protected Rectangle globalBounds;


        private Point location;
        /// <summary>
        /// 获取/设置 控件本地坐标
        /// </summary>
        public Point Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
                (this as ILayoutUpdatable).LayoutUpdate();
            }
        }

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
                this.globalBounds.Size = value;

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
        public Boolean IsPressed { get; private set; }

        #endregion






    }
}
