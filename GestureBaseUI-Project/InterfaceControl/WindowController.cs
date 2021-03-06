﻿using GestureBaseUI_Project.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using static GestureBaseUI_Project.Win32Wrapper.WindowW32W;

namespace GestureBaseUI_Project
{
    /// <summary>
    /// Control active window
    /// </summary>
    public class WindowController
    {
        #region Singleton
        /// <summary>
        /// Lock for thread safe.
        /// </summary>
        private static readonly object mylock = new object();

        /// <summary>
        /// Unique instance.
        /// </summary>
        private static WindowController instance = null;

        private IntPtr _activeWindow;

        /// <summary>
        /// Returns the unique instance, will create it if is the first time called.
        /// </summary>
        public static WindowController Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (mylock)
                    {
                        if (instance == null)
                        {
                            instance = new WindowController();
                        }
                    }
                }
                return instance;
            }
        }

        private IntPtr AppWindow { get; set; }

        /// <summary>
        /// Private constructor for avoid creating multiple instances.
        /// </summary>
        private WindowController()
        {
           
        }
        #endregion

    

        private List<string> Windows = new List<string>();
        public List<ProcessLink> WindowsPtrs = new List<ProcessLink>();

        int w = 0;

        /// <summary>
        /// makes a screenshot of your current desktop and returns a bitmap
        /// </summary>
        /// <returns></returns>
        public  Bitmap CreateScreenshot(IntPtr hWnd)
        {
            
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

        /// <summary>
        /// Set the app winwos to actuals window
        /// </summary>
        public void SetAppWindow()
        {
            AppWindow = GetForegroundWindow();
        }

        public bool SetAppWindowsActive()
        {
            if (AppWindow != null)
            {
                SetForegroundWindow(AppWindow);
                ShowWindowNormal(AppWindow);
                if(_activeWindow == AppWindow)
                {
                    return true;

                }else
                {
                    _activeWindow = AppWindow;
                    return false;
                }
               
            }
            else
            {
                return false;
            }
        }

        public void OpenWindow(IntPtr ptr)
        {
            SetForegroundWindow(ptr);
            MaximazeWindow(ptr);
            _activeWindow = ptr;
        }
       


        private bool AddWnd(int hwnd, int lparam)
        {

            if (IsWindowVisible(hwnd))
            {

               StringBuilder sb = new StringBuilder(255);
                GetWindowText(hwnd, sb, sb.Capacity);
                if (sb.ToString().Contains("-"))
                {
                    var temp = sb.ToString().Split("-");
                    WindowsPtrs.Add(new ProcessLink()
                    {
                        Windows = (IntPtr)hwnd,
                        Title = temp[temp.Length - 1].Trim()

                    }

                       ); ;
                    Debug.WriteLine(sb);
                }
                
                Windows.Add(sb.ToString());
            }
            return true;
        }


        private void LoadAllActiveWindows()
        {
            WindowsPtrs = new List<ProcessLink>();
            EnumWindows(new WindowEnumCallback(this.AddWnd), 0);
        }

        public List<ProcessLink> GetAllActiveWindows()
        {
            LoadAllActiveWindows();

            List<ProcessLink> list = new List<ProcessLink>();

            foreach(ProcessLink i in WindowsPtrs)
            {
                Bitmap temp = CreateScreenshot(i.Windows);
                i.Image = temp;
                list.Add(i) ;
            }

            return list;
        }

        public void CloseActualWindow()
        {
            var aw = GetForegroundWindow();
            if (aw != AppWindow)
            {
                ShowWindow(aw, 0);
            }
        }

        public IntPtr GetActiveWindow()
        {
            return GetForegroundWindow();
        }

        public void SetActiveWindows(IntPtr ptr)
        {
            SetForegroundWindow(ptr);
            MaximazeWindow(ptr);
        }

        public void MaximazeWindow(IntPtr ptr)
        {
            ShowWindow(ptr, 3);
        }

        public void MinimazeWindows(IntPtr ptr)
        {
            ShowWindow(ptr, 6);
        }

        public void CloseWindow(IntPtr ptr)
        {
            ShowWindow(ptr, 0);
        }

        public void ShowWindowNormal(IntPtr ptr)
        {
            ShowWindow(ptr, 1);
        }

        public void OpenNew(string fullPath)
        {
            Process.Start(fullPath);
          
            
        }
    }
}
