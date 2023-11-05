using Aurora.Disktop.Common;
using System.Reflection;


namespace Aurora.Disktop.Xaml
{
    internal class XamlTypedResolver
    {
        private class NameMaping
        {
            public NameMaping(String xmlns)
            {
                this.xmlns = xmlns;
            }
            public readonly String xmlns;
            internal readonly List<String> ClrNamespaces = new List<String>();
            internal Dictionary<String, Type> Components = new Dictionary<String, Type>();



            public void AddNamespace(String _namespace)
            {
                if (!this.ClrNamespaces.Contains(_namespace))
                {
                    this.ClrNamespaces.Add(_namespace);
                }
            }

            public Boolean ContainsNamespace(String _namespace)
            {
                return this.ClrNamespaces.Contains(_namespace);
            }

        }


        private Dictionary<Type, IXamlPropertyConverter> XamlConverters { get; set; }
        private Dictionary<String, NameMaping> xmlnsMap = new Dictionary<String, NameMaping>();


        public XamlTypedResolver()
        {
            this.XamlConverters = new Dictionary<Type, IXamlPropertyConverter>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var assemblyXmlns = new Dictionary<String, NameMaping>();
                var attributes = assembly.GetCustomAttributes<XamlnsDefinitionAttribute>();
                foreach (var xmlns in attributes)
                {
                    if (!assemblyXmlns.TryGetValue(xmlns.XmlNamespace, out var xmlnsMaping))
                    {
                        xmlnsMaping = new NameMaping(xmlns.XmlNamespace);
                        assemblyXmlns.Add(xmlns.XmlNamespace, xmlnsMaping);
                    }
                    xmlnsMaping.AddNamespace(xmlns.ClrNamespace);
                }


                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    // XamlConverter
                    var attribute = type.GetCustomAttribute<XamlConverterAttribute>();
                    if (attribute != null && type.IsClass && type.GetInterface(typeof(IXamlPropertyConverter).Name) != null && type.GetConstructor(Type.EmptyTypes) != null)
                    {
                        var converter = (IXamlPropertyConverter)Activator.CreateInstance(type);
                        if (converter != null)
                        {
                            this.XamlConverters.Add(attribute.TargetType, converter);
                        }
                    }
                    // IComponent
                    if (type != null && type.IsPublic && type.IsVisible && !type.IsValueType && type.IsClass &&
                        type.GetInterface(typeof(IXamlComponent).Name) != null)
                    {
                        foreach (var item in assemblyXmlns)
                        {
                            if (item.Value.ClrNamespaces.Contains(type.Namespace))
                            {
                                item.Value.Components[type.Name] = type;
                            }
                        }
                    }
                }
                foreach (var xmlns in assemblyXmlns)
                {
                    xmlnsMap.Add(xmlns.Key, xmlns.Value);
                }
            }
        }

        public Type ResolveXamlComponent(String xmlns, String type)
        {
            if (xmlnsMap.TryGetValue(xmlns, out var map))
            {
                if (map.Components.TryGetValue(type, out var value))
                {
                    return value;
                }
            }
            return null;
        }

        public IXamlPropertyConverter ResolveXamlConverter(Type typed)
        {
            if (typed.IsEnum) typed = typeof(Enum);
            if (this.XamlConverters.TryGetValue(typed, out var value))
            {
                return value;
            }
            return null;
        }


    }
}
