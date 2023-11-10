using Aurora.UI.Common;
using Aurora.UI.Graphics.FontStashSharp;
using Microsoft.Xna.Framework;


namespace Aurora.UI
{
    public static class AuroraState
    {
        public static GameServiceContainer Services { get; internal set; } = new GameServiceContainer();

        public static PackageManager PackageManager { get; private set; } = new PackageManager();

        public static FontSystem FontSystem { get; private set; } = new FontSystem(new FontSystemParams() { Width = 1024, Height = 1024, IsAlignmentTopLeft = true });

    }


}
