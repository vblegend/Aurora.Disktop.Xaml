using Aurora.Disktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Disktop.Xaml
{
    internal class XamlTypedResolver
    {
        private Dictionary<Type, IXamlPropertyConverter> XamlConverters { get; set; }



        public XamlTypedResolver()
        {
            this.XamlConverters = new Dictionary<Type, IXamlPropertyConverter>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<XamlConverterAttribute>();
                    if ( attribute != null && type.IsClass && type.GetInterface(typeof(IXamlPropertyConverter).Name) != null && type.GetConstructor(Type.EmptyTypes) != null)
                    {
                        var converter = (IXamlPropertyConverter?)Activator.CreateInstance(type);
                        if (converter != null)
                        {
                            this.XamlConverters.Add(attribute.TargetType, converter);
                        }
                    }
                }
            }
        }


        public IXamlPropertyConverter? ResolveXamlConverter(Type typed)
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
