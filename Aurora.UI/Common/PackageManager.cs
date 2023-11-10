using SpriteFontPlus;

namespace Aurora.UI.Common
{

    public class FontManager
    {
        public DynamicSpriteFont Font { get; private set; }
        private Dictionary<String, Int32> fontsmap = new Dictionary<String, Int32>();

        public void Register(String packageName, Byte[] ttfBinary)
        {
            if (fontsmap.ContainsKey(packageName)) return;
            var fontID = 0;
            if (this.Font == null)
            {
                Font = DynamicSpriteFont.FromTtf(ttfBinary, 5);
            }
            else
            {
                fontID = Font.AddTtf(packageName, ttfBinary);
            }
            fontsmap.Add(packageName, fontID);
        }


        public Int32 GetFontID(String fontname)
        {
            if (String.IsNullOrEmpty(fontname)) return 0;
            if (this.fontsmap.TryGetValue(fontname, out var id))
            {
                return id;
            }
            return 0;
        }
    }




    public class PackageManager
    {
        private Dictionary<String, ResourcePackage> keyValuePairs = new Dictionary<String, ResourcePackage>();


        public ResourcePackage this[String packageName]
        {
            get
            {
                if (keyValuePairs.ContainsKey(packageName))
                {
                    return keyValuePairs[packageName];
                }
                return null;
            }
        }

        public void Register(String packageName, ResourcePackage package)
        {
            keyValuePairs.Add(packageName, package);
        }
    }
}
