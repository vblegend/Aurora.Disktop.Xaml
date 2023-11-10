using System.Runtime.InteropServices;

namespace Aurora.UI.Graphics.FontStashSharp
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct FontAtlasNode
    {
        public int X;
        public int Y;
        public int Width;
    }
}
