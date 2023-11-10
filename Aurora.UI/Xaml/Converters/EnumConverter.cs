

namespace Aurora.UI.Xaml.Converters
{

    [XamlConverter(typeof(Enum))]
    internal class EnumConverter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            if (Enum.TryParse(propertyType, value, out var parsedEnum))
            {
                return parsedEnum;
            }
            throw new InvalidConvertTypeException($"Invalid Convert Type: {propertyType.FullName} {value}");
        }
    }
}
