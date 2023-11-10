using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Aurora.UI.Graphics.GraphicContext;

namespace Aurora.UI.Graphics
{
    public class TargetRenderer : IDisposable
    {
        private GraphicContext context;
        private ContextState state;

        private RenderTargetBinding[] originTargets;

        internal TargetRenderer(GraphicContext context, TargetTexture target)
        {
            this.state = context.EndState();
            this.context = context;
            this.originTargets = this.context.GraphicsDevice.GetRenderTargets();
            var targets = new RenderTargetBinding[this.originTargets.Length + 1];
            targets[0] = new RenderTargetBinding(target.Tex());
            if (this.originTargets.Length > 0) {
                Array.Copy(this.originTargets, 0, targets, 1, this.originTargets.Length);
            }
            this.context.GraphicsDevice.SetRenderTargets(targets);
            this.context.SetState(SpriteSortMode.Immediate);
            this.context.GraphicsDevice.Clear(Color.Transparent);
        }







        public void Dispose()
        {
            if (this.context != null)
            {
                this.context.End();
                this.context.GraphicsDevice.SetRenderTargets(this.originTargets);
                this.context.RestoreState(this.state);
                this.context = null;
            }
            this.originTargets = null;
        }
    }
}
