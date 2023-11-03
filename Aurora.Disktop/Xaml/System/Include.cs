using Aurora.Disktop.Controls;
using System.Xml;

namespace Aurora.Disktop.Xaml.System
{
    public class Include : IXamlHandler
    {
        void IXamlHandler.Process(Control root, object bindContext, Control xamlControl, XmlNode node)
        {
            if(node is XmlElement element)
            {
                var url = element.GetAttribute("Url");
                var p = new XamlUIParser(xamlControl, bindContext);
                p.Parse(File.ReadAllText(url));
            }
        }
    }
}
