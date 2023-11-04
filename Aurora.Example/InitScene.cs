using Aurora.Disktop;
using Aurora.Disktop.Controls;
using Aurora.Disktop.Graphics;
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

        //private SimpleTexture dialog { get; set; }

        //private SimpleTexture texture { get; set; }
        //private SimpleTexture effectTexture { get; set; }

        //private TargetTexture target { get; set; }


        //private Effect disableEffect { get; set; }

        protected override async Task OnInitialize()
        {
            //this.dialog  = SimpleTexture.FromFile(this.Renderer.GraphicsDevice, @"D:\game_root\0000193.png");
            //this.texture = SimpleTexture.FromFile(this.Renderer.GraphicsDevice, @"D:\game_root\0004834.png");
            //this.effectTexture = SimpleTexture.FromFile(this.Renderer.GraphicsDevice, @"D:\game_root\0000889.png");
            //var targetTmp = this.Renderer.CreateRenderTarget(128, 64);
            //this.target = this.Renderer.CreateRenderTarget(128, 64);

            try
            {
                this.LoadXamlFromFile("Xaml\\ui.xml");
            }catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
    




            //byte[] bytecode = File.ReadAllBytes(@"D:\SourceCode\Aurora\Aurora.Disktop\Shaders\disable_gl.mgfx");
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

            //var window = this.Add(new Window());
            //window.Name = "Window-测试";
            //window.Location = new Point(100,100);
            //window.Size = new Point(400,200);
            //window.Background = new UIBrush()
            //{
            //    Texture = this.dialog
            //};


            //var button1 = window.Add(new Button());
            //button1.Name = "Button-正常";
            //button1.Location = new Point(0, 0);
            ////button1.Size = new Point(100, 100);
            //button1.SetTheme(SimpleTexture.FromFile(this.Renderer.GraphicsDevice, @"D:\game_root\0000112.png"));

            //var button2 = window.Add(new Button());
            //button2.Name = "Button-禁用";
            //button2.Location = new Point(100, 100);
            ////button2.Size = new Point(100, 100);
            //button2.SetTheme(SimpleTexture.FromFile(this.Renderer.GraphicsDevice, @"D:\game_root\0000112.png"));
            //button2.Disabled = true;


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
            this.Renderer.DrawString(this.Window.Font, $"Fps：{this.Window.FpsCounter.Fps}", new Vector2(5, 10), Color.White);
        }



        protected override void OnUpdate(GameTime gameTime)
        {

        }

        protected override void OnDrawing(GameTime gameTime)
        {


        }
        protected override async Task OnUnInitialize()
        {

        }



        public void Button_Click(Button sender)
        {
            Trace.WriteLine("按钮事件触发");
        }


    }
}
