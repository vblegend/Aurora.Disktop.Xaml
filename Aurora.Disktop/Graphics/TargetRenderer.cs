using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Aurora.Disktop.Graphics
{
    public class TargetRenderer : IDisposable
    {
        private GraphicContext? context;
        private BlendState? originBlendState;

        private RenderTargetBinding[] originTargets;

        internal TargetRenderer(GraphicContext context, TargetTexture target)
        {
            this.originBlendState = context.End();
            this.context = context;
            this.originTargets = this.context.GraphicsDevice.GetRenderTargets();
            var targets = new RenderTargetBinding[this.originTargets.Length + 1];
            targets[0] = new RenderTargetBinding(target.Tex());
            if (this.originTargets.Length > 0) {
                Array.Copy(this.originTargets, 0, targets, 1, this.originTargets.Length);
            }
            this.context.GraphicsDevice.SetRenderTargets(targets);
            this.context.Begin(SpriteSortMode.Immediate);
            //this.context.GraphicsDevice.Clear(Color.Transparent);
        }







        public void Dispose()
        {
            if (this.context != null)
            {
                this.context.End();
                this.context.GraphicsDevice.SetRenderTargets(this.originTargets);
                this.context.Begin(blendState: this.originBlendState);
                this.context = null;
            }
            this.originTargets = null;
            this.originBlendState = null;
        }
    }
}
