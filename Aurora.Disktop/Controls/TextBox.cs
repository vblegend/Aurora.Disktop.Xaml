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






        protected override void OnRender(GameTime gameTime)
        {
            base.OnRender(gameTime);
            if (this.IsFocus)
            {
                this.Renderer.DrawRectangle(this.GlobalBounds, this.ActivedBorder);
            }
            var local = this.GlobalBounds.Location;
            this.Renderer.DrawString(this.Font, this.Text, new Vector2(local.X, local.Y), this.TextColor);



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
                    break;
            }
        }








        protected override void CalcAutoSize()
        {

        }

        protected override void OnGotFocus()
        {
            var imeHandler = AuroraState.Services.GetService<SdlIMEHandler>();
            imeHandler.TextInput += ImeHandler_TextInput;
            imeHandler.TextComposition += ImeHandler_TextComposition;
            imeHandler.StartTextComposition();
        }

        private void ImeHandler_TextComposition(object sender, TextCompositionEventArgs e)
        {
            var rect = new Rectangle(10, 50, 0, 0);
            (sender as SdlIMEHandler).SetTextInputRect(ref rect);
            if (e.CandidateList == null)
            {
                Trace.WriteLine($"{e.CompositedText}");
            }
            else
            {
                Trace.WriteLine($"{e.CompositedText}  {String.Join(' ', e.CandidateList?.Candidates)}");
            }
        }

        protected override void OnLostFocus()
        {
            var imeHandler = AuroraState.Services.GetService<SdlIMEHandler>() ;
            imeHandler.StopTextComposition();
            imeHandler.TextInput -= ImeHandler_TextInput;
            imeHandler.TextComposition -= ImeHandler_TextComposition;
   
        }

        public Color ActivedBorder { get; set; }
        public Color TextColor { get; set; }
        public String Text
        {
            get; set;
        }


    }
}
