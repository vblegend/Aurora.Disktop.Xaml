

using Microsoft.Xna.Framework;


namespace Aurora.UI.Common
{

    public enum XamlHorizontalAlignment
    {
        Left = 0,
        Center = 1,
        Right = 2,
        Stretch = 3
    }

    public enum XamlVerticalAlignment
    {
        Top = 0,
        Center = 1,
        Bottom = 2,
        Stretch = 3
    }

    public struct Thickness
    {
        public Thickness(Int32 value)
        {
            this.Left = this.Top = this.Right = this.Bottom = value;
        }

        public Thickness(Int32 lr, Int32 tb)
        {
            this.Left = this.Right = lr;
            this.Top = this.Bottom = tb;
        }

        public Thickness(Int32 left, Int32 top, Int32 right, Int32 bottom)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        public override string ToString()
        {
            return $"Left:{Left}, Top:{Top}, Right:{Right}, Bottom:{Bottom}";
        }




        public Int32 Left;
        public Int32 Top;
        public Int32 Right;
        public Int32 Bottom;
    }


    public enum XamlDirection
    {
        /// <summary>
        /// 从左到右
        /// </summary>
        LeftToRight = 0,
        /// <summary>
        /// 从下到上
        /// </summary>
        BottomToTop = 1,
        /// <summary>
        /// 从右到左
        /// </summary>
        RightToLeft = 2,
        /// <summary>
        /// 从上到下
        /// </summary>
        TopToBottom = 3
    }


    public enum XamlOrientation {
        /// <summary>
        /// 横向的
        /// </summary>
        Horizontal,
        /// <summary>
        /// 纵向的
        /// </summary>
        Vertical,
    }


}
