using Aurora.UI.Common;
using Aurora.UI.Controls;
using System.Xml;

namespace Aurora.UI.Xaml.System
{
    public class Font : IXamlHandler
    {
        void IXamlHandler.Process(Control root, object bindContext, IXamlComponent component, XmlNode node)
        {
            if (node is XmlElement element)
            {
                var name = element.GetAttribute("Name");
                var path = element.GetAttribute("Path");
                if (String.IsNullOrEmpty(name))
                {
                    throw new Exception("font name cannot be empty");
                }
                if (String.IsNullOrEmpty(path))
                {
                    throw new Exception("font path cannot be empty");
                }
                var data = File.ReadAllBytes(path);
                //AuroraState.FontManager.Register(name, data);

                AuroraState.FontSystem.AddFontMem(name, data);
            }
        }
    }
}
