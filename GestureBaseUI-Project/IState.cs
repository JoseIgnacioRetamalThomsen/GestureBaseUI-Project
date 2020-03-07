using System;
using System.Collections.Generic;
using System.Text;

namespace GestureBaseUI_Project
{
    public interface IState
    {
        void OnEnter(IState from);
        void OnExit(IState to);

        void Update();

        State GetStateName();

    }
}

public enum State
{
    Moving, RightDown, LeftDown, Scrolling, Action
}