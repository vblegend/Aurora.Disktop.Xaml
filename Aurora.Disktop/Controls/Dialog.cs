using Aurora.Disktop.Common;
using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Aurora.Disktop.Controls
{


    public class Dialog : Panel
    {
        public Dialog()
        {
            this.Pinned = true;
            this.AutoTop = true;
        }



        protected override void OnRender(GameTime gameTime)
        {
            base.OnRender(gameTime);
        }




        protected override void OnMouseDown(MouseButtons button, Point point)
        {
            if (button == MouseButtons.Left && !this.Pinned)
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
                    updatable.LayoutUpdate(true);
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


        public void MoveTop()
        {
            if (this.Parent is Panel panel)
            {
                if (panel.Children[panel.Children.Count - 1] != this)
                {
                    panel.Children.Remove(this);
                    panel.Children.Add(this);
                }
            }
        }

        public override bool HitTest(Point position)
        {
            if (!this.GlobalBounds.Contains(position)) return false;
            if (this.Background is TextureBrush brush )
            {
                var offset = position.Sub(this.GlobalLocation);
                if (brush.Texture.GetPixel(offset, out var color))
                {
                    if (color.A < 100) return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 固定对话框，不可移动
        /// </summary>
        public Boolean Pinned { get; set; }

        /// <summary>
        /// 对话框获取焦点后自动置顶
        /// </summary>
        public Boolean AutoTop { get; set; }


        private Point? dropPosition;

        /// <summary>
        /// 命中过滤器
        /// </summary>
        private Boolean HitFilter { get; set; }

        // Declare the event.
        public event XamlClickEventHandler<Dialog> Click;
    }
}
