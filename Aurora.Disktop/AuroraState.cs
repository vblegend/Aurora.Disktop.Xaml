using Aurora.Disktop.Common;
using Microsoft.Xna.Framework;


namespace Aurora.Disktop
{
    public static class AuroraState
    {
        public static GameServiceContainer Services {  get; internal set; } = new GameServiceContainer();

        public static PackageManager PackageManager { get; private set; } = new PackageManager(); 
        
        
        public static FontManager FontManager { get; private set; } = new FontManager();

        


    }


}
