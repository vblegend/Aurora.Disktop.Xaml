using Aurora.UI.Common;
using Aurora.UI.Controls;
using System.Xml;

namespace Aurora.UI.Xaml.System
{
    public class Package : IXamlHandler
    {
        void IXamlHandler.Process(Control root, object bindContext, IXamlComponent component, XmlNode node)
        {
            if (node is XmlElement element)
            {
                var name = element.GetAttribute("Name");
                var path = element.GetAttribute("Path");
                var password = element.GetAttribute("Password");
                var pack = ResourcePackage.Open(path, password);
                AuroraState.PackageManager.Register(name, pack);
            }
        }
    }
}
