using Aurora.Disktop.Common;
using Aurora.Disktop.Components;
using Aurora.Disktop.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.IMEHelper;
using System.Diagnostics;

namespace Aurora.Disktop.Controls
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
            if (this.CurrsorArea.Height != this.Font.Size)
            {
                this.CurrsorArea.Height = (Int32)this.Font.Size;
                this.CurrsorArea.Width = 3;
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
            if (this.currsorVisable) this.Renderer.FillRectangle(this.CurrsorArea.Add(this.GlobalBounds.Location), this.CursorColor);
            // 焦点突出显示边框
            if (this.IsFocus) this.Renderer.DrawRectangle(this.GlobalBounds, this.ActivedBorder);
        }







        private void ImeHandler_TextInput(object sender, MonoGame.IMEHelper.TextInputEventArgs e)
        {
            switch ((int)e.Character)
            {
                case 8:
                    // 退格
                    if (this.Text.Length > 0)
                        this.Text = this.Text.Remove(this.Text.Length - 1, 1);
                    break;
                case 27:
                case 13:
                    // 回车
                    break;
                default:
                    // 输入
                    if (e.Character > UnicodeSimplifiedChineseMax)
                    {
                        this.Text += "?";
                    }
                    else
                    {
                        this.Text += e.Character;
                    }
                    this.TextChanged();
                    break;
            }
        }


        private void TextChanged()
        {
            if (this.finalTexture != null)
            {
                using (this.Renderer.TargetRender(this.finalTexture))
                {
                    var s =  this.Text.AsSpan().Slice(textArea.Left, textArea.Width);
                    this.Renderer.DrawString(this.Font, s.ToString(), new Vector2(0, 0), this.TextColor);

                    this.Font.MeasureString("")
                }
            }

        }






        #region 不会动的属性


        private void ActiveCurrsor()
        {
            this.currsorVisable = false;
            this.lastCurrsorTime = new TimeSpan();
        }


        protected override void CalcAutoSize()
        {
            var oldTex = this.finalTexture;
            this.finalTexture = this.Renderer.CreateRenderTarget(this.GlobalBounds.Width, this.GlobalBounds.Height);
            this.TextChanged();
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
                    this.TextChanged();
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
