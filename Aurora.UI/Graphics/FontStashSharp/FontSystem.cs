using Aurora.UI.Common.StbTrueType;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Aurora.UI.Common.StbTrueType.StbTrueType;

namespace Aurora.UI.Graphics.FontStashSharp
{
    public unsafe class FontSystem
    {
        private FontSystemParams _params_ = new FontSystemParams();
        private readonly List<FontAtlas> _atlases = new List<FontAtlas>();
        private readonly Dictionary<String, Font> _fonts = new Dictionary<String, Font>();
        private float _ith;
        private float _itw;
        private readonly List<RenderItem> _renderItems = new List<RenderItem>();
        private FontAtlas _currentAtlas;
        private Font Font;


        public Alignment Alignment;
        public float BlurValue;
        public float Spacing;
        public Vector2 Scale;



        public String DefaultFont = "";
        public char DefaultChar = '?';
        public FontAtlas CurrentAtlas
        {
            get
            {
                if (_currentAtlas == null)
                {
                    _currentAtlas = new FontAtlas(_params_.Width, _params_.Height, 256, _atlases.Count);
                    _atlases.Add(_currentAtlas);
                }

                return _currentAtlas;
            }
        }

        public List<FontAtlas> Atlases
        {
            get
            {
                return _atlases;
            }
        }

        public event EventHandler CurrentAtlasFull;

        public FontSystem(FontSystemParams p)
        {
            _params_ = p;
            _itw = 1.0f / _params_.Width;
            _ith = 1.0f / _params_.Height;
            ClearState();
        }

        public void ClearState()
        {
            BlurValue = 0;
            Spacing = 0;
            Alignment = Alignment.Top;
        }

        public int AddFontMem(string name, byte[] data)
        {
            var ascent = 0;
            var descent = 0;
            var fh = 0;
            var lineGap = 0;
            var font = new Font
            {
                Name = name,
                Data = data
            };
            fixed (byte* ptr = data)
            {
                if (LoadFont(font._font, ptr, data.Length) == 0)
                    return -1;
            }

            font._font.fons__tt_getFontVMetrics(&ascent, &descent, &lineGap);
            fh = ascent - descent;
            font.Ascent = ascent;
            font.Ascender = ascent / (float)fh;
            font.Descender = descent / (float)fh;
            font.LineHeight = (fh + lineGap) / (float)fh;
            _fonts.Add(name, font);
            if (String.IsNullOrEmpty(this.DefaultFont)) this.DefaultFont = name;
            return _fonts.Count - 1;
        }

        private Font GetFontByName(string name)
        {
            if (_fonts.TryGetValue(name, out var font)) return font;
            if (_fonts.TryGetValue(this.DefaultFont, out var font2)) return font2;
            throw new Exception("");
        }

        internal float DrawText(SpriteBatch batch, float x, float y, String str, float fontSize, Color color, Vector2 vScale)
        {
            if (String.IsNullOrEmpty(str)) return 0.0f;
            FontGlyph glyph = null;
            var q = new FontGlyphSquad();
            var prevGlyphIndex = -1;
            var isize = (int)(fontSize * 10.0f);
            var iblur = (int)BlurValue;
            float scale = 0;
            Font font = this.Font;
            float width = 0;
            if (font.Data == null) return x;
            scale = font._font.fons__tt_getPixelHeightScale(isize / 10.0f);
            if ((Alignment & Alignment.Left) != 0)
            {
            }
            else if ((Alignment & Alignment.Right) != 0)
            {
                var bounds = new Bounds();
                width = TextBounds(x, y, str, fontSize, ref bounds);
                x -= width;
            }
            else if ((Alignment & Alignment.Center) != 0)
            {
                var bounds = new Bounds();
                width = TextBounds(x, y, str, fontSize, ref bounds);
                x -= width * 0.5f;
            }

            float originX = 0.0f;
            float originY = 0.0f;

            originY += GetVertAlign(font, Alignment, isize);
            for (int i = 0; i < str.Length; i += char.IsSurrogatePair(str, i) ? 2 : 1)
            {
                var codepoint = char.ConvertToUtf32(str, i);
                glyph = GetGlyph(font, codepoint, isize, iblur, true);
                if (glyph != null)
                {
                    GetQuad(font, prevGlyphIndex, glyph, scale, Spacing, ref originX, ref originY, &q);

                    q.X0 = (int)(q.X0 * vScale.X);
                    q.X1 = (int)(q.X1 * vScale.X);
                    q.Y0 = (int)(q.Y0 * vScale.Y);
                    q.Y1 = (int)(q.Y1 * vScale.Y);

                    var renderItem = new RenderItem
                    {
                        Atlas = _atlases[glyph.AtlasIndex],
                        _verts = new Rectangle((int)(x + q.X0),
                                (int)(y + q.Y0),
                                (int)(q.X1 - q.X0),
                                (int)(q.Y1 - q.Y0)),
                        _textureCoords = new Rectangle((int)(q.S0 * _params_.Width),
                                (int)(q.T0 * _params_.Height),
                                (int)((q.S1 - q.S0) * _params_.Width),
                                (int)((q.T1 - q.T0) * _params_.Height)),
                        _colors = color
                    };

                    _renderItems.Add(renderItem);
                }

                prevGlyphIndex = glyph != null ? glyph.Index : -1;
            }

            Flush(batch, 0.0f);
            return x;
        }

        public float DrawString(SpriteBatch batch, string font, float fontSize, string text, Vector2 pos, Color color)
        {
            return DrawString(batch, font, fontSize, text, pos, color, Vector2.One);
        }

        public float DrawString(SpriteBatch batch, string font, float fontSize, string text, Vector2 pos, Color color, Vector2 scale)
        {
            this.Font = GetFontByName(font);
            return DrawText(batch, pos.X, pos.Y, text, fontSize, color, scale);
        }

        public Vector2 MeasureString(string font, float fontSize, string text)
        {
            Bounds bounds = new Bounds();
            this.Font = GetFontByName(font);
            this.TextBounds(0, 0, text, fontSize, ref bounds);
            return new Vector2(bounds.X2, bounds.Y2);
        }

        public Rectangle GetTextBounds(string font, float fontSize, Vector2 position, string text)
        {
            Bounds bounds = new Bounds();
            this.Font = GetFontByName(font);
            this.TextBounds(position.X, position.Y, text, fontSize, ref bounds);
            return new Rectangle((int)bounds.X, (int)bounds.Y, (int)(bounds.X2 - bounds.X), (int)(bounds.Y2 - bounds.Y));
        }

        internal float TextBounds(float x, float y, String str,float fontSize, ref Bounds bounds)
        {
            if (String.IsNullOrEmpty(str)) return 0.0f;
            var q = new FontGlyphSquad();
            FontGlyph glyph = null;
            var prevGlyphIndex = -1;
            var isize = (int)(fontSize * 10.0f);
            var iblur = (int)BlurValue;
            float scale = 0;
            Font font = this.Font;
            float startx = 0;
            float advance = 0;
            float minx = 0;
            float miny = 0;
            float maxx = 0;
            float maxy = 0;
            if (font.Data == null) return 0;
            scale = font._font.fons__tt_getPixelHeightScale(isize / 10.0f);
            y += GetVertAlign(font, Alignment, isize);
            minx = maxx = x;
            miny = maxy = y;
            startx = x;
            for (int i = 0; i < str.Length; i += char.IsSurrogatePair(str, i) ? 2 : 1)
            {
                var codepoint = char.ConvertToUtf32(str, i);
                glyph = GetGlyph(font, codepoint, isize, iblur, false);
                if (glyph != null)
                {
                    GetQuad(font, prevGlyphIndex, glyph, scale, Spacing, ref x, ref y, &q);
                    if (q.X0 < minx)
                        minx = q.X0;
                    if (x > maxx)
                        maxx = x;
                    if (_params_.IsAlignmentTopLeft)
                    {
                        if (q.Y0 < miny)
                            miny = q.Y0;
                        if (q.Y1 > maxy)
                            maxy = q.Y1;
                    }
                    else
                    {
                        if (q.Y1 < miny)
                            miny = q.Y1;
                        if (q.Y0 > maxy)
                            maxy = q.Y0;
                    }
                }

                prevGlyphIndex = glyph != null ? glyph.Index : -1;
            }

            advance = x - startx;
            if ((Alignment & Alignment.Left) != 0)
            {
            }
            else if ((Alignment & Alignment.Right) != 0)
            {
                minx -= advance;
                maxx -= advance;
            }
            else if ((Alignment & Alignment.Center) != 0)
            {
                minx -= advance * 0.5f;
                maxx -= advance * 0.5f;
            }

            bounds.X = minx;
            bounds.Y = miny;
            bounds.X2 = maxx;
            bounds.Y2 = maxy;

            return advance;
        }

        public void VertMetrics(float fontSize, out float ascender, out float descender, out float lineh)
        {
            ascender = descender = lineh = 0;
            Font font = this.Font;
            int isize = 0;
            isize = (int)(fontSize * 10.0f);
            if (font.Data == null)
                return;

            ascender = font.Ascender * isize / 10.0f;
            descender = font.Descender * isize / 10.0f;
            lineh = font.LineHeight * isize / 10.0f;
        }

        public void LineBounds(float y, float fontSize, ref float miny, ref float maxy)
        {
            Font font = this.Font;
            int isize = 0;
            isize = (int)(fontSize * 10.0f);
            if (font.Data == null)
                return;
            y += GetVertAlign(font, Alignment, isize);
            if (_params_.IsAlignmentTopLeft)
            {
                miny = y - font.Ascender * isize / 10.0f;
                maxy = miny + font.LineHeight * isize / 10.0f;
            }
            else
            {
                maxy = y + font.Descender * isize / 10.0f;
                miny = maxy - font.LineHeight * isize / 10.0f;
            }
        }

        public void Reset(int width, int height)
        {
            _atlases.Clear();
            foreach (var item in _fonts)
            {
                item.Value.Glyphs.Clear();
            }
            if (width == _params_.Width && height == _params_.Height)
                return;

            _params_.Width = width;
            _params_.Height = height;
            _itw = 1.0f / _params_.Width;
            _ith = 1.0f / _params_.Height;
        }

        public void Reset()
        {
            Reset(_params_.Width, _params_.Height);
        }

        private int LoadFont(stbtt_fontinfo font, byte* data, int dataSize)
        {
            return stbtt_InitFont(font, data, 0);
        }

        private FontGlyph GetGlyph(Font font, int codepoint, int isize, int iblur, bool isBitmapRequired)
        {
            var g = 0;
            var advance = 0;
            var lsb = 0;
            var x0 = 0;
            var y0 = 0;
            var x1 = 0;
            var y1 = 0;
            var gw = 0;
            var gh = 0;
            var gx = 0;
            var gy = 0;
            float scale = 0;
            FontGlyph glyph = null;
            var size = isize / 10.0f;
            if (isize < 2)
                return null;
            if (iblur > 20)
                iblur = 20;

            if (font.TryGetGlyph(codepoint, isize, iblur, out glyph))
            {
                if (!isBitmapRequired || glyph.X0 >= 0 && glyph.Y0 >= 0)
                    return glyph;

            }

            g = font._font.fons__tt_getGlyphIndex(codepoint);
            if (g == 0) g = font._font.fons__tt_getGlyphIndex(DefaultChar);
            if (g == 0)
            {
                throw new Exception(string.Format("Could not find glyph for codepoint {0}", codepoint));
            }

            scale = font._font.fons__tt_getPixelHeightScale(size);
            font._font.fons__tt_buildGlyphBitmap(g, size, scale, &advance, &lsb, &x0, &y0, &x1, &y1);

            var pad = FontGlyph.PadFromBlur(iblur);
            gw = x1 - x0 + pad * 2;
            gh = y1 - y0 + pad * 2;

            var currentAtlas = CurrentAtlas;
            if (isBitmapRequired)
            {
                if (!currentAtlas.AddRect(gw, gh, ref gx, ref gy))
                {
                    var ev = CurrentAtlasFull;
                    if (ev != null)
                    {
                        ev(this, EventArgs.Empty);
                    }

                    // This code will force creation of new atlas
                    _currentAtlas = null;
                    currentAtlas = CurrentAtlas;

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
                glyph = new FontGlyph
                {
                    Codepoint = codepoint,
                    Size = isize,
                    Blur = iblur
                };

                font.SetGlyph(codepoint, isize, iblur, glyph);
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

        private void GetQuad(Font font, int prevGlyphIndex, FontGlyph glyph, float scale,
            float spacing, ref float x, ref float y, FontGlyphSquad* q)
        {
            if (prevGlyphIndex != -1)
            {
                var adv = font._font.fons__tt_getGlyphKernAdvance(prevGlyphIndex, glyph.Index) * scale;
                x += (int)(adv + spacing + 0.5f);
            }

            float rx = 0;
            float ry = 0;

            if (_params_.IsAlignmentTopLeft)
            {
                rx = x + glyph.XOffset;
                ry = y + glyph.YOffset;
                q->X0 = rx;
                q->Y0 = ry;
                q->X1 = rx + (glyph.X1 - glyph.X0);
                q->Y1 = ry + (glyph.Y1 - glyph.Y0);
                q->S0 = glyph.X0 * _itw;
                q->T0 = glyph.Y0 * _ith;
                q->S1 = glyph.X1 * _itw;
                q->T1 = glyph.Y1 * _ith;
            }
            else
            {
                rx = x + glyph.XOffset;
                ry = y - glyph.YOffset;
                q->X0 = rx;
                q->Y0 = ry;
                q->X1 = rx + (glyph.X1 - glyph.X0);
                q->Y1 = ry - (glyph.Y1 + glyph.Y0);
                q->S0 = glyph.X0 * _itw;
                q->T0 = glyph.Y0 * _ith;
                q->S1 = glyph.X1 * _itw;
                q->T1 = glyph.Y1 * _ith;
            }

            x += (int)(glyph.XAdvance / 10.0f + 0.5f);
        }

        private void Flush(SpriteBatch batch, float depth)
        {
            foreach (var atlas in _atlases)
            {
                atlas.Flush(batch.GraphicsDevice);
            }

            for (var i = 0; i < _renderItems.Count; ++i)
            {
                var renderItem = _renderItems[i];
                batch.Draw(renderItem.Atlas.Texture,
                    renderItem._verts,
                    renderItem._textureCoords,
                    renderItem._colors,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    depth);
            }

            _renderItems.Clear();
        }

        private float GetVertAlign(Font font, Alignment align, int isize)
        {
            float result = 0.0f; ;
            if (_params_.IsAlignmentTopLeft)
            {
                if ((align & Alignment.Top) != 0)
                {
                    result = font.Ascender * isize / 10.0f;
                }
                else if ((align & Alignment.Middle) != 0)
                {
                    result = (font.Ascender + font.Descender) / 2.0f * isize / 10.0f;
                }
                else if ((align & Alignment.Baseline) != 0)
                {
                }
                else if ((align & Alignment.Bottom) != 0)
                {
                    result = font.Descender * isize / 10.0f;
                }
            }
            else
            {
                if ((align & Alignment.Top) != 0)
                {
                    result = -font.Ascender * isize / 10.0f;
                }
                else
                if ((align & Alignment.Middle) != 0)
                {
                    result = -(font.Ascender + font.Descender) / 2.0f * isize / 10.0f;
                }
                else
                if ((align & Alignment.Baseline) != 0)
                {
                }
                else if ((align & Alignment.Bottom) != 0)
                {
                    result = -font.Descender * isize / 10.0f;
                }
            }

            return result;
        }
    }
}
