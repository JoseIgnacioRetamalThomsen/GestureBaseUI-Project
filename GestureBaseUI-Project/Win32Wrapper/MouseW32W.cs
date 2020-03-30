using System;
using System.Runtime.InteropServices;
using GestureBaseUI_Project;

namespace GestureBaseUI_Project.Win32Wrapper
{
    /// <summary>
    /// Provide acces to win32 api for controlling mouse actions
    ///    
    ///  Use of external mouse function for control the mouse.
    /// 
    ///  https://www.elitepvpers.com/forum/net-languages/118712-c-windows-api.html 
    /// 
    ///  https://stackoverflow.com/questions/8739523/directing-mouse-events-dllimportuser32-dll-click-double-click
    ///
    /// </summary>
    public sealed class MouseW32W
    {
        ///
        /// Mouse system flags
        /// 
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            XDOWN = 0x00000080,
            HWHEEL = 0x00001000,
        }

        /// <summary>
        /// Produce mouse events using flags
        /// 
        /// http://pinvoke.net/default.aspx/user32/mouse_event.html?diff=y
        /// </summary>
        /// <param name="dwFlags"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dwData"></param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, int dwExtraInfo);

        /// <summary>
        /// Set position of mouse in screen.
        /// 
        /// </summary>
        /// <param name="x"> mouse position</param>
        /// <param name="y">mouse positioon</param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        /// <summary>
        /// Get the actuam mouse position
        /// https://stackoverflow.com/a/4232281
        /// </summary>
        /// <param name="pt">reference to win32point where the position will be set.</param>
        /// <returns>true if success</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]

        public  static extern bool GetCursorPos(ref Win32Point pt);

 
    }
}
