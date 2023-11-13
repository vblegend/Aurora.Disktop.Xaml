using Aurora.UI.Graphics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Xna.Framework.Graphics;
using Resource.Package.Assets;
using System.Diagnostics;

namespace Aurora.UI.Common
{
   












    public class ResourcePackage : IDisposable
    {
        private AssetFileStream assetFileStream;

        public ITextureCache Cache = new TextureMemoryCache(new TimeSpan(0, 0, 30));




        private void test()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            string key = "myKey";
            string value = "myValue";

            // 缓存数据，并设置过期时间
            var cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) };

            // 将数据添加到缓存
            memoryCache.Set(key, value, cacheEntryOptions);



            Stopwatch sw = Stopwatch.StartNew();

            // 访问缓存项时手动更新过期时间
            for (int i = 0; i < 100000; i++)
            {
                memoryCache.TryGetValue(key, out string cachedValue);
            }


            sw.Stop();


            Trace.WriteLine(sw.ElapsedMilliseconds);


        }




        internal ResourcePackage(AssetFileStream assetFileStream)
        {
            this.assetFileStream = assetFileStream;
            test();
        }


        public SimpleTexture LazyLoadTexture()
        {

            return null;
        }






        public SimpleTexture Read(Int32 index)
        {
            var device = AuroraState.Services.GetService<GraphicsDevice>();
            var node = this.assetFileStream.Read((UInt32)index);
            if (node != null)
            {
                Action<byte[]> processor = null;
                //processor = DefaultColorProcessors.PremultiplyAlpha;
                var texture = SimpleTexture.FromAssetPackageNode(device, node, processor);
                return texture;
            }
            return null;
        }

        public SimpleTexture[] ReadTextures(params Int32[] indexs)
        {
            var result = new SimpleTexture[indexs.Length];
            var device = AuroraState.Services.GetService<GraphicsDevice>();
            for (int i = 0; i < indexs.Length; i++)
            {
                if (this.Cache.GetValue(i.ToString(),  out var vv))
                {
                    result[i] = vv;
                }

                var node = this.assetFileStream.Read((UInt32)indexs[i]);
                if (node != null)
                {
                    Action<byte[]> processor = null;
                    // processor = DefaultColorProcessors.PremultiplyAlpha;
                    result[i] = SimpleTexture.FromAssetPackageNode(device, node, processor);
                }
            }
            return result;
        }




        public Texture2D Read2D(Int32 index)
        {

            var device = AuroraState.Services.GetService<GraphicsDevice>();
            var node = this.assetFileStream.Read((UInt32)index);
            if (node != null)
            {
                using (var ms = new MemoryStream(node.Data))
                {
                    return Texture2D.FromStream(device, ms);
                }
            }
            return null;
        }






        public void Dispose()
        {
            if (assetFileStream != null)
            {
                assetFileStream.Close();
                assetFileStream.Dispose();
                assetFileStream = null;
            }
        }
    }
}
