using Aurora.Disktop.Common;
using Microsoft.Xna.Framework;


namespace Aurora.Disktop.Controls
{
    public class Label : ContentControl
    {
        public Label()
        {
            // #d6c79c
            this.HorizontalContentAlignment = XamlHorizontalAlignment.Left;
            this.VerticalContentAlignment = XamlVerticalAlignment.Top;
        }

        /// <summary>
        /// 计算自动大小
        /// </summary>
        /// <returns></returns>
        protected override void CalcAutoSize()
        {
            if (this.NeedCalcAutoHeight)
            {
                this.globalBounds.Height = 20;
            }
            if (this.NeedCalcAutoWidth && this.Font != null)
            {
                var content = this.content.ToString();
                var fontSize = (this.Height - this.Padding.Top - this.Padding.Bottom) / this.Font.Size;
                var size = this.Renderer.MeasureString(this.Font, content) * fontSize;
                var px = size + new Vector2(this.Padding.Left + this.Padding.Right + this.Height, 0);
                this.globalBounds.Width = (Int32)px.X;
            }
        }



    }
}
