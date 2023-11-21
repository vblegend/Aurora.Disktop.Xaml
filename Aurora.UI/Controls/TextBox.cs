using Aurora.UI.Common;
using Aurora.UI.Components;
using Aurora.UI.Graphics;
using Aurora.UI.Platforms.Windows;
using Microsoft.Xna.Framework;
using MonoGame.IMEHelper;


namespace Aurora.UI.Controls
{
    public struct TextRange
    {
        public TextRange()
        {
            this.Start = 0;
            this.End = 0;
        }
        public TextRange(Int32 start, Int32 end)
        {
            this.Start = start;
            this.End = end;
        }
        public Int32 Start;
        public Int32 End;
        public Int32 Length()
        {
            return this.End - this.Start;
        }
        public override string ToString()
        {
            return $"Start = {Start}, End = {End}";
        }
    }



    public class TextBox : Control
    {
        public TextBox()
        {
            this._text = "";
            this.passwordChar = null;
            this.maxLength = 100;
            this.Size = new Point(85, 22);
            this.TextColor = Color.White;
            this.ActivedBorder = ColorExtends.FromHtml("#00a2e8");
        }



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
                this.Renderer.FillRectangle(this.CurrsorArea.Add(this.GlobalBounds.Location).Add(padding), this.CursorColor);
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
                    this.ActiveCurrsor();
                    break;

                case 127:
                    // Delete
                    this.RemoveString(1);
                    this.InvalidateDrawing();
                    this.ActiveCurrsor();
                    break;

                case 9:  // Tab
                case 27: // Esc
                case 13: // Enter
                    break;
                default:
                    // 输入
                    var inputChar = e.Character;
                    AppendString(inputChar.ToString());
                    this.InvalidateDrawing();
                    this.ActiveCurrsor();
                    break;
            }
        }


        protected override void OnKeyDown(IKeyboardMessage args)
        {
            var front = false;
            switch (args.Key)
            {
                case Microsoft.Xna.Framework.Input.Keys.Left:
                    if (!args.Shift)
                    {
                        this.selection = null;
                    }
                    else
                    {   // Pressed shift
                        if (!this.selection.HasValue) this.selection = new TextRange(this.cursorPosition, this.cursorPosition);
                        front = this.cursorPosition == this.selection.Value.Start;
                    }
                    // cursor
                    if (this.cursorPosition > 0) this.cursorPosition--;
                    if (this.cursorPosition < this.viewport.Start)
                    {
                        this.viewport.Start--;
                        this.MeasureViewportStart();
                    }
                    if (this.selection.HasValue)
                    {
                        if (front)
                        {
                            this.selection = new TextRange(this.cursorPosition, this.selection.Value.End);
                        }
                        else
                        {
                            this.selection = new TextRange(this.selection.Value.Start, this.cursorPosition);
                        }
                        if (this.selection.Value.Start == this.selection.Value.End) this.selection = null;
                    }
                    this.InvalidateDrawing();
                    this.ActiveCurrsor();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Right:
                    if (!args.Shift)
                    {
                        this.selection = null;
                    }
                    else
                    {   // Pressed shift
                        if (!this.selection.HasValue) this.selection = new TextRange(this.cursorPosition, this.cursorPosition);
                        front = !(this.cursorPosition == this.selection.Value.End);
                    }
                    // cursor
                    if (this.cursorPosition < this._text.Length) this.cursorPosition++;
                    if (this.cursorPosition > this.viewport.End)
                    {
                        this.viewport.End++;
                        this.MeasureViewportEnd();
                    }
                    if (this.selection.HasValue)
                    {
                        if (front)
                        {
                            this.selection = new TextRange(this.cursorPosition, this.selection.Value.End);
                        }
                        else
                        {
                            this.selection = new TextRange(this.selection.Value.Start, this.cursorPosition);
                        }
                        if (this.selection.Value.Start == this.selection.Value.End) this.selection = null;
                    }
                    this.InvalidateDrawing();
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
                        this.SelectAll();
                    }
                    break;
                case Microsoft.Xna.Framework.Input.Keys.X:
                    if (args.Ctrl)
                    {
                        if (this.selection.HasValue)
                        {
                            var txt = this._text.Substring(this.selection.Value.Start, this.selection.Value.Length());
                            Clipboard.WriteText(txt);
                            this.AppendString("");
                            this.InvalidateDrawing();
                            this.ActiveCurrsor();
                        }
                    }
                    break;
                case Microsoft.Xna.Framework.Input.Keys.C:
                    if (args.Ctrl)
                    {
                        if (this.selection.HasValue)
                        {
                            var txt = this._text.Substring(this.selection.Value.Start, this.selection.Value.Length());
                            Clipboard.WriteText(txt);
                        }
                    }
                    break;
                case Microsoft.Xna.Framework.Input.Keys.V:
                    if (args.Ctrl)
                    {
                        var text = Clipboard.ReadText();
               
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = this.Purification(text);
                            this.AppendString(text);
                            this.InvalidateDrawing();
                            this.ActiveCurrsor();
                        }
                    }
                    break;
                default:
                    break;
            }


            base.OnKeyDown(args);
        }

        /// <summary>
        /// 选中全部文本
        /// </summary>
        public void SelectAll()
        {
            this.selection = new TextRange(0, this._text.Length);
            this.cursorPosition = this._text.Length;
            this.viewport.End = this._text.Length;
            this.MeasureViewportEnd();
            this.InvalidateDrawing();
            this.ActiveCurrsor();
        }



        protected override void OnMouseDown(IMouseMessage args)
        {
            if (this.Root.CurrentTimeSpan - this.lastParssedTime < new TimeSpan(0,0,0,0,300))
            {
               this.SelectAll();
                return;
            }
            this.lastParssedTime =  this.Root.CurrentTimeSpan;
            base.OnMouseDown(args);
            var pos = args.GetLocation(this);
            String viewText;
            if (this.passwordChar.HasValue)
            {
                viewText = "".PadLeft(this.viewport.Length(), this.passwordChar.Value);
            }
            else
            {
                // 未控制长度
                viewText = this._text.Substring(this.viewport.Start);
            }
            var chars = this.Renderer.MeasureChars(this.Font, this.FontSize, viewText);
            var lw = this.padding.Left + 0.0f;
            var index = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if (lw + chars[i] / 2 > pos.X)
                {
                    break;
                }
                lw += chars[i];
                index++;
            }
            this.Root.EventManager.CaptureMouse(this);
            this.selection = null;
            this.cursorPosition = this.viewport.Start + index;
            this.InvalidateDrawing();
            this.ActiveCurrsor();
        }


        protected override void OnMouseUp(IMouseMessage args)
        {
            base.OnMouseUp(args);
            this.Root.EventManager.ReleaseMouse();
        }




        private void RemoveString(Int32 dir)
        {
            if (this.selection.HasValue)
            {
                var left = this._text.Substring(0, this.selection.Value.Start);
                var right = this._text.Substring(this.selection.Value.End);
                this._text = left + right;
                this.cursorPosition = this.selection.Value.Start;
                this.selection = null;
            }
            else if (dir == -1 && this.cursorPosition > 0)
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
                this.viewport.Start = this.cursorPosition - 1;
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
            var left = "";
            var right = "";
            if (this.selection.HasValue)
            {
                left = this._text.Substring(0, this.selection.Value.Start);
                right = this._text.Substring(this.selection.Value.End);
                this._text = left + right;
                this.cursorPosition = this.selection.Value.Start;
                this.selection = null;
                if (this.viewport.Start > this.cursorPosition) this.viewport.Start = this.cursorPosition;
            }

            if (text.Length + this.Length > this.maxLength)
            {
                var len = this.maxLength - this.Length;
                text = text.Substring(0, len);
            }

            left = this._text.Substring(0, cursorPosition);
            right = this._text.Substring(cursorPosition);
            this._text = left + text + right;
            cursorPosition += text.Length;
            if (cursorPosition >= this.viewport.End)
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
            String viewText;
            if (this.passwordChar.HasValue)
            {
                viewText = "".PadLeft(this.viewport.Length(), this.passwordChar.Value);
            }
            else
            {
                viewText = this._text.Substring(this.viewport.Start, this.viewport.Length());
            }
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
            String viewText;
            if (this.passwordChar.HasValue)
            {
                viewText = "".PadLeft(this.viewport.Length(), this.passwordChar.Value);
            }
            else
            {
                // 未控制长度
                viewText = this._text.Substring(this.viewport.Start);
            }

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

        private void InvalidateDrawing()
        {
            if (this.finalTexture != null)
            {
                var y = (this.finalTexture.Height - this.FontSize) / 2;
                using (this.Renderer.TargetRender(this.finalTexture))
                {
                    String viewText;
                    if (this.passwordChar.HasValue)
                    {
                        viewText = "".PadLeft(this.viewport.Length(), this.passwordChar.Value);
                    }
                    else
                    {
                        viewText = this._text.Substring(this.viewport.Start, this.viewport.Length());
                    }
                    if (this.selection.HasValue)
                    {
                        var st = Math.Max(this.selection.Value.Start, this.viewport.Start) - this.viewport.Start;
                        var en = Math.Min(this.selection.Value.End, this.viewport.End) - this.viewport.Start;
                        if (en > st)
                        {
                            var s = this.Renderer.MeasureString(this.Font, this.FontSize, viewText.Substring(0, st));
                            var s2 = this.Renderer.MeasureString(this.Font, this.FontSize, viewText.Substring(st, en - st));
                            this.Renderer.FillRectangle(new Rectangle((Int32)s.X, 0, (Int32)s2.X, 100), new Color(0, 120, 215));
                        }
                    }

                    this.Renderer.DrawString(this.Font, this.FontSize, viewText.ToString(), new Vector2(0, y), this.TextColor);
                }


            }
        }

        private void ReCalcCurrsorPosition()
        {
            var y = (this.finalTexture.Height - this.FontSize) / 2;
            var pos = this.cursorPosition - this.viewport.Start;
            if (pos >= 0)
            {
                var txt = this._text.Substring(this.viewport.Start, pos);
                var size = this.Renderer.MeasureString(this.Font, this.FontSize, txt);
                this.CurrsorArea.X = (Int32)size.X;
                this.CurrsorArea.Y = (Int32)y;
            }
        }




        #region 不会动的属性

        /// <summary>
        /// 提纯字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private String Purification(String value)
        {
            if (String.IsNullOrEmpty(value)) return "";
            return value.Replace("\n", "").Replace("\r", "").Replace("\t", " ");
        }


        /// <summary>
        /// 光标的大小
        /// </summary>
        public Rectangle CurrsorArea;
        public Color CursorColor { get; set; } = Color.White;

        private Boolean currsorVisable = false;
        private TimeSpan lastCurrsorTime;

        private TimeSpan lastParssedTime;
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
            this.ReCalcCurrsorPosition();
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
            this.selection = null;
            this.InvalidateDrawing();
        }

        private TargetTexture finalTexture;


        /// <summary>
        /// 文本区域
        /// </summary>
        private TextRange viewport;

        private TextRange? selection;


        public Char? PasswordChar
        {
            get
            {
                return this.passwordChar;
            }
            set
            {
                this.passwordChar = value;
            }

        }
        private Char? passwordChar;



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
                var neatValue = this.Purification(value);
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
