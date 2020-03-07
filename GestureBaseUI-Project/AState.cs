using GestureBaseUI_Project.model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestureBaseUI_Project
{
    public abstract class AState : IState
    {
        //implemtents inteface
        public abstract State GetStateName();
        public abstract void OnEnter(IState from);
        public abstract void OnExit(IState to);
        public abstract void Update();

        public StateManager stateManager;
    }
}
