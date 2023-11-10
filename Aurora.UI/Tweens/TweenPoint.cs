using Microsoft.Xna.Framework;


namespace Aurora.UI.Tweens
{
    public sealed class TweenPoint : Tween<Point>
    {

        internal sealed override Point Lerp(Point from, Point to, double time)
        {
            return new Point((int)MathHelper.Lerp(from.X, to.X, (float)time), (int)MathHelper.Lerp(from.Y, to.Y, (float)time));
        }

    }
}
