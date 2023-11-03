using Microsoft.Xna.Framework;
namespace Aurora.Disktop
{
    public static class ColorExtends
    {

        // #FFFFFFFF
        // #FFFFFF
        public static void FromHtml(this Color color ,  string htmlColor)
        {

            if ((htmlColor[0] == '#') && ((htmlColor.Length == 7) || (htmlColor.Length == 9)))
            {
                if (htmlColor.Length == 7)
                {
                    color.R = (Byte)Convert.ToInt32(htmlColor.Substring(1, 2), 16);
                    color.G = (Byte)Convert.ToInt32(htmlColor.Substring(3, 2), 16);
                    color.B = (Byte)Convert.ToInt32(htmlColor.Substring(5, 2), 16);
                }
                else
                {
                    color.A = (Byte)Convert.ToInt32(htmlColor.Substring(1, 2), 16);
                    color.R = (Byte)Convert.ToInt32(htmlColor.Substring(3, 2), 16);
                    color.G = (Byte)Convert.ToInt32(htmlColor.Substring(5, 2), 16);
                    color.B = (Byte)Convert.ToInt32(htmlColor.Substring(7, 2), 16);
                }
            }
            else
            {
                throw new Exception();
            }

        }



    }
}
