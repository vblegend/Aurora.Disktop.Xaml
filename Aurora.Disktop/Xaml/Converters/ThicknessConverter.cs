using Aurora.Disktop.Common;


namespace Aurora.Disktop.Xaml.Converters
{
    [XamlConverter(typeof(Thickness))]
    internal class ThicknessConverter : IXamlPropertyConverter
    {
        public Object? Convert(Type propertyType, string value)
        {
            var tokens = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 1)
            {
                return new Thickness(Int32.Parse(tokens[0]));
            }
            else if (tokens.Length == 2)
            {
                return new Thickness(Int32.Parse(tokens[0]), Int32.Parse(tokens[1]));
            }
            else if (tokens.Length == 4)
            {
                return new Thickness(Int32.Parse(tokens[0]), Int32.Parse(tokens[1]), Int32.Parse(tokens[2]), Int32.Parse(tokens[3]));
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
