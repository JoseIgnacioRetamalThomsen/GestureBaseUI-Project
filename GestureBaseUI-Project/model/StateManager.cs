using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GestureBaseUI_Project.model
{
    public class StateManager
    {
        //static public StateManager instance { get { return s_Instance; } }
      //  static protected StateManager s_Instance;

        private Model model;

        protected List<AState> states_stack = new List<AState>();
        protected Dictionary<State, AState> m_StateDict = new Dictionary<State, AState>();


        public StateManager(AState[] states)
        {
            model = new Model(System.IO.Path.Combine(Environment.CurrentDirectory, @"model\gesture_model.pb"));

            // add all states
            for (int i = 0; i < states.Length; ++i)
            {
                states[i].stateManager = this;
                m_StateDict.Add(states[i].GetStateName(), states[i]);

                // add starting state to stack
                states_stack.Clear();
                PushState(states[0].GetStateName());
            }
        }

        public void AddImage(float[,] image)
        {

            float[] imageLast = new float[900];
            int ii = 0;
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    imageLast[ii++] = image[i, j];
                }
            }
           
            var re = model.Predict(imageLast);
            foreach (float f in re)
            {
                Debug.Write(f + " " );
            }
            Debug.WriteLine("");
        }

        private void PushState(State s)
        {
            AState state;
            if (!m_StateDict.TryGetValue(s, out state))
            {
                Debug.WriteLine("Can't find the state named " + s);
                return;
            }

            //check if there is something in the stack
            if (states_stack.Count > 0)
            {
                // call on exit in actua state
                states_stack[states_stack.Count - 1].OnExit(state);

                /// call on enter on new state using the old state
                state.OnEnter(states_stack[states_stack.Count - 1]);
            }
            else
            {
                // coming from no state
                state.OnEnter(null);
            }

            // push new state
            states_stack.Add(state);
        }
    }
}
