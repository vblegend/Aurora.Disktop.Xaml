using Microsoft.Xna.Framework;


namespace Aurora.Disktop.Common.Tweens
{
    public sealed class TweenPoint : Tween<Point>
    {

        internal sealed override Point Lerp(Point from, Point to, Double time)
        {
            return new Point((Int32)MathHelper.Lerp(from.X, to.X, (float)time), (Int32)MathHelper.Lerp(from.Y, to.Y, (float)time));
        }

        internal sealed override void Apply(Point value)
        {
        }
    }
}
