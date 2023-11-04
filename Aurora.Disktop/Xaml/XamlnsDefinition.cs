namespace Aurora.Disktop.Xaml
{

    /// <summary>
    /// 定义程序集中Xaml中命名空间
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class XamlnsDefinitionAttribute : Attribute
    {
        public XamlnsDefinitionAttribute(string xmlNamespace, string clrNamespace)
        {
            XmlNamespace = xmlNamespace;
            ClrNamespace = clrNamespace;
        }

        public string ClrNamespace { get; }

        public string XmlNamespace { get; }
    }


    /// <summary>
    /// Xaml 属性转换器属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class XamlConverterAttribute : Attribute
    {
        public XamlConverterAttribute(Type targetType)
        {
            TargetType = targetType;

        }

        public Type TargetType { get; }


    }




}
