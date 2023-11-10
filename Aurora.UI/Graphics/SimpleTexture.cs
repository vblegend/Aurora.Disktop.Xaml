using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Resource.Package.Assets.Common;
using System.Diagnostics;

namespace Aurora.UI.Graphics
{

    public abstract class ITexture
    {

        internal ITexture(GraphicsDevice graphics)
        {
            this.device = graphics;
        }
        protected Texture2D tex { get; set; }

        public  Vector2 Offset;

        //public BlendState BlendState;

        public abstract Texture2D Tex();

        public GraphicsDevice device { get; private set; }



        /// <summary>
        /// 获取纹理大小范围
        /// 这个值是可以改变的
        /// </summary>
        public Rectangle SourceRect {
            get
            {
                return this.tex.Bounds;
            }
        }


        public Int32 Width
        {
            get
            {
                return this.tex.Bounds.Width;
            }
        }

        public Int32 Height
        {
            get
            {
                return this.tex.Bounds.Height;
            }
        }


        public Boolean GetPixel(Point position, out Color color )
        {
            Color[] colors = new Color[1];
            color = colors[0];
            if (this.tex == null) return false;
            if (!this.SourceRect.Contains(position)) return false;
            this.tex.GetData(0, 0, new Rectangle(position.X, position.Y, 1, 1), colors, 0, 1);
            color = colors[0];
            return true;
        }



    }

    /// <summary>
    /// 简单纹理
    /// </summary>
    public class SimpleTexture : ITexture
    {

        private SimpleTexture(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {

        }


        private void FromStrean(Stream stream, Action<byte[]> colorProcessor)
        {
            this.tex = Texture2D.FromStream(this.device, stream, colorProcessor);
        }


        public static SimpleTexture FromAssetPackageNode(GraphicsDevice graphicsDevice, IReadOnlyDataBlock block, Action<byte[]> colorProcessor = null)
        {
            var context = new SimpleTexture(graphicsDevice);
            Texture2D texture2D;
            if (block.Data.Length > 0)
            {
                texture2D = new Texture2D(graphicsDevice, block.Width, block.Height);
                texture2D.SetData(block.Data);
            }
            else
            {
                texture2D = new Texture2D(graphicsDevice, 1, 1);
            }

            context.Offset = new Vector2(block.OffsetX, block.OffsetY);
            context.tex = texture2D;
            return context;
        }


        public static SimpleTexture FromFileStream(GraphicsDevice graphicsDevice, Stream stream, Action<byte[]> colorProcessor = null)
        {
            var context = new SimpleTexture(graphicsDevice);
            context.FromStrean(stream, colorProcessor);
            return context;
        }


        public static SimpleTexture FromFile(GraphicsDevice graphicsDevice, String filename, Action<byte[]> colorProcessor = null)
        {
            var context = new SimpleTexture(graphicsDevice);
            using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                if (colorProcessor == null && filename.ToLower().EndsWith(".png"))
                {
                    colorProcessor = DefaultColorProcessors.PremultiplyAlpha;
                }
                context.FromStrean(fs, colorProcessor);
            }
            return context;
        }

        public static SimpleTexture FromTexture2D(Texture2D texture)
        {
            var context = new SimpleTexture(texture.GraphicsDevice);
            context.tex = texture;
            return context;
        }

        public override Texture2D Tex()
        {
            return this.tex;
        }

    }



    /// <summary>
    /// 渲染目标纹理
    /// </summary>
    public class TargetTexture : ITexture,IDisposable
    {

        private TargetTexture(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {

        }


        public static TargetTexture Create(GraphicsDevice graphicsDevice, Int32 width, Int32 height)
        {
            var context = new TargetTexture(graphicsDevice);
            context.Resize(width,height);
            return context;
        }

        public void Dispose()
        {
            if (this.tex != null)
            {
                this.tex.Dispose();
                this.tex = null;
            }
        }

        public void Resize(Int32 width, Int32 height)
        {
            var raw = this.tex;
            this.tex = new RenderTarget2D(device, width, height);
            if (raw != null)
            {
                raw.Dispose();
            }
        }


        public override RenderTarget2D Tex()
        {
            return this.tex as RenderTarget2D;
        }
    }
}
