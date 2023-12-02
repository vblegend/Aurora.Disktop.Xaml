using Microsoft.Xna.Framework;
using Aurora.UI.Animation;

namespace Aurora.UI.Tweens.Tweens
{
    public sealed class TweenPoint : Tween<TweenPoint, Point>
    {

        internal sealed override Point Lerp(Point from, Point to, double time)
        {
            return new Point((int)MathHelper.Lerp(from.X, to.X, (float)time), (int)MathHelper.Lerp(from.Y, to.Y, (float)time));
        }

    }
}
