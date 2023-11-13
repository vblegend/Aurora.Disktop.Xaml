

using Aurora.UI.Graphics;
using Aurora.UI.Xaml.System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Xna.Framework.Graphics;
using Resource.Package.Assets;
using System;
using System.Diagnostics;


namespace Aurora.UI.Common
{

    public interface ITextureCache
    {
        public void SetValue(String key, SimpleTexture texture);

        public Boolean GetValue(String key, out SimpleTexture texture);

    }



    public class TextureMemoryCache : ITextureCache
    {
        private MemoryCache _cache;
        // 缓存数据，并设置过期时间
        private MemoryCacheEntryOptions _cacheEntryOptions;


        public TextureMemoryCache(TimeSpan? SlidingExpiration)
        {
            var options = new MemoryCacheOptions();
            _cache = new MemoryCache(options);
            _cacheEntryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = SlidingExpiration
            };
        }

        public bool GetValue(string key, out SimpleTexture texture)
        {
            return _cache.TryGetValue(key, out texture);
        }

        public void SetValue(string key, SimpleTexture texture)
        {
            _cache.Set(key, texture, this._cacheEntryOptions);
        }

    }



    public class PackageManager
    {
        public ITextureCache TextureCache;
        private GraphicsDevice Device;

        private Dictionary<String, AssetFileStream> packages = new Dictionary<String, AssetFileStream>();


        public PackageManager()
        {
            this.TextureCache = new TextureMemoryCache(new TimeSpan(0, 0, 30));
            this.Device = AuroraState.Services.GetService<GraphicsDevice>();
        }


        public void Register(String packageName, String path, String password)
        {
            this.Device = AuroraState.Services.GetService<GraphicsDevice>();
            if (packages.ContainsKey(packageName)) return;
            var file = AssetFileStream.Open(path, password);
            packages.Add(packageName, file);
        }






        public SimpleTexture LazyLoadTexture(String packageName, Int32 index)
        {
            if (this.TextureCache.GetValue(packageName + index, out var texture))
            {
                return texture;
            }
            if (!this.packages.TryGetValue(packageName, out var package)) return null;

            var info = package.LazyRead((UInt32)index);
            if (info != null)
            {
                texture = SimpleTexture.FromAssetPackageLazy(this.Device, info);
                this.TextureCache.SetValue(packageName + index, texture);
                return texture;
            }
            return null;
        }







        /// <summary>
        /// 从指定资源包加载纹理
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public SimpleTexture LoadTexture(String packageName,Int32 index)
        {
            if (this.TextureCache.GetValue(packageName + index, out var texture))
            {
                return texture;
            }
            if (!this.packages.TryGetValue(packageName, out var package))return null;
 
            var block = package.Read((UInt32)index);
            if (block != null)
            {
                texture = SimpleTexture.FromAssetPackageNode(this.Device, block, null);
                this.TextureCache.SetValue(packageName + index, texture);
                return texture;
            }
            return null;
        }




        /// <summary>
        /// 从指定资源包加载多个纹理
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="indexs"></param>
        /// <returns></returns>
        public SimpleTexture[] LoadTextures(String packageName, params Int32[] indexs)
        {
            var result = new SimpleTexture[indexs.Length];
            if (!this.packages.TryGetValue(packageName, out var package)) return null;
            for (int i = 0; i < indexs.Length; i++)
            {
                var index = (UInt32)indexs[i];
                if (this.TextureCache.GetValue(packageName + index, out var texture))
                {
                    result[i] = texture;
                }
                else
                {
                    var block = package.Read(index);
                    if (block != null)
                    {
                        texture = SimpleTexture.FromAssetPackageNode(this.Device, block, null);
                        this.TextureCache.SetValue(packageName + index, texture);
                        result[i] = texture;
                    }
                }

            }
            return result;
        }







    }
}
