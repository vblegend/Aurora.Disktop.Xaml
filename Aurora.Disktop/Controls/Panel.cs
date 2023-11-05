using Aurora.Disktop.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Disktop.Controls
{

    public interface IPanelControl
    {
        T Add<T>(T control) where T : Control;
        void Remove(Control control);
        Control? this[Int32 index] { get; }
        Int32 Count { get; }
        Int32 IndexOf(Control control);
    }












    public class Panel : Control, IPanelControl, ILayoutUpdatable, IRenderable, IQuery
    {
        public Panel()
        {
            this.Children = new List<Control>();
        }
        public List<Control> Children { get; protected set; }
        public Control? this[int index] => this.Children[index];
        public int Count => this.Children.Count;
        public T Add<T>(T control) where T : Control
        {
            this.Children.Add(control);
            control.Parent = this;
            control.Root = this.Root;
            if (this is ILayoutUpdatable updatable)
            {
                updatable.LayoutUpdate(true, true);
            }
            return control;
        }

        public int IndexOf(Control control)
        {
            return this.Children.IndexOf(control);
        }

        public void Remove(Control control)
        {
            this.Children.Remove(control);
            control.Parent = null;
            if (this is ILayoutUpdatable updatable)
            {
                updatable.LayoutUpdate(true);
            }
        }

        void IRenderable.ProcessUpdate(GameTime gameTime)
        {
            this.OnUpdate(gameTime);
            for (int i = 0; i < this.Children.Count; i++)
            {
                var child = this.Children[i];
                if (child.Visible && child is IRenderable renderable)
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
            for (int i = 0; i < this.Children.Count; i++)
            {
                var child = this.Children[i];
                if (child.Visible && child is IRenderable renderable)
                {
                    renderable.ProcessRender(gameTime);
                }
            }
            this.DrawDebugBounds();
        }

        void ILayoutUpdatable.LayoutUpdate(Boolean updateChildren, Boolean force = false)
        {
            if (this.CalcGlobalBounds() || force)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i] is ILayoutUpdatable updatable)
                    {
                        if (updateChildren) updatable.LayoutUpdate(true);
                        this.extendBounds = Rectangle.Union(this.extendBounds, Children[i].GlobalBounds);
                    }
                }
            }
        }

        #region IQuery
        public new Control? this[string name]
        {
            get
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i].Name == name) return Children[i];
                }
                return null;
            }
        }
        #endregion

    }
}
