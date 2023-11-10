using Aurora.UI;
using Aurora.UI.Controls;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Aurora.Example
{
    internal class InitScene : PlayScene
    {
        public InitScene(PlayWindow Window) : base(Window)
        {

        }

        protected override async Task OnInitialize()
        {
            try
            {
                this.LoadXamlFromFile("Xaml\\ui.xml");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }





            //byte[] bytecode = File.ReadAllBytes(@"D:\SourceCode\Aurora\Aurora.UI\Shaders\disable_gl.mgfx");
            //this.disableEffect = new Effect(this.Renderer.GraphicsDevice, bytecode);

            //using (var render = this.Renderer.TargetRender(targetTmp))
            //{
            //    this.Renderer.SetState(effect: this.disableEffect);
            //    this.Renderer.Clear(Color.Yellow);
            //    this.Renderer.Draw(this.texture, new Vector2(0, 0), Color.White);
            //}



            //using (this.Renderer.TargetRender(this.target))
            //{

            //    this.Renderer.Clear(Color.Sienna);
            //    this.Renderer.Draw(this.texture, new Vector2(0, 0), Color.White);


            //    //this.Renderer.SetBlendState(null, null);
            //    this.Renderer.Draw(targetTmp, new Vector2(64, 0), Color.White);
            //}
        }









        protected override void OnRender(GameTime gameTime)
        {
            //this.GraphicContext.StrokeRectangle(new Rectangle(400, 400, 200, 200), Color.Green, 1);
            //this.GraphicContext.DrawMarkRect(new Rectangle(550, 500, 200, 200), 145, Color.Gray);

            //this.Renderer.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //DrawModelWithTransparencyEffect(model, world, view, projection);
            //this.Renderer.GraphicsDevice.BlendState = BlendState.Opaque;



            //var brush = new IBrush() { 
            //    Texture = this.dialog,
            //    Color = Color.White
            //};


            //this.Renderer.SetState(effect: this.disableEffect);
            //this.Renderer.DrawGrid( new Rectangle(0, 0, 500, 500), brush, new Thickness(this.dialog.Width / 3));



            //this.Renderer.SetState();






            //this.Renderer.Draw(this.target, new Vector2(590, 500), Color.White);
            //this.Renderer.SetBlendState(BlendState.Additive);
            //this.Renderer.Draw(this.effectTexture, new Vector2(1450, 350), Color.White);

        }



        protected override void OnUpdate(GameTime gameTime)
        {

        }

        protected override async Task OnUnInitialize()
        {

        }



        public void Button_Click(Button sender)
        {
            Trace.WriteLine("Button Click事件触发");
        }

        public void Image_Click(Image sender)
        {
            Trace.WriteLine("Image Click事件触发");
        }


        public void Debuging_Click(CheckBox sender)
        {
            this.Debuging = sender.Value;
        }


        public void FullScreen_Click(CheckBox sender)
        {
            var _graphics = AuroraState.Services.GetService<GraphicsDeviceManager>();
            _graphics.IsFullScreen = sender.Value;
            //_graphics.PreferredBackBufferWidth = 1440;
            //_graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();
        }


        public void PBAdd_Click(Button sender)
        {
            var hp = this.Query<ProgressBar>("Status/HP");
            var mp = this.Query<ProgressBar>("Status/MP");
            var exp = this.Query<ProgressBar>("Status/EXP");
            

            hp.Value += Math.Min(hp.MaxValue - hp.Value, 40);
            mp.Value += Math.Min(mp.MaxValue - mp.Value, 40);
            exp.Value += Math.Min(exp.MaxValue - exp.Value, 40);
        }
        public void PBDec_Click(Button sender)
        {
            var hp = this.Query<ProgressBar>("Status/HP");
            var mp = this.Query<ProgressBar>("Status/MP");
            var exp = this.Query<ProgressBar>("Status/EXP");
            hp.Value -= Math.Min(hp.Value, 40);
            mp.Value -= Math.Min(mp.Value, 40);
            exp.Value -= Math.Min(exp.Value, 40);
        }



    }
}
