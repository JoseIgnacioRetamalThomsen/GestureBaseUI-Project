using System;
using System.Diagnostics;
using System.Numerics;
using FLS;
using FLS.Rules;


namespace GestureBaseUI_Project
{
    public class HandPositionMapper
    {
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

        FuzzyMouseSpeed fms;
        public HandPositionMapper(MyRect screen, MyRect moveArea)
        {
            this.screen = screen;
            this.moveArea = moveArea;

            relationX = screen.Width / moveArea.Width;
            relationY = screen.Height / moveArea.Height;

            fms = new FuzzyMouseSpeed();
        }

        private int width, heigth;
        
        private Win32Point mouseLastPosition;
        private Win32Point mouseNextPosition;
        private int nextX, nextY;

        private const float MIN_MOVE = 2;
        private const float MAX_MOVE = 100;

        private Vector2 _screenSize;
        private Vector2 _startMousePosition;
       

        private Vector2 _lastHandPosition;
        private Vector2 _startHandPosition;
        private Vector2 _change;
        HorizontalState _horizontalState = HorizontalState.Idle;

        private int movedistance = 20;

       

        public HandPositionMapper(Vector2 screenSize)
        {


            _screenSize = screenSize;
            Debug.WriteLine("Created");

             //variation = new LinguisticVariable("Variation");

        }
      


        
        public void startMoving(Vector2 startMousePosition, Vector2 startHandPosition)
        {
            this._startMousePosition = startMousePosition;
            this._startHandPosition = startHandPosition;
            this._lastHandPosition = startHandPosition;
        }

        private bool Moveright(int x)
        {
            int distanceRemaining = this.width- mouseLastPosition.X;
           // Debug.WriteLine("rem: "+distanceRemaining);
            int distanceToMove = (distanceRemaining * x)/100;
            mouseNextPosition.X = mouseLastPosition.X + distanceToMove;
            return true;
        }
        private bool MoveLeft(int x)
        {
            int distanceRemaining = this.mouseLastPosition.Y;
           // Debug.WriteLine("rem: " + distanceRemaining);
            int distanceToMove = (distanceRemaining * x) / 100;
            mouseNextPosition.X = mouseLastPosition.X - distanceToMove;
            return true;
        }
        int moveMult = 2;
       

        int x_boundary_left = 600;
        int x_boundary_right = 200;
        private Vector2 oldHandPosition = Vector2.Zero;
        private Win32Point last = new Win32Point { 
        X=0,Y=0};
        bool f = true;


        public Win32Point getNextPosition1(Vector2 newHandPosition)
        {
            Vector2  t= screen.Center + (newHandPosition - moveArea.Center) * 2;
            return new Win32Point() { 
            X= (int)t.X,Y=(int)t.Y};
        }


        private Vector2 startPosition = new Vector2();
        Win32Point lastPosition = new Win32Point() { };
        public void StartMoving(Vector2 handPosition)
        {
            lastPosition = MouseController.Instance.GetMousePosition();
            startPosition = handPosition;

            next.X = lastPosition.X;
            next.Y = lastPosition.Y;
            Debug.WriteLine("mouse position + x:" + lastPosition.X + " y: " + lastPosition.Y);
        }

        Win32Point next = new Win32Point();

        public Win32Point GetNextDis(Vector2 newHandPosition)
        {
            float dx = newHandPosition.X - startPosition.X;
            float dy = newHandPosition.Y - startPosition.Y;
            float dxA = Math.Abs(dx);
            float dyA = Math.Abs(dy);

            Debug.WriteLine("Dx: " + dx);



            int dxspeed = (int)fms.Get(dxA);

            Debug.WriteLine(dxspeed);
            /* if (dxA < 70)
            {
                dxspeed = 10;
            }
            else if (dxA < 90)
            {
                dxspeed = 20;
            } else if (dxA < 110)
            {
                dxspeed = 30;
            } else if (dxA < 150)
            {
                dxspeed = 60;
            } /*else if (dxA < 35)
            {
                dxspeed = 30;

            } else if (dxA < 50)
            {
                dxspeed = 40;
            } else if (dxA < 70)
            {
                dxspeed = 50;
            }
            else if (dxA < 100)
            {
                dxspeed = 79;
            } else {
                dxspeed = 100;
            }
            */


            int dyspeed = (int)fms.Get(dyA);
            /*     if (dyA < 70)
                  {
                      dyspeed = 10;
                  }
                  else if (dyA < 90)
                  {
                      dyspeed = 20;
                  }
                  else if (dyA < 110)
                  {
                      dyspeed = 30;
                  }
                  else if (dyA < 150)
                  {
                      dyspeed = 60;
                  }/*
                  else if (dyA < 35)
                  {
                      dyspeed = 30;

                  }
                  else if (dyA < 50)
                  {
                      dyspeed = 40;
                  }
                  else if (dyA < 70)
                  {
                      dyspeed = 50;
                  }
                  else if (dyA < 100)
                  {
                      dyspeed = 79;
                  }
                  else
                  {
                      dyspeed = 100;
                  }

                  */

            if (dxA > 20)
            {
                if (dx < 0)
                {
                    next.X = lastPosition.X + dxspeed;//(int)(dxA * 0.2);
                }
                else if (dx > 0)
                {
                    next.X = lastPosition.X - dxspeed;//(int)(dxA * 0.2);
                }
            }
            
            if (dyA > 20)
            {
                if(dy < 0 )
                {
                    next.Y = lastPosition.Y - dyspeed; //(int)(dyA * 0.2);
                }else if(dy > 0)
                {
                    next.Y = lastPosition.Y + dyspeed; //(int)(dyA * 0.2);
                }
            }

            if (next.X > screen.Right)
            {
                next.X = (int)screen.Right;
            }else if(next.X < screen.Left)
            {
                next.X = 0;
            }

           
            if (next.Y > screen.Bottom)
            {
                next.Y = (int)screen.Bottom;
            }
            else if (next.Y < 0)
            {
                next.Y = 0;
            }

            lastPosition = next;
            return next;
        }

        public Win32Point getNextPosition(Vector2 newHandPosition)
        {
            if(Math.Abs(oldHandPosition.Y- newHandPosition.Y) >500 || Math.Abs(oldHandPosition.X - newHandPosition.X) > 500
            || Math.Abs(oldHandPosition.Y- newHandPosition.Y) <2 || Math.Abs(oldHandPosition.X - newHandPosition.X) < 2)
                {
              
                if (f)
                {
                    oldHandPosition = newHandPosition;
                    f = false;
                }
                //oldHandPosition = newHandPosition;
                return last;
            }

           
            float xh = -(moveArea.Left+newHandPosition.X);
            float yh =   -(moveArea.Top -newHandPosition.Y);
         
            float ys = yh * relationY*(0.7F) -2.5F;
            float xs = xh * relationX * (0.7F) - 2.5F;

            oldHandPosition = newHandPosition;
            last.X = (int)xs;
            last.Y = (int)ys;
            return last;
            int newX = 0;
            float dx = newHandPosition.X - _startHandPosition.X;
            //_startMousePosition = newHandPosition;
            float abs_dx = Math.Abs(dx);
            Debug.WriteLine(abs_dx);

            int moveDistance = 0;
            if (abs_dx < 5)
            {
                moveDistance = 1;
            }else
                if(abs_dx<10)
            {
                moveDistance = 5;

            }else
            if (abs_dx<20)
            {
                moveDistance = 30;
            }
            else
            if(abs_dx <30)
            {
                moveDistance = 70;
            }
            else
            if(abs_dx<90)
            {
                moveDistance = 90;
            }
            else
            {
                moveDistance = 0;
            }
            //Debug.WriteLine(dx);
            _startHandPosition = newHandPosition;
            if (abs_dx < MIN_MOVE || abs_dx > MAX_MOVE)
            {
                return mouseNextPosition;/*
                if (_horizontalState == HorizontalState.Right)
                {
                    _horizontalState = HorizontalState.Right;
                    newX = (int)_startMousePosition.X + movedistance;
                }
                else
                    if (_horizontalState == HorizontalState.Left)
                {
                    // Debug.WriteLine("Move LEFT");
                    _horizontalState = HorizontalState.Left;
                    newX = (int)_startMousePosition.X - movedistance;

                }
                else
                {

                    //  Debug.WriteLine("Idle");
                    _horizontalState = HorizontalState.Idle;
                    return mouseLastPosition;
                }*/
            }else
           
            if (dx > 0)//left
            {
                _horizontalState = HorizontalState.Left;
                MoveLeft(moveDistance);
                /*
                if (_horizontalState == HorizontalState.Right)
                {
                    //  Debug.WriteLine("Idle");
                    _horizontalState = HorizontalState.Idle;
                    return mouseLastPosition;
                }
                else
                    if (_horizontalState == HorizontalState.Left)
                {
                    //  Debug.WriteLine("Move LEFT");
                    _horizontalState = HorizontalState.Left;
                    MoveLeft(moveDistance);
                }
                else
                {
                    //  Debug.WriteLine("Move LEFT");
                    _horizontalState = HorizontalState.Left;
                    MoveLeft(moveDistance);
                }
                */


            }
            else
                if (dx < 0)///right
            {
                _horizontalState = HorizontalState.Idle;
                Moveright(moveDistance);
                /*
                if (_horizontalState == HorizontalState.Left)
                {
                    //  Debug.WriteLine("Idle");
                    _horizontalState = HorizontalState.Idle;
                    return mouseLastPosition;
                }
                else
                    if (_horizontalState == HorizontalState.Right)
                {
                    //  Debug.WriteLine("Move Right");
                    _horizontalState = HorizontalState.Right;
                    Moveright(moveDistance);
                }
                else
                {
                    //  Debug.WriteLine("Move Right");
                    _horizontalState = HorizontalState.Right;
                    Moveright(moveDistance);

                }

    */
            }

            mouseLastPosition.X = mouseNextPosition.X;
            return mouseNextPosition;
        }
    }

}
enum HorizontalState
{
    Left, Right, Idle
}