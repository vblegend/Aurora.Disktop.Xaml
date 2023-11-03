using Aurora.Disktop.Common;
using Microsoft.Xna.Framework;


namespace Aurora.Disktop
{
    public static class AuroraState
    {
        public static GameServiceContainer Services {  get; internal set; } = new GameServiceContainer();

        public static PackageManager PackageManager { get; internal set; } = new PackageManager();

    }
}
