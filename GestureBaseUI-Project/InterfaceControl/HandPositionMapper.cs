using System;
using System.Diagnostics;
using System.Numerics;
using FLS;
using FLS.Rules;


namespace GestureBaseUI_Project
{
    public class HandPositionMapper
    {

        private const int MIN_X = 20;
        private const int MIN_Y = 20;
        /// <summary>
        /// The screen area
        /// </summary>
        private MyRect screen;

        /// <summary>
        /// The hand area
        /// </summary>
        private MyRect moveArea;

        /// <summary>
        /// x constant for scalate from move area to screen area.
        /// </summary>
        private float relationX;

        /// <summary>
        /// y constant for scalere from move area to screen area.
        /// </summary>
        private float relationY;

        Win32Point next = new Win32Point();

        FuzzyMouseSpeed fms;

        private Vector2 oldHandPosition = Vector2.Zero;

        private Vector2 startPosition = new Vector2();

        Win32Point lastPosition = new Win32Point() { };

        private Win32Point last = new Win32Point
        {
            X = 0,
            Y = 0
        };

        bool f = true;
        public HandPositionMapper(MyRect screen, MyRect moveArea)
        {
            this.screen = screen;
            this.moveArea = moveArea;

            relationX = screen.Width / moveArea.Width;
            relationY = screen.Height / moveArea.Height;

            fms = new FuzzyMouseSpeed();
        }

        // start movement when using scalling movement
        public void StartMoving(Vector2 handPosition)
        {
            lastPosition = MouseController.Instance.GetMousePosition();
            startPosition = handPosition;

            next.X = lastPosition.X;
            next.Y = lastPosition.Y;

        }

        float dx;
        float dy;
        float dxA;
        float dyA;
        int dxspeed;
        int dyspeed;

        /// <summary>
        /// Get the next position using fuzzy logic
        /// </summary>
        /// <param name="newHandPosition"></param>
        /// <returns></returns>
        public Win32Point GetNextDis(Vector2 newHandPosition)
        {
            dx = newHandPosition.X - startPosition.X;
            dy = newHandPosition.Y - startPosition.Y;
            dxA = Math.Abs(dx);
            dyA = Math.Abs(dy);

            //get speed values 
            dxspeed = (int)fms.Get(dxA);
            dyspeed = (int)fms.Get(dyA);

            // x move
            // check if min move happens
            if (dxA > MIN_X)
            {
                //chose move direction
                if (dx < 0)
                {
                    next.X = lastPosition.X + dxspeed;
                }
                else if (dx > 0)
                {
                    next.X = lastPosition.X - dxspeed;
                }
            }

            // y move
            if (dyA > MIN_Y)
            {
                if (dy <= 0)
                {
                    next.Y = lastPosition.Y - dyspeed;
                }
                else if (dy > 0)
                {
                    next.Y = lastPosition.Y + dyspeed;
                }
            }

            // bound x to screen
            if (next.X > screen.Right)
            {
                next.X = (int)screen.Right;
            }
            else if (next.X < screen.Left)
            {
                next.X = 0;
            }

            // bound y to screen
            if (next.Y > screen.Bottom)
            {
                next.Y = (int)screen.Bottom;
            }
            else if (next.Y < 0)
            {
                next.Y = 0;
            }

            // save position
            lastPosition = next;

            // return result
            return next;
        }

        public Win32Point getNextPosition(Vector2 newHandPosition)
        {
            if (Math.Abs(oldHandPosition.Y - newHandPosition.Y) > 500 || Math.Abs(oldHandPosition.X - newHandPosition.X) > 500
            || Math.Abs(oldHandPosition.Y - newHandPosition.Y) < 2 || Math.Abs(oldHandPosition.X - newHandPosition.X) < 2)
            {

                if (f)
                {
                    oldHandPosition = newHandPosition;
                    f = false;
                }
                //oldHandPosition = newHandPosition;
                return last;
            }


            float xh = -(moveArea.Left + newHandPosition.X);
            float yh = -(moveArea.Top - newHandPosition.Y);

            float ys = yh * relationY * (0.7F) - 2.5F;
            float xs = xh * relationX * (0.7F) - 2.5F;

            oldHandPosition = newHandPosition;
            last.X = (int)xs;
            last.Y = (int)ys;
            return last;

        }
    }

}
enum HorizontalState
{
    Left, Right, Idle
}