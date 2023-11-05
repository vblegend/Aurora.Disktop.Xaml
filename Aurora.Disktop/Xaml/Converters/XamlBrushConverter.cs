using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Aurora.Disktop.Xaml.Converters
{

    [XamlConverter(typeof(IXamlBrush))]
    internal class XamlBrushConverter : IXamlPropertyConverter
    {

        public Object Convert(Type propertyType, string value)
        {
            if (String.IsNullOrEmpty(value)) return null;
            if (value.StartsWith("color://"))
            {
                // color://#FFFFFFFF
                return new ColorBrush(value.Substring(8));
            }
            else if (value.StartsWith("package://"))
            {
                // package://texture,pkgName,1000
                // package://grid,pkgName,1000
                var tokens = value.Substring(10).Split(',');
                var pack = AuroraState.PackageManager[tokens[1]];
                var texture = pack.Read(Int32.Parse(tokens[2]));
                if (tokens[0] == "texture")
                {
                    Graphics.FillMode fill = Graphics.FillMode.None;
                    if (tokens.Length > 3)
                    {
                        fill = (Graphics.FillMode)Enum.Parse(typeof(Graphics.FillMode), tokens[3]);
                    }
                    return new TextureBrush(texture, fill);
                }
                else if (tokens[0] == "grid")
                {
                    return new NineGridBrush(texture);
                }
            }
            else if (value.StartsWith("file://"))
            {
                // file://texture,filename
                // file://grid,filename
                var tokens = value.Substring(7).Split(',');
                var device = AuroraState.Services.GetService<GraphicsDevice>();
                var texture = SimpleTexture.FromFile(device,tokens[1]);
                if (tokens[0] == "texture")
                {
                    return new TextureBrush(texture);
                }
                else if (tokens[0] == "grid")
                {
                    return new NineGridBrush(texture);
                }
            }
            throw new Exception();

        }
    }
}
