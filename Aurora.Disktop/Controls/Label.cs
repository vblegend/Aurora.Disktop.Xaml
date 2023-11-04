using Aurora.Disktop.Common;
using Microsoft.Xna.Framework;


namespace Aurora.Disktop.Controls
{
    public class Label : ContentControl
    {
        public Label()
        {
            // #d6c79c
            this.HorizontalContentAlignment = HorizontalAlignment.Left;
            this.VerticalContentAlignment = VerticalAlignment.Top;
        }
        protected override void OnRender(GameTime gameTime)
        {
            
            base.OnRender(gameTime);
        }
        
    }
}
