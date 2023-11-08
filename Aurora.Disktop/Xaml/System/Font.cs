﻿using Aurora.Disktop.Common;
using Aurora.Disktop.Controls;
using System.Drawing;
using System.Xml;

namespace Aurora.Disktop.Xaml.System
{
    public class Font : IXamlHandler
    {
        void IXamlHandler.Process(Control root, object bindContext, IXamlComponent component, XmlNode node)
        {
            if (node is XmlElement element)
            {
                var name = element.GetAttribute("Name");
                var path = element.GetAttribute("Path");
                var size = element.GetAttribute("Size");
                if (String.IsNullOrEmpty(name))
                {
                    throw new Exception("font name cannot be empty");
                }
                if (String.IsNullOrEmpty(path))
                {
                    throw new Exception("font path cannot be empty");
                }
                // default size  20
                if (String.IsNullOrEmpty(size)) size = "20";
                var font = TTFFont.FromFile(path, Int32.Parse(size));
                AuroraState.FontManager.Register(name, font);
            }
        }
    }
}
