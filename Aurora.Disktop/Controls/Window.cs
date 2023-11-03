using Aurora.Disktop.Common;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Aurora.Disktop.Controls
{


    public class Window : Panel
    {




        protected override void OnRender(GameTime gameTime)
        {
            base.OnRender(gameTime);
        }




        protected override void OnMouseDown(MouseButtons button, Point point)
        {
            if (button == MouseButtons.Left && this.CanMove)
            {
                dropPosition = point;
            }
        }

        protected override void OnMouseMove(Point point)
        {
            if (dropPosition.HasValue)
            {
                var offset = point.Sub(dropPosition.Value);
                this.Location = this.Location.Add(offset);
                if (this is ILayoutUpdatable updatable)
                {
                    updatable.LayoutUpdate();
                }
                dropPosition = point;
            }
        }

        protected override void OnMouseUp(MouseButtons button, Point point)
        {
            if (button == MouseButtons.Left)
            {
                dropPosition = null;
            }
        }



        public Boolean AutoTop { get; set; }

        public Boolean CanMove { get; set; }

        private Point? dropPosition;

        // Declare the event.
        public event XamlClickEventHandler<Window> Click;
    }
}
