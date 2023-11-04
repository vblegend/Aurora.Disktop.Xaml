using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Disktop.Xaml.Converters
{


    [XamlConverter(typeof(Object))]
    internal class ObjectConverter : IXamlPropertyConverter
    {
        public object? Convert(Type propertyType, string value)
        {
            return value;
        }
    }
}
