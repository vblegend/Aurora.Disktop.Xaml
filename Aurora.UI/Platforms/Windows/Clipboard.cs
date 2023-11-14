using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace Aurora.UI.Platforms.Windows
{
    internal class Clipboard
    {
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseClipboard();

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EmptyClipboard();

        // Clipboard formats
        public const uint CF_TEXT = 1;
        public const uint CF_UNICODETEXT = 13;


        public static Boolean WriteText(string text)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;
            // Open the clipboard
            if (OpenClipboard(IntPtr.Zero))
            {
                try
                {
                    // Empty the clipboard
                    EmptyClipboard();

                    // Convert the string to a byte array with the appropriate encoding
                    byte[] dataBytes = Encoding.Unicode.GetBytes(text + '\0'); // '\0' is null-terminator for CF_UNICODETEXT

                    // Allocate unmanaged memory and copy the byte array
                    IntPtr dataPtr = Marshal.AllocHGlobal(dataBytes.Length);
                    Marshal.Copy(dataBytes, 0, dataPtr, dataBytes.Length);

                    // Set clipboard data
                    SetClipboardData(CF_UNICODETEXT, dataPtr);
                    return true;
                }
                finally
                {
                    // Close the clipboard
                    CloseClipboard();
                }
            }
            return false;
        }

        public static string ReadText()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return null;
            string clipboardText = string.Empty;
            // Open the clipboard
            if (OpenClipboard(IntPtr.Zero))
            {
                try
                {
                    // Get clipboard data
                    IntPtr dataPtr = GetClipboardData(CF_UNICODETEXT);

                    if (dataPtr != IntPtr.Zero)
                    {
                        // Convert the data pointer to a managed string
                        clipboardText = Marshal.PtrToStringUni(dataPtr);
                    }
                }
                finally
                {
                    // Close the clipboard
                    CloseClipboard();
                }
            }
            return clipboardText;
        }
    }








}
