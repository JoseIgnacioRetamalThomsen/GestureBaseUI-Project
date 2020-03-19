using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using static GestureBaseUI_Project.Prediction;

namespace GestureBaseUI_Project
{
    public class MousePositionController
    {
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

        public MousePositionController(Vector2 screenSize)
        {
            _screenSize = screenSize;
            Debug.WriteLine("Created");
        }
        public MousePositionController(int screenWidth, int screenHeight, int mouseX,int mouseY)
        {
            this.width = screenWidth;
            this.heigth = screenHeight;
            
            this.mouseLastPosition = new Win32Point{ X = mouseX,Y= mouseY};


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
        public Prediction.Win32Point getNextPosition(Vector2 newHandPosition)
        {
          
            
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