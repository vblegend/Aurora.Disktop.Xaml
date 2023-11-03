
namespace Aurora.Disktop.Xaml.Converters
{


    [XamlConverter(typeof(Boolean))]
    internal class BooleanConverter : IXamlPropertyConverter
    {
        public object? Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            if (value == "True") return true;
            if (value == "False") return false;
            throw new Exception();
        }

    }



    [XamlConverter(typeof(Boolean?))]
    internal class BooleanNullableConverter : IXamlPropertyConverter
    {
        public object? Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            if (value == "True") return true;
            if (value == "False") return false;
            if (value == "Null") return null;
            throw new Exception();
        }

    }
}
