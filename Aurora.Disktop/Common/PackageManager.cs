using SpriteFontPlus;

namespace Aurora.Disktop.Common
{

    public class FontManager
    {
        private DynamicSpriteFont fonts;
        private Dictionary<String, Int32> fontsmap = new Dictionary<String, Int32>();



        private Dictionary<String, DynamicSpriteFont> keyValuePairs = new Dictionary<String, DynamicSpriteFont>();


        public DynamicSpriteFont this[String fontName]
        {
            get
            {
                if (keyValuePairs.ContainsKey(fontName))
                {
                    return keyValuePairs[fontName];
                }
                return null;
            }
        }

        public void Register(String packageName, DynamicSpriteFont package)
        {





            keyValuePairs.Add(packageName, package);
        }


        public Int32 GetFontID(String fontname)
        {
            if (this.fontsmap.TryGetValue(fontname, out var id)) return id;
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
