using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

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
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, int dwExtraInfo);

        //https://stackoverflow.com/a/4232281
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]

        internal static extern bool GetCursorPos(ref Win32Point pt);



        /// <summary>
        /// Modified from http://www.pinvoke.net/default.aspx/user32/GetWindow.html
        /// Retrieves a handle to a window that has the specified relationship (Z-Order or owner) to the specified window.
        /// </summary>
        /// <remarks>The EnumChildWindows function is more reliable than calling GetWindow in a loop. An application that
        /// calls GetWindow to perform this task risks being caught in an infinite loop or referencing a handle to a window
        /// that has been destroyed.</remarks>
        /// <param name="hWnd">A handle to a window. The window handle retrieved is relative to this window, based on the
        /// value of the uCmd parameter.</param>
        /// <param name="uCmd">The relationship between the specified window and the window whose handle is to be
        /// retrieved.</param>
        /// <returns>
        /// If the function succeeds, the return value is a window handle. If no window exists with the specified relationship
        /// to the specified window, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

        private enum GetWindowType : uint
        {
            /// <summary>
            /// The retrieved handle identifies the window of the same type that is highest in the Z order.
            /// <para/>
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDFIRST = 0,
            /// <summary>
            /// The retrieved handle identifies the window of the same type that is lowest in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDLAST = 1,
            /// <summary>
            /// The retrieved handle identifies the window below the specified window in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDNEXT = 2,
            /// <summary>
            /// The retrieved handle identifies the window above the specified window in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDPREV = 3,
            /// <summary>
            /// The retrieved handle identifies the specified window's owner window, if any.
            /// </summary>
            GW_OWNER = 4,
            /// <summary>
            /// The retrieved handle identifies the child window at the top of the Z order,
            /// if the specified window is a parent window; otherwise, the retrieved handle is NULL.
            /// The function examines only child windows of the specified window. It does not examine descendant windows.
            /// </summary>
            GW_CHILD = 5,
            /// <summary>
            /// The retrieved handle identifies the enabled popup window owned by the specified window (the
            /// search uses the first such window found using GW_HWNDNEXT); otherwise, if there are no enabled
            /// popup windows, the retrieved handle is that of the specified window.
            /// </summary>
            GW_ENABLEDPOPUP = 6
        }


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetForegroundWindow();


        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        public delegate bool WindowEnumCallback(int hwnd, int lparam);
        //https://stackoverflow.com/a/10819641
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(WindowEnumCallback lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        public static extern void GetWindowText(int h, StringBuilder s, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(int h);

        private List<string> Windows = new List<string>();
        public List<IntPtr> WindowsPtrs = new List<IntPtr>();

        //scren show

        private const int SRCCOPY = 0x00CC0020;
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hObject, int nXDest, int
nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int
nYSrc, int dwRop);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
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
