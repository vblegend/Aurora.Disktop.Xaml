using Aurora.Disktop.Common;
using Aurora.Disktop.Controls;
using System.Xml;

namespace Aurora.Disktop.Xaml.System
{
    public class Package : IXamlHandler
    {
        void IXamlHandler.Process(Control root, object bindContext, Control xamlControl, XmlNode node)
        {
            if (node is XmlElement element)
            {
                var name = element.GetAttribute("Name");
                var path = element.GetAttribute("Path");
                var pack = ResourcePackage.Open(path);
                AuroraState.PackageManager.Register(name, pack);
            }
        }
    }
}
