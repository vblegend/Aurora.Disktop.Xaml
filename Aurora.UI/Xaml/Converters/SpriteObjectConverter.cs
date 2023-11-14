using Aurora.UI.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
 

namespace Aurora.UI.Xaml.Converters
{
    [XamlConverter(typeof(SpriteObject))]
    internal class SpriteObjectConverter : IXamlPropertyConverter
    {


        // package://name,1001,3,1
        // file://filename,3,1
        // content://filename,3,1

        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();

            if (value.StartsWith("package://"))
            {
                var args = value.Substring(10).Split(",", StringSplitOptions.RemoveEmptyEntries);
                var texture = AuroraState.PackageManager.LoadLazyTexture(args[0], Int32.Parse(args[1]));
                return new SpriteObject(texture, Int32.Parse(args[2]), Int32.Parse(args[3]));
            }
            else if (value.StartsWith("file://"))
            {
                var args = value.Substring(7).Split(",", StringSplitOptions.RemoveEmptyEntries);
                var device = AuroraState.Services.GetService<GraphicsDevice>();
                SimpleTexture texture = SimpleTexture.FromFile(device, args[0]);
                return new SpriteObject(texture, Int32.Parse(args[1]), Int32.Parse(args[2]));
            }
            else if (value.StartsWith("content://"))
            {
                var args = value.Substring(10).Split(",", StringSplitOptions.RemoveEmptyEntries);
                var content = AuroraState.Services.GetService<ContentManager>();
                var texture = content.Load<Texture2D>(args[0]);
                var stexture = SimpleTexture.FromTexture2D(texture);
                return new SpriteObject(stexture, Int32.Parse(args[1]), Int32.Parse(args[2]));
            }
            else
            {
                throw new Exception();
            }
        }
    }


}
