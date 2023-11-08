using Aurora.Disktop;
using Aurora.Disktop.Common;
 

namespace Aurora.Example
{
    internal class MainWindow : PlayWindow
    {
        public override void OnAfterInitialize()
        {

            this.Window.Title = "Aurora UI";
            //this.IsMouseVisible = true;

            //var pack = ResourcePackage.Open(@"./ui.asset");

            //AuroraState.PackageManager.Register("ui", pack);
            _ = this.LoadScene<InitScene>();


          //  this.Font = TTFFont.FromFile("./Fonts/Microsoft YaHei.ttf", 24);

           // AuroraState.Services.AddService(this.Font);  


            //var file = AssetFileStream.Open(@"D:\game_root\Data\mouse.asset", "");
            //List<SimpleTexture> textures = new List<SimpleTexture>();
            //for (uint i = 0; i < 49; i++)
            //{
            //    var block = file.Read(i);
            //    using (var ms = new MemoryStream(block.Data))
            //    {
            //        textures.Add(SimpleTexture.FromStream(this.GraphicsDevice, ms));
            //    }
            //}
            //var Cursor = AuroraState.Services.GetService<ICursorService>();
            //Cursor.SetTextures(textures.ToArray());
            //Cursor.Source = CursorSource.CustomCursor;
        }





    }
}
