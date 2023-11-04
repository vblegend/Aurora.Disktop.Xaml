using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

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



    [XamlConverter(typeof(SimpleTexture[]))]
    internal class SimpleTextureArrayConverter : IXamlPropertyConverter
    {
        public object? Convert(Type propertyType, string value)
        {
            if (String.IsNullOrEmpty(value)) return null;
            if (value.StartsWith("package://"))
            {
                // package://pkgName,1000
                var tokens = value.Substring(10).Split(',', StringSplitOptions.RemoveEmptyEntries);
                var pack = AuroraState.PackageManager[tokens[0]];
                var indexs = new List<Int32>();
                for (int i = 1; i < tokens.Length; i++)
                {
                    var token = tokens[i];
                    var rangeIndex = token.IndexOf("-");
                    if (rangeIndex > -1)
                    {
                        var s = Int32.Parse(token.Substring(0, rangeIndex));
                        var e = Int32.Parse(token.Substring(rangeIndex + 1));
                        for (int f = s; f <= e; f++) indexs.Add(f);
                    }
                    else
                    {
                        indexs.Add(Int32.Parse(token));
                    }
                }
                return pack.ReadTextures(indexs.ToArray());
            }
            else if (value.StartsWith("file://"))
            {
                // file://filename
                var filenames = value.Substring(7).Split(",", StringSplitOptions.RemoveEmptyEntries);
                var result = new SimpleTexture[filenames.Length];
                for (int i = 0; i < filenames.Length; i++)
                {
                    var device = AuroraState.Services.GetService<GraphicsDevice>();
                    result[i] = SimpleTexture.FromFile(device, filenames[i]);
                }
                return result;
            }
            throw new Exception();

        }
    }

}
