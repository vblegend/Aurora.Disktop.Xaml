using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Resource.Package.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Disktop.Common
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



        public SimpleTexture? Read(Int32 index)
        {

            var device = AuroraState.Services.GetService<GraphicsDevice>();
            var node = this.assetFileStream.Read((UInt32)index);
            if (node != null)
            {
                using (var ms = new MemoryStream(node.Data))
                {
                    var texture = SimpleTexture.FromStream(device, ms);
                    texture.Offset = new Microsoft.Xna.Framework.Vector2(node.OffsetX, node.OffsetY);
                    return texture;
                }
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
                    using (var ms = new MemoryStream(node.Data))
                    {
                        result[i] = SimpleTexture.FromStream(device, ms);
                        result[i].Offset = new Microsoft.Xna.Framework.Vector2(node.OffsetX, node.OffsetY);
                    }
                }
            }
            return result;
        }




        public Texture2D? Read2D(Int32 index)
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
