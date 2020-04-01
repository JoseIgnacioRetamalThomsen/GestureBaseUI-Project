using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using static GestureBaseUI_Project.Win32Wrapper.MouseW32W;
using static GestureBaseUI_Project.Win32Wrapper.WindowW32W;

namespace GestureBaseUI_Project
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
        private MouseController()
        {
          
        }
        #endregion
                  
        #region public acces methods 

      
       

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
            mouse_event((uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
        }


        public void Click()
        {
            mouse_event((uint)MouseEventFlags.LEFTDOWN | (uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
        }

        public void ScrolDown(int amount)
        {


            mouse_event((uint)MouseEventFlags.WHEEL, 0, 0, -amount, 0);

        }

        public void ScrolUp(int amount)
        {
            mouse_event((uint)MouseEventFlags.WHEEL, 0, 0, amount, 1);
        }

        public void DoubleClick()
        {
            mouse_event((uint)MouseEventFlags.LEFTDOWN | (uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
            mouse_event((uint)MouseEventFlags.LEFTDOWN | (uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);

        }

        internal Win32Point GetMousePosition()
        {
            Win32Point ptr = new Win32Point();
            GetCursorPos(ref ptr);

            return ptr;
        }
        #endregion

    }
}
