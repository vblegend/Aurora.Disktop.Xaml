using Aurora.Disktop.Common;
using Aurora.Disktop.Controls;
using System;
using System.Xml;

namespace Aurora.Disktop.Xaml
{


    public interface IXamlHandler : IXamlComponent
    {
        public void Process(Control root, Object bindContext, IXamlComponent xamlControl, XmlNode node);
    }



    public interface IXamlPropertyConverter
    {
        public Object Convert(Type propertyType, String value);

    }



    public class InvalidConvertTypeException : Exception
    {
        public InvalidConvertTypeException(String message) : base(message)
        {

        }
    }

}
