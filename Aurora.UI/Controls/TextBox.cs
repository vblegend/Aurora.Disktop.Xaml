using Aurora.UI.Common;
using Aurora.UI.Components;
using Aurora.UI.Graphics;
using Aurora.UI.Platforms.Windows;
using Microsoft.Xna.Framework;
using MonoGame.IMEHelper;
using System.Diagnostics;

namespace Aurora.UI.Controls
{
    public class TextBox : Control
    {
        public TextBox()
        {
            this.maxLength = 100;
            this.Size = new Point(85, 22);
            this.TextColor = Color.White;
            this.ActivedBorder = ColorExtends.FromHtml("#00a2e8");
        }


        const int UnicodeSimplifiedChineseMin = 0x4E00;
        const int UnicodeSimplifiedChineseMax = 0x9FA5;
        private readonly TimeSpan OneSecond = new TimeSpan(0, 0, 0, 0, 500);



        protected override void OnUpdate(GameTime gameTime)
        {
            if (this.CurrsorArea.Height != this.FontSize)
            {
                this.CurrsorArea.Height = (Int32)this.FontSize;
                this.CurrsorArea.Width = 2;
            }

            if (gameTime.TotalGameTime - lastCurrsorTime > OneSecond)
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
                var text = this._text.Substring(textArea.Left, this.cursorPosition - textArea.Left);
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
                    this.ActiveCurrsor();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Right:
                    if (this.cursorPosition < this._text.Length) this.cursorPosition++;
                    this.ActiveCurrsor();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Home:
                    if (this.cursorPosition > 0) this.cursorPosition = 0;
                    this.ActiveCurrsor();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.End:
                    this.cursorPosition = this._text.Length;
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
                if (this.cursorPosition <= this.textArea.X && this.textArea.X > 0)
                {
                    this.textArea.X--;
                }
            }
            else if (dir == 1 && this.cursorPosition < this._text.Length)
            {
                this._text = this._text.Remove(this.cursorPosition, 1);
            }
        }



        private void AppendString(String text)
        {
            if (cursorPosition > this._text.Length) cursorPosition = this._text.Length;
            if (cursorPosition == this._text.Length)
            {
                this._text += text;
            }
            else if (cursorPosition == 0)
            {
                this._text = text + this._text;
            }
            else
            {
                var left = this._text.Substring(0, cursorPosition);
                var right = this._text.Substring(cursorPosition);
                this._text = left + text + right;
            }
            cursorPosition += text.Length;



            //  this.textArea.Width += this._text.Length;

            //var dis = this._text.Substring(this.textArea.Left, this.cursorPosition - this.textArea.Left);
            //if (this.MeasureStringWidth(dis) > this.globalBounds.Width)
            //{
            //    this.textArea.X += text.Length;
            //}

        }


        private void InsertString(String text)
        {



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
                    var s = this.Text.AsSpan().Slice(textArea.Left, this.Length - textArea.Left);
                    var y = (this.finalTexture.Height - this.FontSize) / 2;
                    this.Renderer.DrawString(this.Font, this.FontSize, s.ToString(), new Vector2(0, y), this.TextColor);
                    //this.Font.MeasureString("")
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
            var oldTex = this.finalTexture;
            this.finalTexture = this.Renderer.CreateRenderTarget(this.GlobalBounds.Width - (this.padding.Left + this.padding.Right), this.GlobalBounds.Height - (this.padding.Top + this.padding.Bottom));
            this.InvalidateDrawing();
            if (oldTex != null)
            {
                oldTex.Dispose();
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
        private Rectangle textArea;


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
                    this._text = neatValue;
                    if (this._text.Length > this.maxLength)
                    {
                        this._text = this._text.Substring(0, this.maxLength);
                    }
                    //this.textArea.Width = this.Text.Length;
                    this.cursorPosition = this._text.Length;
                    this.InvalidateDrawing();
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
                if (this._text.Length > this.maxLength)
                {
                    this._text = this._text.Substring(0, this.maxLength);
                }
            }
        }
        private Int32 maxLength;
        private String _text;
        #endregion

    }
}
