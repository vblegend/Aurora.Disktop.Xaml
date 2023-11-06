using Microsoft.Xna.Framework;


namespace Aurora.Disktop.Common.Tweens
{
    public sealed class TweenRectangle : Tween<Rectangle>
    {

        internal sealed override Rectangle Lerp(Rectangle from, Rectangle to, Double time)
        {
            var left = (Int32)MathHelper.Lerp(from.Left, to.Left, (float)time);
            var top = (Int32)MathHelper.Lerp(from.Top, to.Top, (float)time);
            var width = (Int32)MathHelper.Lerp(from.Width, to.Width, (float)time);
            var height = (Int32)MathHelper.Lerp(from.Height, to.Height, (float)time);
            return new Rectangle(left,top,width,height);
        }

        internal sealed override void Apply(Rectangle value)
        {

        }
    }
}
