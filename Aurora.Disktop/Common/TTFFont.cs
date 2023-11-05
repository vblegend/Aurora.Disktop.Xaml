using SpriteFontPlus;


namespace Aurora.Disktop.Common
{
    public static class TTFFont
    {

        public static DynamicSpriteFont FromFile(string filename, float fontSize)
        {
            var data = File.ReadAllBytes(filename);
            return DynamicSpriteFont.FromTtf(data, fontSize);
        }



    }
}
