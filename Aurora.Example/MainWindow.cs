using Aurora.Disktop;
using Aurora.Disktop.Common;
using Aurora.Disktop.Graphics;
using Aurora.Disktop.Services;
using Microsoft.Xna.Framework.Graphics;
using Resource.Package.Assets;

namespace Aurora.Example
{
    internal class MainWindow : PlayWindow
    {
        public override void OnAfterInitialize()
        {

            this.Window.Title = "Aurora UI";


            var pack = ResourcePackage.Open(@"D:\game_root\Data\ui.asset");

            AuroraState.PackageManager.Register("ui", pack);



            this.Font = TTFFont.FromFile("./Fonts/default.ttf", 24);
            _ = this.LoadScene<InitScene>();
            var file = AssetFileStream.Open(@"D:\game_root\Data\mouse.asset", "");
            List<SimpleTexture> textures = new List<SimpleTexture>();
            for (uint i = 0; i < 49; i++)
            {
                var block = file.Read(i);
                using (var ms = new MemoryStream(block.Data))
                {
                    textures.Add(SimpleTexture.FromStream(this.GraphicsDevice, ms));
                }
            }
            var Cursor = AuroraState.Services.GetService<ICursorService>();
            Cursor.SetTextures(textures.ToArray());
            Cursor.Source = CursorSource.CustomCursor;
        }





    }
}
