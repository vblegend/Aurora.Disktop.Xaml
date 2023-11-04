using Aurora.Disktop.Common;
using SpriteFontPlus;

namespace Aurora.Disktop.Xaml.Converters
{
    [XamlConverter(typeof(DynamicSpriteFont))]
    internal class FontConverter : IXamlPropertyConverter
    {
        public Object? Convert(Type propertyType, string value)
        {
            if (String.IsNullOrEmpty(value)) return null;
            if (value.StartsWith("font://"))
            {
                // font://微软雅黑,32
                var tokens = value.Substring(7).Split(',');
                //var pack = AuroraState.PackageManager[tokens[0]];

                return null;
            }
            else if (value.StartsWith("ttf://"))
            {
                // ttf://./fonts/xxx.ttf,24
                var tokens = value.Substring(6).Split("," , StringSplitOptions.RemoveEmptyEntries);
                var font = TTFFont.FromFile(tokens[0], Int32.Parse(tokens[1]));
                return font;
            }
            throw new Exception();

        }
    }







    
}
