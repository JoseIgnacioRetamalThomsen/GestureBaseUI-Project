using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static GestureBaseUI_Project.Win32Wrapper.WindowW32W;

namespace GestureBaseUI_Project
{
    /// <summary>
    /// Define a rectangle that can represent a screen or a area for moving the hands.
    /// Follows the structure of the win32 RECT structure.
    /// https://docs.microsoft.com/en-us/windows/win32/api/windef/ns-windef-rect
    /// </summary>
    public struct MyRect
    {
        /// <summary>
        /// Specifies the x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public float Left { get; private set; }

        /// <summary>
        /// Specifies the y-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public float Top { get; private set; }

        /// <summary>
        /// Specifies the x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public float Right { get; private set; }

        /// <summary>
        /// Specifies the y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public float Bottom { get; private set; }

        public float DeltaX { get; private set; }
        public float DeltaY { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public Vector2 Center
        {
            get; private set;

        }

        public MyRect(RECT rect )
        {
            Left = rect.left;
            Top = rect.top;
            Right = rect.right;
            Bottom = rect.bottom;

            DeltaX = rect.right - rect.left;
            DeltaY = rect.bottom - rect.top;
            Width = Math.Abs(DeltaX);
            Height = Math.Abs(DeltaY);
            Center = new Vector2(Left + DeltaX * 0.5f, Top + DeltaY * 0.5f);
        }

        public MyRect(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;

            DeltaX = right - left;
            DeltaY = bottom - top;
            Width = Math.Abs(DeltaX);
            Height = Math.Abs(DeltaY);
            Center = new Vector2(Left + DeltaX * 0.5f, Top + DeltaY * 0.5f);
        }

        public override string ToString()
        {
            return Left.ToString() + ", " + Top.ToString() + ", " + Right.ToString() + ", " + Bottom.ToString();
        }
    }
}
