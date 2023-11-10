using Aurora.UI.Common;
using Aurora.UI.Components;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.IMEHelper;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace Aurora.UI.Controls
{
    public class TextBox : Control
    {
        public TextBox()
        {
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






        }







        protected override void OnRender(GameTime gameTime)
        {
            base.OnRender(gameTime);

            //var local = this.GlobalBounds.Location;
            //this.Renderer.DrawString(this.Font, this.Text, new Vector2(local.X, local.Y), this.TextColor);

            if (this.finalTexture != null)
            {
                this.Renderer.Draw(this.finalTexture, this.GlobalBounds, Color.White);
            }

            // 显示光标
            if (this.currsorVisable)
            {
                var text = this._text.Substring(textArea.Left, this.cursorPosition - textArea.Left);
                var lr =  this.MeasureStringWidth(text);
                this.Renderer.FillRectangle(this.CurrsorArea.Add(this.GlobalBounds.Location).Add(new Point(lr, 0)), this.CursorColor);
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

                    this.RemoveString();

                    this.InvalidateDrawing();
                    //if (this.Text.Length > 0)
                    //    this.Text = this.Text.Remove(this.Text.Length - 1, 1);
                    break;
                case 27:
                case 13:
                    // 回车
                    break;
                default:
                    // 输入
                    var inputChar = "";
                    if (e.Character > UnicodeSimplifiedChineseMax)
                    {
                        inputChar += "?";
                    }
                    else
                    {
                        inputChar += e.Character;
                    }


                    AppendString(inputChar);





                    this.InvalidateDrawing();
                    break;
            }
        }


        private void RemoveString()
        {
            if (this.cursorPosition > 0)
            {
                this.cursorPosition = Math.Max(this.cursorPosition - 1, 0);
                this._text = this._text.Remove(this.cursorPosition, 1);
                if (this.cursorPosition <= this.textArea.X && this.textArea.X > 0)
                {
                    this.textArea.X--;
                }
            }
        }



        private void AppendString(String text)
        {

            if (cursorPosition > this._text.Length) cursorPosition = this._text.Length;
            if (cursorPosition == this._text.Length)
            {
                this._text += text;
                cursorPosition+= text.Length;
               
            }
            else
            {




            }

          //  this.textArea.Width += this._text.Length;

            var dis = this._text.Substring(this.textArea.Left, this.cursorPosition - this.textArea.Left);
            if (this.MeasureStringWidth(dis) > this.globalBounds.Width)
            {
                this.textArea.X += text.Length;
            }

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
                    var s =  this.Text.AsSpan().Slice(textArea.Left, this.Length - textArea.Left);
                    this.Renderer.DrawString(this.Font, this.FontSize, s.ToString(), new Vector2(0, 0), this.TextColor);

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
            this.finalTexture = this.Renderer.CreateRenderTarget(this.GlobalBounds.Width, this.GlobalBounds.Height);
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
        }

        protected override void OnLostFocus()
        {
            var imeHandler = AuroraState.Services.GetService<SdlIMEHandler>();
            imeHandler.StopTextComposition();
            imeHandler.TextInput -= ImeHandler_TextInput;
            this.currsorVisable = false;
        }

        protected override void OnMouseEnter()
        {
            this.Root.Cursor.Push(Cursors.Text);
        }

        protected override void OnMouseLeave()
        {
            this.Root.Cursor.Pop();
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
                if (this._text != value)
                {
                    this._text = value;
                    //this.textArea.Width = this.Text.Length;
                    this.cursorPosition = value.Length;
                    this.InvalidateDrawing();
                }
            }
        }

        public Int32 Length
        {
            get => this._text.Length;
        }


        private String _text;
        #endregion

    }
}
