using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aurora.UI.Graphics
{

    /// <summary>
    /// 画刷填充方式
    /// </summary>
    public enum FillMode
    {
        //
        // 摘要:
        //     映像是沿控件的矩形工作区的顶部左侧对齐。
        None = 0,
        //
        // 摘要:
        //     图像平铺控件的客户端矩形。
        Tile = 1,
        //
        // 摘要:
        //     图像控件的客户端矩形内居中。
        Center = 2,
        //
        // 摘要:
        //     拉伸图形到控件的矩形工作区。
        Stretch = 3,
    }

    /// <summary>
    /// 画刷接口
    /// </summary>
    public interface IXamlBrush
    {
        public abstract void Draw(GraphicContext context, Rectangle destrect, Color color);

    }


    /// <summary>
    /// 颜色画刷
    /// </summary>
    public class ColorBrush : IXamlBrush
    {
        public readonly Color Color;

        public ColorBrush(Color color)
        {
            Color = color;
        }

        public ColorBrush(String htmlColor)
        {
            this.Color = ColorExtends.FromHtml(htmlColor);
        }

        public void Draw(GraphicContext context, Rectangle destrect, Color color)
        {
            context.FillRectangle(destrect, color);
        }
    }


    /// <summary>
    /// 普通纹理画刷
    /// </summary>
    public class TextureBrush : IXamlBrush
    {
        /// <summary>
        /// 填充模式
        /// </summary>
        public FillMode FillMode;


        public readonly ITexture Texture;

        public TextureBrush(ITexture texture)
        {
            this.Texture = texture;
            this.FillMode = FillMode.None;
        }
        public TextureBrush(ITexture texture, FillMode fillMode)
        {
            this.Texture = texture;
            this.FillMode = fillMode;
        }
        public void Draw(GraphicContext context, Rectangle destrect, Color color)
        {
         //   var state = context.PushState(blendState: BlendState.Additive);
            if (FillMode == FillMode.Tile)
            {
                context.DrawTitle(this.Texture, destrect, color);
            }
            else if (FillMode == FillMode.Stretch)
            {
                context.Draw(this.Texture, destrect, this.Texture.SourceRect, color);
            }
            else if (FillMode == FillMode.None)
            {
                context.Draw(this.Texture, destrect.Location.ToVector2(), color);
            }
            else if (FillMode == FillMode.Center)
            {
                var pos = destrect.Location - this.Texture.SourceRect.Center;
                context.Draw(this.Texture, pos.ToVector2(), color);
            }
         //   if (state) context.PopState();
        }
    }


    /// <summary>
    /// 九宫格画刷
    /// </summary>
    public class NineGridBrush : IXamlBrush
    {
        private readonly ITexture Texture;

        public NineGridBrush(ITexture texture)
        {
            this.Texture = texture;
        }

        public void Draw(GraphicContext context, Rectangle destrect, Color color)
        {
            //if (destrect.Width < this.Texture.Width || destrect.Height < this.Texture.Height)
            //{
            //    context.Draw(this.Texture, destrect, color);
            //    return;
            //}
            context.DrawNineSlice(this.Texture, destrect);
        }

    }




}
