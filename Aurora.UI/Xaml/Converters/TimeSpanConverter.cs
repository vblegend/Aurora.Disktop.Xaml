
namespace Aurora.UI.Xaml.Converters
{

    [XamlConverter(typeof(TimeSpan))]
    internal class TimeSpanConverter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            var ms = Int32.Parse(value);
            return new TimeSpan(0, 0, 0, 0, ms);
        }
    }
}
