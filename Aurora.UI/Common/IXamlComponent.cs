using Aurora.UI.Controls;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;

namespace Aurora.UI.Common
{
    /// <summary>
    /// GUI控件查询接口
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// 获取当前控件下指定名称的对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Control，未找到时返回 null</returns>
        Control this[String name] { get; }

        /// <summary>
        /// 查询当前控件下指定路径的控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="path">控件路径 control/control/</param>
        /// <returns>Control，未找到时返回 null</returns>
        T Query<T>(String path) where T : Control;
    }


    /// <summary>
    /// Xaml Component
    /// </summary>
    public interface IXamlComponent
    {
    }



    /// <summary>
    /// Xaml UI Control
    /// </summary>
    public interface IXamlControl : IXamlComponent
    {
    }



    public delegate void XamlClickEventHandler<T>(T sender) where T : Control;

    public delegate void XamlEventHandler<T>(T sender) where T : Control;

    public delegate void XamlEventHandler<T, TArgs>(T sender, TArgs args) where T : Control;



    public class ItemEventArgs<TItem>
    {
        internal ItemEventArgs(Int32 index, TItem item, Point position)
        {
            this.Index = index;
            this.Item = item;
            this.GlobalPosition = position;
        }
        public readonly Int32 Index;
        public readonly TItem Item;
        public readonly Point GlobalPosition;
    }

    public delegate void XamlItemEventHandler<T, TItem>(T sender, ItemEventArgs<TItem> args) where T : Control;








    public class ImageMatrixDrawEventArgs<TItem>
    {
        internal ImageMatrixDrawEventArgs(Int32 index, TItem item, Rectangle rectangle, GraphicContext renderer, GameTime gameTime)
        {
            this.Index = index;
            this.Item = item;
            this.Rectangle = rectangle;
            this.Renderer = renderer;
            this.GameTime = gameTime;
        }
        public readonly Int32 Index;
        public readonly TItem Item;
        public readonly Rectangle Rectangle;
        public readonly GraphicContext Renderer;
        public readonly GameTime GameTime;
    }
    public delegate void ImageMatrixDrawEventHandler<T, TItem>(T sender, ImageMatrixDrawEventArgs<TItem> args) where T : Control;
}
