using Microsoft.Xna.Framework;
namespace Aurora.UI
{
    public static class ColorExtends
    {

        // #FFFFFFFF
        // #FFFFFF
        public static Color FromHtml(string htmlColor)
        {
            if ((htmlColor[0] == '#') && ((htmlColor.Length == 7) || (htmlColor.Length == 9)))
            {
                var A = 255;
                var R = 0;
                var G = 0;
                var B = 0;
                if (htmlColor.Length == 7)
                {
                    R = (Byte)Convert.ToInt32(htmlColor.Substring(1, 2), 16);
                    G = (Byte)Convert.ToInt32(htmlColor.Substring(3, 2), 16);
                    B = (Byte)Convert.ToInt32(htmlColor.Substring(5, 2), 16);
                }
                else
                {
                    A = (Byte)Convert.ToInt32(htmlColor.Substring(1, 2), 16);
                    R = (Byte)Convert.ToInt32(htmlColor.Substring(3, 2), 16);
                    G = (Byte)Convert.ToInt32(htmlColor.Substring(5, 2), 16);
                    B = (Byte)Convert.ToInt32(htmlColor.Substring(7, 2), 16);
                }
                return new Color(R, G, B, A);
            }
            else
            {
                throw new Exception();
            }

        }



    }
}
