using System;
using System.Runtime.InteropServices;

namespace GestureBaseUI_Project.MouseControl
{
    /// <summary>
    /// Control the mouse: set the curson, click and controls the mouse wheel.
    /// 
    /// Implemented as a thread safe singleton
    /// </summary>
    public sealed class MouseController
    {
        #region Singleton
        /// <summary>
        /// Lock for thread safe.
        /// </summary>
        private static readonly object mylock = new object();

        /// <summary>
        /// Unique instance.
        /// </summary>
        private static MouseController instance = null;

        /// <summary>
        /// Returns the unique instance, will create it if is the first time called.
        /// </summary>
        public static MouseController Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (mylock)
                    {
                        if (instance == null)
                        {
                            instance = new MouseController();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Private constructor for avoid creating multiple instances.
        /// </summary>
        private MouseController() { }
        #endregion

        #region External mouse control

        ///
        ///  Use of external mouse function for control the mouse.
        /// 
        ///  https://www.elitepvpers.com/forum/net-languages/118712-c-windows-api.html 
        /// 
        ///  https://stackoverflow.com/questions/8739523/directing-mouse-events-dllimportuser32-dll-click-double-click
        ///

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
        }

        /// <summary>
        /// Set position of mouse in screen.
        /// 
        ///         /// </summary>
        /// <param name="x"> mouse position</param>
        /// <param name="y">mouse positioon</param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

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
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        #endregion


        #region public acces methods 
        ///
        /// Public access methods for controlling the mouse.
        ///

       
        public void SetPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }
        /// <summary>
        /// Right button down        
        /// </summary>
        public void RightDown()
        {
            mouse_event((uint)MouseEventFlags.RIGHTDOWN, 0, 0, 0, 0);
        }

        /// <summary>
        /// Right button up
        /// </summary>
        public void RightUp()
        {
            mouse_event((uint)MouseEventFlags.RIGHTUP, 0, 0, 0, 0);
        }
       
        /// <summary>
        /// Left mouse button down.
        /// </summary>
        public void LeftDown()
        {
            mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
        }

        /// <summary>
        /// Left Mouse button down
        /// </summary>
        public void LeftUp()
        {
            mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
        }



        #endregion

    }
}
