
namespace Aurora.Disktop.Xaml.Converters
{

    [XamlConverter(typeof(String))]
    internal class StringConverter : IXamlPropertyConverter
    {
        public Object? Convert(Type propertyType, string value)
        {
            return value;
        }
    }
}
