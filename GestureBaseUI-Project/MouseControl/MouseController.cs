using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using static GestureBaseUI_Project.Win32Wrapper.MouseW32W;
using static GestureBaseUI_Project.Win32Wrapper.WindowW32W;

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
        private MouseController()
        {
            LoadAllActiveWindows();
        }
        #endregion

        #region External mouse control

        ///
        ///  Use of external mouse function for control the mouse.
        /// 
        ///  https://www.elitepvpers.com/forum/net-languages/118712-c-windows-api.html 
        /// 
        ///  https://stackoverflow.com/questions/8739523/directing-mouse-events-dllimportuser32-dll-click-double-click
        ///


        private List<string> Windows = new List<string>();
        public List<IntPtr> WindowsPtrs = new List<IntPtr>();




        /// <summary>
        /// makes a screenshot of your current desktop and returns a bitmap
        /// </summary>
        /// <returns></returns>
        public static Bitmap CreateScreenshot()//IntPtr hWnd)
        {
            var hWnd = MouseController.Instance.WindowsPtrs[0];
            IntPtr hSorceDC = GetWindowDC(hWnd);
            RECT rect = new RECT();
            GetWindowRect(hWnd, ref rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;
            IntPtr hDestDC = CreateCompatibleDC(hSorceDC);
            IntPtr hBitmap = CreateCompatibleBitmap(hSorceDC, width, height);
            IntPtr hObject = SelectObject(hDestDC, hBitmap);
            BitBlt(hDestDC, 0, 0, width, height, hSorceDC, 0, 0, SRCCOPY);
            SelectObject(hDestDC, hObject);
            DeleteDC(hDestDC);
            ReleaseDC(hWnd, hSorceDC);
            Bitmap screenshot = Bitmap.FromHbitmap(hBitmap);
            DeleteObject(hBitmap);
            return screenshot;
        }



        private bool AddWnd(int hwnd, int lparam)
        {

            if (IsWindowVisible(hwnd))
            {

                StringBuilder sb = new StringBuilder(255);
                GetWindowText(hwnd, sb, sb.Capacity);
                if (sb.ToString().Contains("-"))
                {
                    WindowsPtrs.Add((IntPtr)hwnd);
                }
                Debug.WriteLine(sb);
                Windows.Add(sb.ToString());
            }
            return true;
        }

        private void LoadAllActiveWindows()
        {
            WindowsPtrs = new List<IntPtr>();
            EnumWindows(new WindowEnumCallback(this.AddWnd), 0);
        }

        #endregion


        #region public acces methods 

        int w = 0;
        public String NextWindows()
        {



            bool h = SetForegroundWindow(WindowsPtrs[w++ % WindowsPtrs.Count]);

            //  var active=  SetActiveWindow(nextWindow);
            return " " + h + " active";
        }

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
        #endregion



    }
}
