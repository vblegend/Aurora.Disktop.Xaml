using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Resource.Package.Assets.Common;

namespace Aurora.UI.Graphics
{

    public abstract class ITexture : IDisposable
    {
        private bool disposedValue;
        public GraphicsDevice device { get; private set; }
        public Vector2 Offset;
        internal Rectangle sourceRect;
        protected Texture2D tex { get; set; }

        internal ITexture(GraphicsDevice graphics)
        {
            this.device = graphics;
        }

        public abstract Texture2D Tex();

        /// <summary>
        /// 获取纹理大小范围
        /// 这个值是可以改变的
        /// </summary>
        public Rectangle SourceRect
        {
            get
            {
                return this.sourceRect;
            }
        }


        public Int32 Width
        {
            get
            {
                return this.sourceRect.Width;
            }
            protected set
            {
                this.sourceRect.Width = value;
            }
        }

        public Int32 Height
        {
            get
            {
                return this.sourceRect.Height;
            }
            protected set
            {
                this.sourceRect.Height = value;
            }
        }

        public Boolean GetPixel(Point position, out Color color)
        {
            Color[] colors = new Color[1];
            color = colors[0];
            if (this.tex == null) return false;
            if (!this.SourceRect.Contains(position)) return false;
            this.tex.GetData(0, 0, new Rectangle(position.X, position.Y, 1, 1), colors, 0, 1);
            color = colors[0];
            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }
                if (this.tex != null)
                {
                    this.tex.Dispose();
                    this.tex = null;
                }
                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        ~ITexture()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }


    /// <summary>
    /// 懒加载纹理
    /// </summary>
    public class LazyTexture : ITexture
    {
        private Boolean loaded;
        private Func<Byte[]> LazyReader;

        public LazyTexture(GraphicsDevice graphics) : base(graphics)
        {
        }


        /// <summary>
        /// 资源懒加载，仅创建纹理对象，不加载数据
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static LazyTexture FromAssetPackageNode(GraphicsDevice graphicsDevice, IReadOnlyLazyInfo info)
        {
            var context = new LazyTexture(graphicsDevice);
            context.LazyReader = info.GetReader();
            context.Width = info.Width;
            context.Height = info.Height;
            context.Offset = new Vector2(info.OffsetX, info.OffsetY);
            return context;
        }



        public override Texture2D Tex()
        {
            if (this.tex == null && !this.loaded)
            {
                var data = this.LazyReader();
                this.tex = new Texture2D(this.device, this.Width, this.Height);
                this.tex.SetData(data);
                this.loaded = true;
            }
            return this.tex;
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

        #region MyRegion

        private void FromStrean(Stream stream, Action<byte[]> colorProcessor)
        {
            this.tex = Texture2D.FromStream(this.device, stream, colorProcessor);
            this.sourceRect = new Rectangle(0, 0, this.tex.Width, this.tex.Height);
        }

        public static SimpleTexture FromTexture2D(Texture2D texture)
        {
            var context = new SimpleTexture(texture.GraphicsDevice);
            context.tex = texture;
            return context;
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
        #endregion

        public override Texture2D Tex()
        {
            return this.tex;
        }
    }



    /// <summary>
    /// 渲染目标纹理
    /// </summary>
    public class TargetTexture : ITexture
    {

        private TargetTexture(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {

        }


        public static TargetTexture Create(GraphicsDevice graphicsDevice, Int32 width, Int32 height)
        {
            var context = new TargetTexture(graphicsDevice);
            context.Resize(width, height);
            return context;
        }

        public void Resize(Int32 width, Int32 height)
        {
            var raw = this.tex;
            this.tex = new RenderTarget2D(device, width, height);
            this.sourceRect = new Rectangle(0, 0, width, height);
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
