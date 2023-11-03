using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
