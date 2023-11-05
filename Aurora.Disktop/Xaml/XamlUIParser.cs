using Aurora.Disktop.Controls;
using System.Diagnostics;
using System.Xml;
using Aurora.Disktop.Common;
using Microsoft.Xna.Framework;
using Aurora.Disktop.Xaml.Converters;
using Aurora.Disktop.Graphics;
using System.Reflection;
using System.Xml.Linq;

namespace Aurora.Disktop.Xaml
{
    public class XamlUIParser
    {
        private Object bindContext;
        private Type bindContextType;
        private Control Host;
        private XmlDocument doc = new XmlDocument();
        private readonly XamlTypedResolver typedResolver;


        public XamlUIParser(Control form, Object bindContext)
        {
            this.typedResolver = new XamlTypedResolver();
            this.Host = form;
            this.bindContext = bindContext;
            this.bindContextType = bindContext.GetType();
        }







        public void Parse(string xamlContent)
        {
            doc.RemoveAll();
            doc.LoadXml(xamlContent);
            if (doc.DocumentElement == null || doc.DocumentElement.Name != "Template")
            {
                throw new Exception("");
            }

            this.parseNodeResource(this.Host, doc.DocumentElement);

            foreach (XmlElement element in doc.DocumentElement.ChildNodes)
            {
                this.internalParse(this.Host, element);
            }
        }


        private void parseNodeResource(Control control, XmlElement element)
        {
            var resources = element.GetElementsByTagName($"{element.LocalName}.Resource");
            if (resources.Count == 1)
            {
                this.ParsePropertys(this.Host, resources.Item(0) as XmlElement);
            }
        }


        private void internalParse(Control parent, XmlElement element)
        {
            if (element.LocalName.Contains(".")) return;
            var typedName = element.Name;
            // Control 抽象接口， 这里返回接口处理组件或其他模块
            IXamlComponent component = GenerateComponent(element.LocalName, element.NamespaceURI);
            if (component is IXamlHandler pipeline)
            {
                pipeline.Process(this.Host, this.bindContext, parent, element);
            }
            else if (component is Control control)
            {
                // 属性
                this.ParsePropertys(control, element);
                if (parent is IPanelControl panel)
                {
                    panel.Add(control);
                    // Children
                    foreach (XmlElement item in element.ChildNodes)
                    {
                        this.internalParse(control, item);
                    }
                }
                else if (parent is ContentControl contentControl)
                {
                    if (contentControl.Content != null)
                    {
                        throw new Exception("Content 只能有一个");
                    }
                    contentControl.Content = control;
                    // Children
                    foreach (XmlElement item in element.ChildNodes)
                    {
                        this.internalParse(control, item);
                    }
                }
            }
            else
            {
                Trace.WriteLine($"Unknown component type {typedName}");
            }
        }


        private void ParsePropertys(Control control, XmlElement element)
        {
            foreach (XmlAttribute attribute in element.Attributes)
            {
                var namespaceUrl = attribute.NamespaceURI;
                if (String.IsNullOrEmpty(namespaceUrl))
                {
                    this.ParseCommonAttribute(control, attribute);
                }
                else
                {
                    var typed = this.typedResolver.ResolveXamlComponent(namespaceUrl, attribute.LocalName);
                    if (typed != null)
                    {
                        var component = (IXamlComponent)Activator.CreateInstance(typed);
                        if (component is IXamlHandler handler)
                        {
                            handler.Process(this.Host, this.bindContext, control, attribute);
                        }
                        else
                        {
                            Trace.WriteLine($"Unknown Namespace: {namespaceUrl} Property: {attribute.LocalName}");
                        }
                    }
                }
            }
        }


        private void ParseCommonAttribute(Control control, XmlAttribute attribute)
        {
            var type = control.GetType();
            // 事件绑定
            var eventInfo = type.GetEvent(attribute.LocalName, BindingFlags.Instance | BindingFlags.Public);
            if (eventInfo != null)
            {
                var methodInfo = this.bindContextType.GetMethod(attribute.Value);
                if (eventInfo != null && eventInfo.EventHandlerType != null && methodInfo != null)
                {
                    // 创建委托并绑定事件处理程序
                    Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this.bindContext, methodInfo);
                    eventInfo.AddEventHandler(control, handler);
                }
                return;
            }

            // 字段绑定
            var fieldInfo = type.GetField(attribute.LocalName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo != null)
            {
                if (fieldInfo.FieldType != null)
                {
                    var converter = this.typedResolver.ResolveXamlConverter(fieldInfo.FieldType);
                    if (converter != null)
                    {
                        var value = converter.Convert(fieldInfo.FieldType, attribute.Value);
                        fieldInfo.SetValue(control, value);
                    }
                }
                return;
            }



            // 属性赋值
            var propertyInfo = type.GetProperty(attribute.LocalName, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo != null)
            {
                if (propertyInfo.PropertyType != null)
                {
                    var converter = this.typedResolver.ResolveXamlConverter(propertyInfo.PropertyType);
                    if (converter != null)
                    {
                        var value = converter.Convert(propertyInfo.PropertyType, attribute.Value);
                        propertyInfo.SetValue(control, value, null);
                    }
                }
                return;
            }


            Trace.WriteLine($"Unknown Namespace: {attribute.NamespaceURI} Property: {attribute.LocalName}");
        }

        private IXamlComponent GenerateComponent(String typeName, String namespaceURI)
        {
            var typed = this.typedResolver.ResolveXamlComponent(namespaceURI, typeName);
            if (typed != null && typed.GetConstructor(Array.Empty<Type>()) != null)
            {
                return (IXamlComponent)Activator.CreateInstance(typed);
            }
            return null;
        }

    }
}

static class Utils
{

    public static Point ParsePoint(String value)
    {
        if (value == null) throw new Exception();
        var arr = value.Split(",", StringSplitOptions.RemoveEmptyEntries);
        return new Point(Int32.Parse(arr[0]), Int32.Parse(arr[1]));
    }


}
