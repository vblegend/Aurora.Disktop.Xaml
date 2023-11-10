using Aurora.UI.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Resource.Package.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.UI.Common
{
    public class ResourcePackage : IDisposable
    {
        private AssetFileStream assetFileStream;


        private ResourcePackage(AssetFileStream assetFileStream)
        {
            this.assetFileStream = assetFileStream;
        }


        public static ResourcePackage Open(String filename, String password = "")
        {
            var file = AssetFileStream.Open(filename, password);
            return new ResourcePackage(file);
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
