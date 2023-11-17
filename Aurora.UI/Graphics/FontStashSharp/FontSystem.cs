using Aurora.UI.Xaml.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Aurora.UI.Common.StbTrueType.StbTrueType;


namespace Aurora.UI.Graphics.FontStashSharp
{

    internal struct PrevGlyphInfo
    {
        public PrevGlyphInfo()
        {
            Font = null;
            Index = -1;
        }

        public Font Font;
        public int Index;

    }
    public unsafe class FontSystem
    {
        private Int32 BufferWidth;
        private Int32 BufferHeight;
        private float _ith;
        private float _itw;


        private readonly List<FontAtlas> _atlases = new List<FontAtlas>();
        private readonly Dictionary<String, Font> _fonts = new Dictionary<String, Font>();


        private readonly Dictionary<String, Font[]> _fontcollection = new Dictionary<String, Font[]>();



        private readonly List<RenderItem> _renderItems = new List<RenderItem>();
        private FontAtlas _currentAtlas;


        public String DefaultFont = "";
        public char DefaultChar = ' '; // \u2593


        public FontSystem(Int32 bufferWidth, Int32 bufferHeight)
        {
            this.BufferWidth = bufferWidth;
            this.BufferHeight = bufferHeight;
            _itw = 1.0f / this.BufferWidth;
            _ith = 1.0f / this.BufferHeight;
        }


        private FontAtlas CreateAtlas()
        {
            _currentAtlas = new FontAtlas(this.BufferWidth, this.BufferHeight, 256, _atlases.Count);
            _atlases.Add(_currentAtlas);
            return _currentAtlas;
        }

        public int AddFontMem(string name, byte[] data)
        {
            var font = new Font { Name = name };
            if (!font.LoadFont(data)) return -1;
            _fonts.Add(name, font);
            if (String.IsNullOrEmpty(this.DefaultFont)) this.DefaultFont = name;
            return _fonts.Count - 1;
        }

        private Font[] GetFontByName(string name)
        {
            if (this._fontcollection.TryGetValue(name, out var fontColl)) return fontColl;
            var fonts = name.Split(',', StringSplitOptions.RemoveEmptyEntries);
            List<Font> fontList = new List<Font>();

            for (int i = 0; i < fonts.Length; i++)
            {
                if (_fonts.TryGetValue(fonts[i], out var font)) fontList.Add(font);
            }

            if (fontList.Count == 0)
            {
                if (_fonts.TryGetValue(this.DefaultFont, out var font2))  fontList.Add(font2);
            }
            var result = fontList.ToArray();
            this._fontcollection.Add(name, result);
            return result;
        }

        internal float DrawText(SpriteBatch batch, Font[] fonts, float fontSize, float x, float y, String str, Color color, Vector2 vScale)
        {
            if (String.IsNullOrEmpty(str) || fonts.Length == 0) return x;
            var q = new FontGlyphSquad();
            var prevGlyphIndex = new PrevGlyphInfo();
            var isize = (int)(fontSize * 10.0f);
            var scale = 0.0f;
            float originX = 0.0f;
            float originY = 0.0f;


            for (int i = 0; i < str.Length; i += char.IsSurrogatePair(str, i) ? 2 : 1)
            {
                var codepoint = char.ConvertToUtf32(str, i);
                var glyph = GetGlyph(fonts, codepoint, isize, true);
                if (glyph != null)
                {
                    if (glyph.Font != prevGlyphIndex.Font)
                    {
                        scale = glyph.Font.GetPixelHeightScale(isize / 10.0f);
                        originY = glyph.Font.Ascender * isize / 10.0f;
                    }
                    GetQuad(glyph.Font, prevGlyphIndex, glyph, scale, ref originX, ref originY, &q);
                    q.X0 = (int)(q.X0 * vScale.X);
                    q.X1 = (int)(q.X1 * vScale.X);
                    q.Y0 = (int)(q.Y0 * vScale.Y);
                    q.Y1 = (int)(q.Y1 * vScale.Y);

                    var renderItem = new RenderItem
                    {
                        Atlas = _atlases[glyph.AtlasIndex],
                        _verts = new Rectangle((int)(x + q.X0), (int)(y + q.Y0), (int)(q.X1 - q.X0), (int)(q.Y1 - q.Y0)),
                        _textureCoords = new Rectangle((int)(q.S0 * this.BufferWidth), (int)(q.T0 * this.BufferHeight), (int)((q.S1 - q.S0) * this.BufferWidth), (int)((q.T1 - q.T0) * this.BufferHeight)),
                        _color = color
                    };

                    _renderItems.Add(renderItem);
                }
                prevGlyphIndex.Font = glyph.Font;
                prevGlyphIndex.Index = glyph != null ? glyph.Index : -1;
                //prevGlyphIndex = glyph != null ? glyph.Index : -1;
            }

            Flush(batch);
            return x;
        }

        public float DrawString(SpriteBatch batch, string font, float fontSize, string text, Vector2 pos, Color color)
        {
            var theFont = GetFontByName(font);
            return DrawText(batch, theFont, fontSize, pos.X, pos.Y, text, color, Vector2.One);
        }

        public float DrawString(SpriteBatch batch, string font, float fontSize, string text, Vector2 pos, Color color, Vector2 scale)
        {
            var theFont = GetFontByName(font);
            return DrawText(batch, theFont, fontSize, pos.X, pos.Y, text, color, scale);
        }

        public Vector2 MeasureString(string font, float fontSize, string text)
        {
            Bounds bounds = new Bounds();
            var theFont = GetFontByName(font);
            this.TextBounds(0, 0, theFont, fontSize, text, ref bounds);
            return new Vector2(bounds.X2, bounds.Y2);
        }

        public Rectangle GetTextBounds(string font, float fontSize, Vector2 position, string text)
        {
            Bounds bounds = new Bounds();
            var theFont = GetFontByName(font);
            this.TextBounds(position.X, position.Y, theFont, fontSize, text, ref bounds);
            return new Rectangle((int)bounds.X, (int)bounds.Y, (int)(bounds.X2 - bounds.X), (int)(bounds.Y2 - bounds.Y));
        }

        internal float TextBounds(float x, float y, Font[] fonts, float fontSize, String str, ref Bounds bounds)
        {
            if (String.IsNullOrEmpty(str) || fonts.Length == 0) return 0.0f;
            var q = new FontGlyphSquad();
            var prevGlyphIndex = new PrevGlyphInfo();
            var isize = (int)(fontSize * 10.0f);
            var scale = 0.0f;
            var minx = x;
            var maxx = x;
            var miny = y;
            var maxy = y;
            var startx = x;
            for (int i = 0; i < str.Length; i += char.IsSurrogatePair(str, i) ? 2 : 1)
            {
                var codepoint = char.ConvertToUtf32(str, i);
                var glyph = GetGlyph(fonts, codepoint, isize, false);
                if (glyph != null)
                {

                    if (prevGlyphIndex.Font != glyph.Font)
                    {
                        scale = glyph.Font.GetPixelHeightScale(isize / 10.0f);
                        y = glyph.Font.Ascender * isize / 10.0f;
                    }


                    GetQuad(glyph.Font, prevGlyphIndex, glyph, scale, ref x, ref y, &q);
                    if (q.X0 < minx) minx = q.X0;
                    if (x > maxx) maxx = x;

                    if (q.Y0 < miny) miny = q.Y0;
                    if (q.Y1 > maxy) maxy = q.Y1;

                }
                prevGlyphIndex.Font = glyph.Font;
                prevGlyphIndex.Index = glyph != null ? glyph.Index : -1;
                //prevGlyphIndex = glyph != null ? glyph.Index : -1;
            }
            var advance = x - startx;
            bounds.X = minx;
            bounds.Y = miny;
            bounds.X2 = maxx;
            bounds.Y2 = maxy;

            return advance;
        }

        //public void VertMetrics(String font, float fontSize, out float ascender, out float descender, out float lineh)
        //{
        //    ascender = descender = lineh = 0;
        //    var isize = (int)(fontSize * 10.0f);
        //    var theFont = GetFontByName(font);
        //    if (theFont == null) return;
        //    ascender = theFont.Ascender * isize / 10.0f;
        //    descender = theFont.Descender * isize / 10.0f;
        //    lineh = theFont.LineHeight * isize / 10.0f;
        //}

        //public void LineBounds(String font, float y, float fontSize, ref float miny, ref float maxy)
        //{
        //    var isize = (int)(fontSize * 10.0f);
        //    var theFont = GetFontByName(font);
        //    if (theFont == null) return;
        //    y += theFont.Ascender * isize / 10.0f;
        //    miny = y - theFont.Ascender * isize / 10.0f;
        //    maxy = miny + theFont.LineHeight * isize / 10.0f;
        //}

        public void Reset(int width, int height)
        {
            _atlases.Clear();
            foreach (var item in _fonts)
            {
                item.Value.Glyphs.Clear();
            }
            if (width == this.BufferWidth && height == this.BufferHeight) return;
            this.BufferWidth = width;
            this.BufferHeight = height;
            _itw = 1.0f / width;
            _ith = 1.0f / height;
        }

        public void Reset()
        {
            Reset(this.BufferWidth, this.BufferHeight);
        }

        private int LoadFont(stbtt_fontinfo font, byte* data, int dataSize)
        {
            return stbtt_InitFont(font, data, 0);
        }

        private FontGlyph GetGlyph(Font[] fonts, int codepoint, int isize, bool isBitmapRequired)
        {
            var advance = 0;
            var lsb = 0;
            var x0 = 0;
            var y0 = 0;
            var x1 = 0;
            var y1 = 0;
            var gx = 0;
            var gy = 0;
            FontGlyph glyph = null;
            Font font = null;
            var size = isize / 10.0f;
            if (isize < 2) return null;


            for (int i = 0; i < fonts.Length; i++)
            {
                if (fonts[i].TryGetGlyph(codepoint, isize, out glyph))
                {
                    if (!isBitmapRequired || glyph.X0 >= 0 && glyph.Y0 >= 0) return glyph;
                }
            }
            

            Int32 g = 0;
            for (int i = 0; i < fonts.Length; i++)
            {
                g = fonts[i].GetGlyphIndex(codepoint);
                if (g > 0)
                {
                    font = fonts[i];
                    break;
                }
            }
            if (g == 0)
            {
                for (int i = 0; i < fonts.Length; i++)
                {
                    g = fonts[i].GetGlyphIndex(DefaultChar);
                    if (g > 0)
                    {
                        font = fonts[i];
                        break;
                    }
                }
            }
            if (g == 0)
            {
                throw new Exception(string.Format("Could not find glyph for codepoint {0}", codepoint));
            }

            var scale = font.GetPixelHeightScale(size);
            font.BuildGlyphBitmap(g, size, scale, &advance, &lsb, &x0, &y0, &x1, &y1);

            var pad = 2;
            var gw = x1 - x0 + pad * 2;
            var gh = y1 - y0 + pad * 2;

            if (_currentAtlas == null) this.CreateAtlas();
            var currentAtlas = _currentAtlas;
            if (isBitmapRequired)
            {
                if (!currentAtlas.AddRect(gw, gh, ref gx, ref gy))
                {
                    // This code will force creation of new atlas
                    currentAtlas = CreateAtlas();
                    // Try to add again
                    if (!currentAtlas.AddRect(gw, gh, ref gx, ref gy))
                    {
                        throw new Exception(string.Format("Could not add rect to the newly created atlas. gw={0}, gh={1}", gw, gh));
                    }
                }
            }
            else
            {
                gx = -1;
                gy = -1;
            }

            if (glyph == null)
            {
                glyph = new FontGlyph { Codepoint = codepoint, Size = isize, Font = font };
                font.SetGlyph(codepoint, isize, glyph);
            }

            glyph.Index = g;
            glyph.AtlasIndex = currentAtlas.Index;
            glyph.X0 = gx;
            glyph.Y0 = gy;
            glyph.X1 = glyph.X0 + gw;
            glyph.Y1 = glyph.Y0 + gh;
            glyph.XAdvance = (int)(scale * advance * 10.0f);
            glyph.XOffset = x0 - pad;
            glyph.YOffset = y0 - pad;
            if (!isBitmapRequired) return glyph;

            currentAtlas.RenderGlyph(font, glyph, gw, gh, scale);

            return glyph;
        }

        private void GetQuad(Font font, PrevGlyphInfo prevGlyphIndex, FontGlyph glyph, float scale, ref float x, ref float y, FontGlyphSquad* q)
        {
            if (font == prevGlyphIndex.Font && prevGlyphIndex.Index != -1)
            {
                var adv = font.GetGlyphKernAdvance(prevGlyphIndex.Index, glyph.Index) * scale;
                x += (int)(adv + 0.5f);
            }
            var rx = x + glyph.XOffset;
            var ry = y + glyph.YOffset;
            q->X0 = rx;
            q->Y0 = ry;
            q->X1 = rx + (glyph.X1 - glyph.X0);
            q->Y1 = ry + (glyph.Y1 - glyph.Y0);
            q->S0 = glyph.X0 * _itw;
            q->T0 = glyph.Y0 * _ith;
            q->S1 = glyph.X1 * _itw;
            q->T1 = glyph.Y1 * _ith;
            x += (int)(glyph.XAdvance / 10.0f + 0.5f);
        }

        private void Flush(SpriteBatch batch)
        {
            foreach (var atlas in _atlases)
            {
                atlas.Flush(batch.GraphicsDevice);
            }

            for (var i = 0; i < _renderItems.Count; ++i)
            {
                var renderItem = _renderItems[i];
                batch.Draw(renderItem.Atlas.Texture, renderItem._verts, renderItem._textureCoords, renderItem._color);
            }

            _renderItems.Clear();
        }
    }
}
