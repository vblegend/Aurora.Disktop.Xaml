using Microsoft.Xna.Framework;


namespace Aurora.Disktop.Tweens
{
    public sealed class TweenRectangle : Tween<Rectangle>
    {

        internal sealed override Rectangle Lerp(Rectangle from, Rectangle to, double time)
        {
            var left = (int)MathHelper.Lerp(from.Left, to.Left, (float)time);
            var top = (int)MathHelper.Lerp(from.Top, to.Top, (float)time);
            var width = (int)MathHelper.Lerp(from.Width, to.Width, (float)time);
            var height = (int)MathHelper.Lerp(from.Height, to.Height, (float)time);
            return new Rectangle(left, top, width, height);
        }

    }
}
