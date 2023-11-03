using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework.Graphics;


namespace Aurora.Disktop.Xaml.Converters
{

    [XamlConverter(typeof(SimpleTexture))]
    internal class SimpleTextureConverter : IXamlPropertyConverter
    {
        public object? Convert(Type propertyType, string value)
        {
            if (String.IsNullOrEmpty(value)) return null;
            if (value.StartsWith("package://"))
            {
                // package://pkgName,1000
                var tokens = value.Substring(10).Split(',');
                var pack = AuroraState.PackageManager[tokens[0]];
                return pack.Read(Int32.Parse(tokens[1]));
            }
            else if (value.StartsWith("file://"))
            {
                // file://filename
                var filename = value.Substring(7);
                var device = AuroraState.Services.GetService<GraphicsDevice>();
                return SimpleTexture.FromFile(device, filename);
            }
            throw new Exception();

        }
    }
}
