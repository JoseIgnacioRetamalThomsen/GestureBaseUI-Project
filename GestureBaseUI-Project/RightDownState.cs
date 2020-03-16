using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Media;

namespace GestureBaseUI_Project
{
    public class RightDownState : AState
    {
        private State myState = State.RightDown;
        private Prediction prediction;

        public RightDownState(Prediction prediction)
        {
            this.prediction = prediction;
        }

        public override State GetStateName()
        {
            return myState;
        }

        public override void OnEnter(IState from)
        {
            Debug.WriteLine(myState + " on enter");
            prediction.SetColor(Colors.Yellow);
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
