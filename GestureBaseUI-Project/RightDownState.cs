using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GestureBaseUI_Project
{
    public class RightDownState : AState
    {
        private State myState = State.RightDown;
        public override State GetStateName()
        {
            return myState;
        }

        public override void OnEnter(IState from)
        {
            Debug.WriteLine(myState + " on enter");
        }

        public override void OnExit(IState to)
        {
            Debug.WriteLine(myState + " on exite");
        }

        public override void Update()
        {
            Debug.WriteLine(myState + "Update");
        }
    }
}
