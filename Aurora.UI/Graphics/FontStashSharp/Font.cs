

using Aurora.UI.Common.StbTrueType;
using Aurora.UI.Xaml.System;
using static Aurora.UI.Common.StbTrueType.StbTrueType;

namespace Aurora.UI.Graphics.FontStashSharp
{
    internal unsafe class Font
    {
        private readonly Dictionary<ulong, FontGlyph> _glyphs = new Dictionary<ulong, FontGlyph>();

        public stbtt_fontinfo _font = new stbtt_fontinfo();
        public string Name;
        public byte[] Data;
        public float Ascent;
        public float Ascender;
        public float Descender;
        public float LineHeight;

        public Dictionary<ulong, FontGlyph> Glyphs
        {
            get
            {
                return _glyphs;
            }
        }


        public Boolean LoadFont(Byte[] data)
        {
            var ascent = 0;
            var descent = 0;
            var lineGap = 0;
            fixed (byte* ptr = data)
            {
                if (stbtt_InitFont(this._font, ptr, 0) == 0)
                {
                    return false;
                }
            }
            this.GetFontVMetrics(&ascent, &descent, &lineGap);
            var fh = ascent - descent;
            this.Ascent = ascent;
            this.Ascender = ascent / (float)fh;
            this.Descender = descent / (float)fh;
            this.LineHeight = (fh + lineGap) / (float)fh;
            this.Data = data;
            return true;
        }


        public int GetGlyphIndex(int codepoint)
        {
            return stbtt_FindGlyphIndex(this._font, codepoint);
        }

        public float GetPixelHeightScale(float size)
        {
            return (float)stbtt_ScaleForPixelHeight(this._font, (float)size);
        }

        public int BuildGlyphBitmap(int glyph, float size, float scale, int* advance, int* lsb, int* x0, int* y0, int* x1, int* y1)
        {
            stbtt_GetGlyphHMetrics(this._font, glyph, advance, lsb);
            stbtt_GetGlyphBitmapBox(this._font, glyph, (float)scale, (float)scale, x0, y0, x1, y1);
            return 1;
        }

        public void RenderGlyphBitmap(byte* output, int outWidth, int outHeight, int outStride, float scaleX, float scaleY, int glyph)
        {
            stbtt_MakeGlyphBitmap(this._font, output, outWidth, outHeight, outStride, (float)scaleX, (float)scaleY, glyph);
        }

        public void GetFontVMetrics(int* ascent, int* descent, int* lineGap)
        {
            stbtt_GetFontVMetrics(this._font, ascent, descent, lineGap);
        }

        public int GetGlyphKernAdvance(int glyph1, int glyph2)
        {
            return stbtt_GetGlyphKernAdvance(this._font, glyph1, glyph2);
        }

        private ulong BuildKey(int codePoint, int size)
        {
            ulong result = (uint)codePoint << 32;
            result |= (ulong)size << 16;
            return result;
        }

        public bool TryGetGlyph(int codePoint, int size, out FontGlyph glyph)
        {
            var key = BuildKey(codePoint, size);
            return _glyphs.TryGetValue(key, out glyph);
        }

        public void SetGlyph(int codePoint, int size, FontGlyph glyph)
        {
            var key = BuildKey(codePoint, size);
            _glyphs[key] = glyph;
        }
    }
}
