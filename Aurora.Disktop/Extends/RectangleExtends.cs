

namespace Microsoft.Xna.Framework
{
    public static class RectangleExtends
    {


        public static void Extends(this Rectangle rectangle, Point point)
        {
            if (point.X < rectangle.X) rectangle.X = point.X;
            if (point.Y < rectangle.Y) rectangle.Y = point.Y;
            if (point.X > rectangle.X + rectangle.Width) rectangle.Width = point.X - rectangle.Width;
            if (point.Y > rectangle.Y + rectangle.Height) rectangle.Height = point.Y - rectangle.Height;
        }


        public static Rectangle Add(this Rectangle rectangle, Point point)
        {
            return new Rectangle(rectangle.Left + point.X, rectangle.Top + point.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle Sub(this Rectangle rectangle, Point point)
        {
            return new Rectangle(rectangle.Left - point.X, rectangle.Top - point.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle Add(this Rectangle rectangle, Vector2 point)
        {
            return new Rectangle(rectangle.Left + (Int32)point.X, rectangle.Top + (Int32)point.Y, rectangle.Width, rectangle.Height);
        }

        public static void Offset(this Point origin, Point point)
        {
            origin.X += point.X;
            origin.Y += point.Y;
        }

        public static Point Add(this Point origin, Point point)
        {
            return new Point(origin.X + point.X, origin.Y + point.Y);
        }
        public static Point Sub(this Point origin, Point point)
        {
            return new Point(origin.X - point.X, origin.Y - point.Y);
        }
    }
}
