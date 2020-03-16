using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Media;

namespace GestureBaseUI_Project
{
    public class MovingState : AState
    {
        State myState = State.Moving;
        private Prediction prediction;

        public MovingState(Prediction prediction)
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
            prediction.SetColor(Colors.Red);
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
