using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace GestureBaseUI_Project
{
    public class MousePositionController
    {

        private const float MIN_MOVE = 10;
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


        public void startMoving(Vector2 startMousePosition, Vector2 startHandPosition)
        {
            this._startMousePosition = startMousePosition;
            this._startHandPosition = startHandPosition;
            this._lastHandPosition = startHandPosition;
        }
        public Vector2 getNextPosition(Vector2 newHandPosition)
        {
            int newX = 0;
            float dx = newHandPosition.X - _startHandPosition.X;
            //_startMousePosition = newHandPosition;
            float abs_dx = Math.Abs(dx);
            Debug.WriteLine(dx);
            _startHandPosition = newHandPosition;
            if (abs_dx < MIN_MOVE || abs_dx > MAX_MOVE)
            {

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
                    return _startMousePosition;
                }
            }else
           
            if (dx > 0)//left
            {
                if (_horizontalState == HorizontalState.Right)
                {
                    //  Debug.WriteLine("Idle");
                    _horizontalState = HorizontalState.Idle;
                    return _startMousePosition;
                }
                else
                    if (_horizontalState == HorizontalState.Left)
                {
                    //  Debug.WriteLine("Move LEFT");
                    _horizontalState = HorizontalState.Left;
                    newX = (int)_startMousePosition.X - movedistance;
                }
                else
                {
                    //  Debug.WriteLine("Move LEFT");
                    _horizontalState = HorizontalState.Left;
                    newX = (int)_startMousePosition.X - movedistance;
                }



            }
            else
                if (dx < 0)///right
            {
                if (_horizontalState == HorizontalState.Left)
                {
                    //  Debug.WriteLine("Idle");
                    _horizontalState = HorizontalState.Idle;
                    return _startMousePosition;
                }
                else
                    if (_horizontalState == HorizontalState.Right)
                {
                    //  Debug.WriteLine("Move Right");
                    _horizontalState = HorizontalState.Right;
                    newX = (int)_startMousePosition.X + movedistance;
                }
                else
                {
                    //  Debug.WriteLine("Move Right");
                    _horizontalState = HorizontalState.Right;
                    newX = (int)_startMousePosition.X + movedistance;

                }


            }

            _startMousePosition.X = newX;
            return new Vector2(newX, 0);
        }
    }

}
enum HorizontalState
{
    Left, Right, Idle
}