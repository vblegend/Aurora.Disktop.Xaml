using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Disktop.Xaml.Converters
{
    internal class IntConverter
    {
    }


    [XamlConverter(typeof(Int64))]
    internal class Int64Converter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            return Int64.Parse(value);
        }
    }

    [XamlConverter(typeof(UInt64))]
    internal class UInt64Converter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            return UInt64.Parse(value);
        }
    }

    [XamlConverter(typeof(Int32))]
    internal class Int32Converter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            return Int32.Parse(value);
        }
    }

    [XamlConverter(typeof(UInt32))]
    internal class UInt32Converter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            return UInt32.Parse(value);
        }
    }


    [XamlConverter(typeof(Int16))]
    internal class Int16Converter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            return Int16.Parse(value);
        }
    }

    [XamlConverter(typeof(UInt16))]
    internal class UInt16Converter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            return UInt16.Parse(value);
        }
    }



    [XamlConverter(typeof(Byte))]
    internal class ByteConverter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            return Byte.Parse(value);
        }
    }

    [XamlConverter(typeof(Char))]
    internal class CharConverter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            return Char.Parse(value);
        }
    }


    [XamlConverter(typeof(Double))]
    internal class DoubleConverter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            return Double.Parse(value);
        }
    }

    [XamlConverter(typeof(Single))]
    internal class SingleConverter : IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, string value)
        {
            if (value == null) throw new Exception();
            return Single.Parse(value);
        }
    }






}
