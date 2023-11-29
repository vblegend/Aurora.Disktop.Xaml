using Aurora.UI;
using Aurora.UI.Common;
using Aurora.UI.Controls;
using Aurora.UI.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Aurora.Example
{
    
    internal class InitScene : PlayScene
    {
        private ITexture[] itemEffect = new ITexture[0];
        private ITexture[] items = new ITexture[0];



        public InitScene(PlayWindow Window) : base(Window)
        {

        }

        protected override async Task OnInitialize()
        {
            try
            {
                this.LoadXamlFromFile("Xaml\\ui2.xml");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }


            this.items = AuroraState.PackageManager.LoadLazyTextures("ui",112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127);
            this.itemEffect = AuroraState.PackageManager.LoadLazyTextures("ui", 96,97,98,99,100,101,102,103,104,105);
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

        protected override async Task OnUnInitialize()
        {

        }


        protected override void OnUpdate(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastEffect > effectInterval)
            {
                effectIndex = ++effectIndex % itemEffect.Length;
                lastEffect = gameTime.TotalGameTime;
            }
        }
        private TimeSpan effectInterval = new TimeSpan(0, 0, 0, 0, 200);
        private TimeSpan lastEffect;
        private Int32 effectIndex = 0;
        public void ImageMatrix_ItemDrawing(ItemMatrix sender, MatrixDrawEventArgs<IMatrixItem> args)
        {
            args.Renderer.Draw(this.items[args.Index % 16], args.GlobalPosition, Color.White);
            if (args.Index % (args.Item.Row +1) == 0)
            {
                args.Renderer.Draw(this.itemEffect[effectIndex], args.Rectangle.Center.ToVector2(), Color.White);
            }
        }


        public void ImageMatrix_ItemMenu(ItemMatrix sender, MatrixEventArgs<IMatrixItem> args)
        {
            Trace.WriteLine($"POP Menu: {args.Index} {args.Item}");
        }

        public void ImageMatrix_ItemClick(ItemMatrix sender, MatrixEventArgs<IMatrixItem> args)
        {
            Trace.WriteLine($"Click: {args.Index} {args.Item}");
        }

        public void ImageMatrix_ItemMouseLeave(ItemMatrix sender, MatrixEventArgs<IMatrixItem> args)
        {
            //Trace.WriteLine($"Mouse Leave: {args.Index} {args.Item}");
        }

        public void ImageMatrix_ItemMouseEnter(ItemMatrix sender, MatrixEventArgs<IMatrixItem> args)
        {
            //Trace.WriteLine($"Mouse Enter: {args.Index} {args.Item}");
        }
        public void ImageMatrix_ItemMouseDown(ItemMatrix sender, MatrixEventArgs<IMatrixItem> args)
        {


            //Trace.WriteLine($"Mouse Down: {args.Index} {args.Item}");
        }
        public void ImageMatrix_ItemMouseUp(ItemMatrix sender, MatrixEventArgs<IMatrixItem> args)
        {
            //Trace.WriteLine($"Mouse Up: {args.Index} {args.Item}");
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
