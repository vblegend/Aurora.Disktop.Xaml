﻿

using Microsoft.Xna.Framework;


namespace Aurora.Disktop.Common
{

    public enum HorizontalAlignment
    {
        Left = 0,
        Center = 1,
        Right = 2,
        Stretch = 3
    }

    public enum VerticalAlignment
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


    public class Sized
    {
        public Int32 Width;
        public Int32 Height;




        public Boolean AutoWidth { get; private set; }
        public Boolean AutoHeight { get; private set; }




        public Point ToPoint()
        {
            return new Point(Width, Height);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(Width, Height);
        }
    }


}
