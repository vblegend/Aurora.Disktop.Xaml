using Aurora.UI.Common;
using Aurora.UI.Components;
using Aurora.UI.Graphics;
using Aurora.UI.Platforms.Windows;
using Microsoft.Xna.Framework;
using MonoGame.IMEHelper;
using System.Diagnostics;

namespace Aurora.UI.Controls
{
    public struct TextViewPort
    {
        public Int32 Start;
        public Int32 End;
        public Int32 Length()
        {
            return this.End - this.Start;
        }
    }



    public class TextBox : Control
    {
        public TextBox()
        {
            this.maxLength = 100;
            this.Size = new Point(85, 22);
            this.TextColor = Color.White;
            this.ActivedBorder = ColorExtends.FromHtml("#00a2e8");
        }


        //const int UnicodeSimplifiedChineseMin = 0x4E00;
        //const int UnicodeSimplifiedChineseMax = 0x9FA5;
        private readonly TimeSpan CurrsorFlashInterval = new TimeSpan(0, 0, 0, 0, 500);



        protected override void OnUpdate(GameTime gameTime)
        {
            if (this.CurrsorArea.Height != this.FontSize)
            {
                this.CurrsorArea.Height = (Int32)this.FontSize;
                this.CurrsorArea.Width = 2;
            }
            if (gameTime.TotalGameTime - lastCurrsorTime > CurrsorFlashInterval)
            {
                this.currsorVisable = this.IsFocus && !this.currsorVisable;
                lastCurrsorTime = gameTime.TotalGameTime;
            }
            base.OnUpdate(gameTime);
        }

        protected override void OnRender(GameTime gameTime)
        {
            base.OnRender(gameTime);
            var padding = new Point(this.padding.Left, this.padding.Top);
            if (this.finalTexture != null)
            {
                // 显示最终渲染结果
                this.Renderer.Draw(this.finalTexture, this.GlobalBounds.Location.Add(padding), Color.White);
            }

            // 显示光标
            if (this.currsorVisable)
            {
                var text = this._text.Substring(viewport.Start, this.cursorPosition - viewport.Start);
                var lr = this.MeasureStringWidth(text);
                this.Renderer.FillRectangle(this.CurrsorArea.Add(this.GlobalBounds.Location).Add(new Point(lr, 0)).Add(padding), this.CursorColor);
            }
            // 焦点突出显示边框
            if (this.IsFocus) this.Renderer.DrawRectangle(this.GlobalBounds, this.ActivedBorder);
        }

        private void ImeHandler_TextInput(object sender, MonoGame.IMEHelper.TextInputEventArgs e)
        {
            switch ((int)e.Character)
            {
                case 8:
                    // 退格
                    this.RemoveString(-1);
                    this.InvalidateDrawing();
                    break;
                case 9:
                    AppendString(this.TabChar);
                    this.InvalidateDrawing();
                    break;
                case 127:
                    // 退格
                    this.RemoveString(1);
                    this.InvalidateDrawing();
                    break;
                case 27: // Esc
                case 13: // Enter
                    break;
                default:
                    // 输入
                    var inputChar = e.Character;
                    AppendString(inputChar.ToString());
                    this.InvalidateDrawing();
                    break;
            }
        }


        protected override void OnKeyDown(IKeyboardMessage args)
        {
            switch (args.Key)
            {
                case Microsoft.Xna.Framework.Input.Keys.Left:
                    if (this.cursorPosition > 0) this.cursorPosition--;
                    if (this.cursorPosition < this.viewport.Start)
                    {
                        this.viewport.Start--;
                        this.MeasureViewportStart();
                        this.InvalidateDrawing();
                    }
                    this.ActiveCurrsor();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Right:
                    if (this.cursorPosition < this._text.Length) this.cursorPosition++;
                    if (this.cursorPosition > this.viewport.End)
                    {
                        this.viewport.End++;
                        this.MeasureViewportEnd();
                        this.InvalidateDrawing();
                    }
                    this.ActiveCurrsor();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Home:
                    if (this.cursorPosition > 0) this.cursorPosition = 0;

                    this.viewport.Start = 0;
                    this.MeasureViewportStart();
                    this.InvalidateDrawing();

                    this.ActiveCurrsor();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.End:
                    this.cursorPosition = this._text.Length;
                    this.viewport.End = this._text.Length;
                    this.MeasureViewportEnd();
                    this.InvalidateDrawing();

                    this.ActiveCurrsor();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.A:
                    if (args.Ctrl)
                    {
                        Trace.WriteLine("全选");
                    }
                    break;

                case Microsoft.Xna.Framework.Input.Keys.X:
                    if (args.Ctrl)
                    {
                        Trace.WriteLine("剪切");
                    }
                    break;

                case Microsoft.Xna.Framework.Input.Keys.C:
                    if (args.Ctrl)
                    {
                        Trace.WriteLine("拷贝");
                        Clipboard.WriteText("#Clipboard#");
                    }
                    break;

                case Microsoft.Xna.Framework.Input.Keys.V:
                    if (args.Ctrl)
                    {
                        var text = Clipboard.ReadText();
                        if (!String.IsNullOrEmpty(text))
                        {
                            this.AppendString(text);
                            this.InvalidateDrawing();
                        }
                    }
                    break;
                default:
                    break;
            }
        }



        private void RemoveString(Int32 dir)
        {
            if (dir == -1 && this.cursorPosition > 0)
            {
                this.cursorPosition = Math.Max(this.cursorPosition - 1, 0);
                this._text = this._text.Remove(this.cursorPosition, 1);
            }
            else if (dir == 1 && this.cursorPosition < this._text.Length)
            {
                this._text = this._text.Remove(this.cursorPosition, 1);
            }

            if (this.cursorPosition <= this.viewport.Start && this.viewport.Start > 0)
            {
                this.viewport.Start--;
                this.MeasureViewportStart();
            }
            else
            {
                this.MeasureViewportEnd();
            }
        }

        private void AppendString(String text)
        {
            if (cursorPosition > this._text.Length) cursorPosition = this._text.Length;
            var left = this._text.Substring(0, cursorPosition);
            var right = this._text.Substring(cursorPosition);
            this._text = left + text + right;
            cursorPosition += text.Length;
            if (cursorPosition>= this.viewport.End)
            {
                this.viewport.End += text.Length;
                this.MeasureViewportEnd();
            }
            else
            {
                this.MeasureViewportStart();
            }
        }

        /// <summary>
        /// 从后方测量Viewport Start
        /// </summary>
        private void MeasureViewportEnd()
        {
            if (this.viewport.Start < 0) this.viewport.Start = 0;
            if (this.viewport.End > this._text.Length) this.viewport.End = this._text.Length;
            var width = this.GlobalBounds.Width - (this.padding.Left + this.padding.Right);
            var viewText = this._text.Substring(this.viewport.Start, this.viewport.Length());
            var chars = this.Renderer.MeasureChars(this.Font, this.FontSize, viewText);
            var charW = 0.0f;
            var index = chars.Length - 1;
            while (index >= 0)
            {
                charW += chars[index];
                if (charW > width)
                {
                    break;
                }
                index--;
            }
            if (index >= 0)
            {
                this.viewport.Start = this.viewport.Start + index + 1;
            }
        }


        /// <summary>
        /// 从前方测量Viewport End
        /// </summary>
        private void MeasureViewportStart()
        {
            if (this.viewport.Start < 0) this.viewport.Start = 0;
            if (this.viewport.End > this._text.Length) this.viewport.End = this._text.Length;
            var width = this.GlobalBounds.Width - (this.padding.Left + this.padding.Right);
            var viewText = this._text.Substring(this.viewport.Start, this.viewport.Length());
            var chars = this.Renderer.MeasureChars(this.Font, this.FontSize, viewText);
            var charW = 0.0f;
            var index = 0;
            while (index < chars.Length)
            {
                charW += chars[index];
                if (charW > width)
                {
                    break;
                }
                index++;
            }
            if (index >= 0)
            {
                this.viewport.End = this.viewport.Start + index;
            }
        }



        private Int32 MeasureStringWidth(String text)
        {
            var lr = this.Renderer.MeasureString(this.Font, this.FontSize, text);
            return (Int32)lr.X;
        }


        private void InvalidateDrawing()
        {
            if (this.finalTexture != null)
            {
                using (this.Renderer.TargetRender(this.finalTexture))
                {
                    var s = this.Text.AsSpan().Slice(viewport.Start, this.Length - viewport.Start);
                    var y = (this.finalTexture.Height - this.FontSize) / 2;
                    this.Renderer.DrawString(this.Font, this.FontSize, s.ToString(), new Vector2(0, y), this.TextColor);
                }
            }
        }


        #region 不会动的属性


        public int CursorPosition
        {
            get { return cursorPosition; }
            set
            {
                if (cursorPosition != value)
                {
                    cursorPosition = value;
                    InvalidateDrawing();
                }
            }
        }
        private int cursorPosition;

        private void ActiveCurrsor()
        {
            this.currsorVisable = false;
            this.lastCurrsorTime = new TimeSpan();
        }


        protected override void CalcAutoSize()
        {
            var width = this.GlobalBounds.Width - (this.padding.Left + this.padding.Right);
            var height = this.GlobalBounds.Height - (this.padding.Top + this.padding.Bottom);
            if (width > 0 && height > 0)
            {
                if (this.finalTexture == null) this.finalTexture = this.Renderer.CreateRenderTarget(1, 1);
                if (this.finalTexture.Width != width || this.finalTexture.Height != height)
                {
                    this.finalTexture.Resize(width, height);
                }
                this.InvalidateDrawing();
            }
        }


        protected override void OnGotFocus()
        {
            var imeHandler = AuroraState.Services.GetService<SdlIMEHandler>();
            imeHandler.TextInput += ImeHandler_TextInput;
            imeHandler.StartTextComposition();
            this.ActiveCurrsor();
            base.OnGotFocus();
        }

        protected override void OnLostFocus()
        {
            var imeHandler = AuroraState.Services.GetService<SdlIMEHandler>();
            imeHandler.StopTextComposition();
            imeHandler.TextInput -= ImeHandler_TextInput;
            this.currsorVisable = false;
            base.OnLostFocus();
        }

        protected override void OnMouseEnter()
        {
            this.Root.Cursor.Push(Cursors.Text);
            base.OnMouseEnter();
        }

        protected override void OnMouseLeave()
        {
            this.Root.Cursor.Pop();
            base.OnMouseLeave();
        }


        public void Clear()
        {
            this._text = "";
            this.cursorPosition = 0;
            this.viewport.Start = 0;
            this.viewport.End = 0;
            this.InvalidateDrawing();
        }

        private TargetTexture finalTexture;
        /// <summary>
        /// 光标的大小
        /// </summary>
        public Rectangle CurrsorArea;
        public Color CursorColor { get; set; } = Color.White;

        private Boolean currsorVisable = false;
        private TimeSpan lastCurrsorTime;

        /// <summary>
        /// 文本区域
        /// </summary>
        private TextViewPort viewport;


        /// <summary>
        /// 光标所在位置
        /// </summary>
        private Int32 CurrsorPos = 0;
        /// <summary>
        /// 选中区域
        /// </summary>
        private Rectangle selectionArea;

        /// <summary>
        /// 制表符长度
        /// </summary>
        public Int32 TabSize
        {
            get
            {
                return this.TabChar.Length;
            }
            set
            {
                if (value <= 0 || value >= 8)
                {
                    throw new Exception("");
                }
                this.TabChar = "".PadLeft(value);
            }

        }

        private String TabChar = "    ";



        public Thickness Padding
        {
            get
            {
                return padding;
            }
            set
            {
                padding = value;

            }
        }

        private Thickness padding;

        public Color ActivedBorder { get; set; }
        public Color TextColor { get; set; }
        public String Text
        {
            get => this._text;
            set
            {
                var neatValue = value.Replace("\n", "").Replace("\t", this.TabChar);
                if (this._text != neatValue)
                {
                    this.Clear();
                    if (neatValue.Length > 0) this.AppendString(neatValue);
                }
            }
        }

        public Int32 Length
        {
            get => this._text.Length;
        }

        public Int32 MaxLength
        {
            get
            {
                return this.maxLength;
            }
            set
            {
                if (value <= 0)
                {
                    throw new InvalidDataException("");
                }
                this.maxLength = value;
            }
        }
        private Int32 maxLength;
        private String _text;
        #endregion

    }
}
