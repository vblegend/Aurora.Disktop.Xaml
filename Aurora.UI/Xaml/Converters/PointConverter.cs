
using Microsoft.Xna.Framework;

namespace Aurora.UI.Xaml.Converters
{

    [XamlConverter(typeof(Point))]
    internal class PointConverter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            var arr = value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var x = arr[0] == "auto" ? Int32.MinValue : Int32.Parse(arr[0]);
            var y = arr[1] == "auto" ? Int32.MinValue : Int32.Parse(arr[1]);
            return new Point(x, y);
        }
    }
}
