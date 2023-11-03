using Aurora.Disktop.Common;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Aurora.Disktop.Xaml
{
    internal class NameMaping
    {
        public NameMaping(string xmlns)
        {
            this.xmlns = xmlns;
        }
        public readonly string xmlns;
        internal readonly List<string> ClrNamespaces = new List<string>();
    }



    internal class ModuleMain
    {
        // 可用的类型列表
        private static Dictionary<string, NameMaping> xmlnsMap = new Dictionary<string, NameMaping>();
        private static Dictionary<string, Type> componentMap = new Dictionary<string, Type>();


        [ModuleInitializer]
        public static void Initialize()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) ParseAssembly(assembly);
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private static void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args)
        {
            ParseAssembly(args.LoadedAssembly);
        }

        private static void ParseAssembly(Assembly assembly)
        {
            var attributes = assembly.GetCustomAttributes<XamlNamespaceDefinitionAttribute>();
            foreach (var xmlns in attributes)
            {
                if (!xmlnsMap.TryGetValue(xmlns.XmlNamespace, out var xmlnsMaping))
                {
                    xmlnsMaping = new NameMaping(xmlns.XmlNamespace);
                    xmlnsMap.Add(xmlns.XmlNamespace, xmlnsMaping);
                }
                if (!xmlnsMaping.ClrNamespaces.Contains(xmlns.ClrNamespace))
                {
                    xmlnsMaping.ClrNamespaces.Add(xmlns.ClrNamespace);
                }
            }
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsPublic && type.IsVisible && !type.IsValueType && type.IsClass && type.GetInterface(typeof(IXamlComponent).Name) != null)
                {
                    componentMap.Add(type.Namespace + "." + type.Name, type);
                }
            }
        }


        public static Type? ResolvingType(string xmlns, string type)
        {
            if (xmlnsMap.TryGetValue(xmlns, out var map))
            {
                foreach (var clrns in map.ClrNamespaces)
                {
                    if (componentMap.TryGetValue($"{clrns}.{type}", out var value))
                    {
                        return value;
                    }
                }
            }
            return null;
        }
    }

}
