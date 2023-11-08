using Aurora.Disktop.Controls;
using System.Diagnostics;
using System.Xml;
using Aurora.Disktop.Common;
using Microsoft.Xna.Framework;
using Aurora.Disktop.Xaml.Converters;
using Aurora.Disktop.Graphics;
using System.Reflection;
using System.Xml.Linq;
using System.Resources;
using System.ComponentModel;
using System;

namespace Aurora.Disktop.Xaml
{
    public class XamlUIParser
    {
        private Object bindContext;
        private Type bindContextType;
        private IXamlComponent Host;
        private XmlDocument doc = new XmlDocument();
        private readonly XamlTypedResolver typedResolver;


        public XamlUIParser(IXamlComponent form, Object bindContext)
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
            this.TryParseNodeResource(this.Host, doc.DocumentElement);
            foreach (XmlElement element in doc.DocumentElement.ChildNodes)
            {
                this.internalParse(this.Host, element);
            }
        }


        private void TryParseNodeResource(IXamlComponent component, XmlElement element)
        {
            var resources = element.GetElementsByTagName($"{element.LocalName}.Resource");
            if (resources.Count == 1)
            {
                this.ParsePropertys(this.Host, resources.Item(0) as XmlElement);
                if (resources.Item(0) is XmlElement resElement)
                {
                    foreach (XmlElement ele in resElement.ChildNodes)
                    {
                        this.internalParse(this.Host, ele);
                    }
                }
            }
        }


        private IXamlComponent GetProperty(IXamlComponent component, String proName)
        {
            var type = component.GetType();
            // 字段绑定
            var fieldInfo = type.GetField(proName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(component) as IXamlComponent;
            }

            // 属性赋值
            var propertyInfo = type.GetProperty(proName, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(component) as IXamlComponent;
            }
            return null;
        }


        private void internalParse(IXamlComponent parent, XmlElement element)
        {
            if (element.LocalName.Contains("."))
            {
                var tokens = element.LocalName.Split(".", StringSplitOptions.RemoveEmptyEntries);
                if (tokens[0] != element.ParentNode.LocalName)
                {
                    throw new Exception("");
                }
                var @object = this.GetProperty(parent, tokens[1]);
                if (@object != null)
                {
                    this.ParsePropertys(@object, element);
                }
                return;
            }

            var typedName = element.Name;
            // Control 抽象接口， 这里返回接口处理组件或其他模块
            IXamlComponent component = GenerateComponent(element.LocalName, element.NamespaceURI);
            if (component is IXamlHandler pipeline)
            {
                pipeline.Process(this.Host as Control, this.bindContext, parent, element);
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


        private void ParsePropertys(IXamlComponent component, XmlElement element)
        {
            foreach (XmlAttribute attribute in element.Attributes)
            {
                var namespaceUrl = attribute.NamespaceURI;
                if (String.IsNullOrEmpty(namespaceUrl))
                {
                    this.ParseCommonAttribute(component, attribute);
                }
                else
                {
                    var typed = this.typedResolver.ResolveXamlComponent(namespaceUrl, attribute.LocalName);
                    if (typed != null)
                    {
                        var comp = (IXamlComponent)Activator.CreateInstance(typed);
                        if (comp is IXamlHandler handler)
                        {
                            handler.Process(this.Host as Control, this.bindContext, comp, attribute);
                        }
                        else
                        {
                            Trace.WriteLine($"Unknown Namespace: {namespaceUrl} Property: {attribute.LocalName}");
                        }
                    }
                }
            }
        }


        private void ParseCommonAttribute(IXamlComponent component, XmlAttribute attribute)
        {
            var type = component.GetType();
            // 事件绑定
            var eventInfo = type.GetEvent(attribute.LocalName, BindingFlags.Instance | BindingFlags.Public);
            if (eventInfo != null)
            {
                var methodInfo = this.bindContextType.GetMethod(attribute.Value);
                if (eventInfo != null && eventInfo.EventHandlerType != null && methodInfo != null)
                {
                    // 创建委托并绑定事件处理程序
                    Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this.bindContext, methodInfo);
                    eventInfo.AddEventHandler(component, handler);
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
                        fieldInfo.SetValue(component, value);
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
                        propertyInfo.SetValue(component, value, null);
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
