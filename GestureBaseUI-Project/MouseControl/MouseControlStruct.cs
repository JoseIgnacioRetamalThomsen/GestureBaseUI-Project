using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace GestureBaseUI_Project.MouseControl
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        public Int32 X;
        public Int32 Y;

        public Win32Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    };

    public struct Rectangle
    {
        public Win32Point TopLeft { get; private set; }
        public Win32Point BottomRight { get; private set; }

        public Vector2 Center { get; private set; }

        public Win32Point CenterInt { get; private set; }

        public int Width { get; private set; }
        public int Heigth { get; private set; }

        public Rectangle(Win32Point topLeft, Win32Point bottomRight) : this()
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;

            Width = Math.Abs(bottomRight.X - topLeft.X);
            Heigth = Math.Abs(bottomRight.Y - TopLeft.Y);

            Center = new Vector2((bottomRight.X - topLeft.X) / 2, (bottomRight.Y - TopLeft.X));
            CenterInt = new Win32Point() { X = (int)Math.Round(Center.X), Y = (int)Math.Round(Center.Y) };

        }
    }
}
