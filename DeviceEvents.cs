using System;
using System.Threading;
using System.Windows.Forms;

namespace RCC
{
    public static class DeviceEvents
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010,
            XDown = 0x00000080,
            Xup = 0x00000100
        }
        /// <summary>
        /// Funtion for send mouse event
        /// </summary>
        /// <param name="valueDown"> Key flag for down event </param>
        /// <param name="valueUp"> Key flag for up event </param>
        /// <param name="x"> Mouse position x </param>
        /// <param name="y"> Mouse position y </param>
        /// <param name="interval"> Interval beaten mouse event down and up </param>
        public static void SendMouseEvent(MouseEventFlags valueDown, MouseEventFlags valueUp, int x, int y, int interval = 0)
        {
            AllDllImport.mouse_event((uint)valueDown, (uint)x, (uint)y, 0, 0);
            Thread.Sleep(interval != 0 ? interval : 100);
            AllDllImport.mouse_event((uint)valueUp, (uint)x, (uint)y, 0, 0);
        }
        /// <summary>
        ///  Function for paste text
        /// </summary>
        /// <param name="text"> Text for paste </param>
        public static void SendPasteText(string text)
        {
            Clipboard.SetText(text);
            SendKeyEvent(0x11, 0x56, 0x00); // Ctrl + V
            SendKeyEvent(0x0D, 0x00, 0x00); // Enter
        }
        /// <summary>
        /// Function for send key event
        /// </summary>
        /// <param name="key1"> value first key </param>
        /// <param name="key2"> value second key </param>
        /// <param name="key3"> value third event </param>
        /// <param name="interval"> Interval beaten key event down and up </param>
        public static void SendKeyEvent(byte key1, byte key2, byte key3, int interval = 0)
        {
            if (key1 != 0x00) AllDllImport.keybd_event(key1, 0, 0, 0);
            if (key2 != 0x00) AllDllImport.keybd_event(key2, 0, 0, 0);
            if (key3 != 0x00) AllDllImport.keybd_event(key3, 0, 0, 0);

            Thread.Sleep(interval != 0 ? interval : 100);

            if (key1 != 0x00) AllDllImport.keybd_event(key1, 0, 0x2, 0);
            if (key2 != 0x00) AllDllImport.keybd_event(key2, 0, 0x2, 0);
            if (key3 != 0x00) AllDllImport.keybd_event(key3, 0, 0x2, 0);
        }
    }
}