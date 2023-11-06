using Microsoft.Xna.Framework;

namespace Aurora.Disktop.Common.Tweens
{
    public sealed class TweenVector2 : Tween<Vector2>
    {

        internal sealed override Vector2 Lerp(Vector2 from, Vector2 to, Double time)
        {
            return Vector2.Lerp(from, to, (float)time);
        }

        internal sealed override void Apply(Vector2 value)
        {
        }
    }
}
