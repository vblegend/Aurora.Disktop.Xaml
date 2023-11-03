using Microsoft.Xna.Framework.Graphics;



namespace Aurora.Disktop.Extends
{
    public class SpriteBatchExtends : SpriteBatch
    {
        public SpriteBatchExtends(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public void xxx(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(blendState: BlendState.NonPremultiplied);

            //this._beginCalled


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);





        }


        public void xxfx(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
        }

    }
}
