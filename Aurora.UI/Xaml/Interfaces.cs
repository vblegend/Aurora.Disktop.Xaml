using Aurora.UI.Common;
using Aurora.UI.Controls;
using System;
using System.Xml;

namespace Aurora.UI.Xaml
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
