using Microsoft.Xna.Framework;

namespace Aurora.Disktop.UI
{
    public class SimpleFpsCounter
    {
        private UInt32 updates = 0;
        private double elapsed = 0;
        private double last = 0;
        private double now = 0;
        public UInt32 Fps { get; private set; }

        public void Update(GameTime gameTime)
        {
            now = gameTime.TotalGameTime.TotalSeconds;
            elapsed = now - last;
            if (elapsed > 1.0f)
            {
                this.Fps = updates;
                elapsed = 0;
                updates = 0;
                last = now;
            }
            updates++;
        }
    }

}
