using Aurora.UI.Common;
using Aurora.UI.Controls;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace Aurora.UI.Xaml.System
{
    public class NewClass : IXamlHandler
    {
        void IXamlHandler.Process(Control root, object bindContext, IXamlComponent component, XmlNode node)
        {
            if (node is XmlElement element)
            {
                var _property = element.GetAttribute("Property");
                var _class = element.GetAttribute("Class");
                var typed = Type.GetType(_class);
                if (typed== null)
                {
                    throw new Exception($"Unknown Type : {_class}");
                }
                var _object = Activator.CreateInstance(typed);
                if (!this.SetPropertyValue(component, _property, _object))
                {
                    Trace.WriteLine($"Unknown NewClass Property: {_property} Class: {_class}");
                }
            }
        }



        private Boolean SetPropertyValue(IXamlComponent component, String propertyName, Object value)
        {
            var type = component.GetType();
            // 字段绑定
            var fieldInfo = type.GetField(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo != null)
            {
                if (fieldInfo.FieldType != null)
                {
                    fieldInfo.SetValue(component, value);
                    return true;
                }
         
            }
            // 属性赋值
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo != null)
            {
                if (propertyInfo.PropertyType != null)
                {
                    propertyInfo.SetValue(component, value, null);
                    return true;
                }
            }
            return false;
        }








    }










}
