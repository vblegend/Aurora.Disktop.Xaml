using Microsoft.Xna.Framework;


namespace Aurora.UI.Xaml.Converters
{

    [XamlConverter(typeof(Color))]
    internal class ColorConverter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) return Color.Transparent;
            return ColorExtends.FromHtml(value);
        }
    }
}
