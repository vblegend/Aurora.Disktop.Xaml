using Microsoft.Xna.Framework;

namespace Aurora.UI.Tweens
{
    public sealed class TweenVector2 : Tween<Vector2>
    {

        internal sealed override Vector2 Lerp(Vector2 from, Vector2 to, double time)
        {
            return Vector2.Lerp(from, to, (float)time);
        }

    }
}
