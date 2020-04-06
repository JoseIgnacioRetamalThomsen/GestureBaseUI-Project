using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace GestureBaseUI_Project.model
{
    public class StateManager
    {

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private HandPositionMapper mouseController;

        private string statusText;
        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }


        //static public StateManager instance { get { return s_Instance; } }
        //  static protected StateManager s_Instance;

        private Model model;

      
        

        private  Vector3 position = new Vector3();
        Prediction pred;
        int tc = 0;
        public StateManager(Prediction prediction, HandPositionMapper mousecontroller)
        {
            pred = prediction;

            this.mouseController = mousecontroller;

            model = new Model(System.IO.Path.Combine(Environment.CurrentDirectory, @"Model\gesture_model1.pb"));

            // add all states
        }
        float[] p = new float[13];
        bool isFirst = true;
        float speed = 10f;
        bool movingRight = false;
        bool movingUp = false;
        public Vector3 Position { get => position; }
        Vector3 StartPosition = Vector3.Zero;
        bool isFirstPosition = true;
        Vector3 MousePosition = Vector3.Zero;
        HandPositionMapper mc;

        public void SetPosition(Vector3 newpos)
        {
            Win32Point temp = mouseController.getNextPosition(new Vector2(newpos.X, newpos.Y));
            //pred.SetPointPosition(1920, 1200);
          //  pred.SetPointPosition(temp);
            MouseController.Instance.SetPosition(temp.X,temp.Y);

            return;
        }

       
            /*
            float newX = newpos.X;
            float newY = newpos.Y;
            float moveDistance=0;

            if (isFirstPosition)
            {
                StartPosition = newpos;
                MousePosition.X = 600;
                pred.SetPointPosition(600, 600);
                isFirstPosition = false;
                return;
            }

            //horizontal
            // move to left
            float dx = newpos.X - StartPosition.X;
           
            Debug.WriteLine("dx" + dx +" x:"  + newpos.X);
            float dxa = Math.Abs(dx);
            if (dxa < 5 || dxa > 50) return;

            if(dxa < 10)
            {
                moveDistance = 1;
            }else if(dxa < 20)
            {
                moveDistance = 4;
            }else if(dxa < 30)
            {
                moveDistance = 20;

            }
            else 
            {
                moveDistance = 80;
            }
            if (dx > 0)
            {

                movingRight = true;
                newX = (StartPosition.X + speed);
                //StartPosition.X = newpos.X;

                pred.SetPointPosition(MousePosition.X - moveDistance * speed, 600);
                MousePosition.X = MousePosition.X - moveDistance * speed;
                StartPosition = newpos;
          
                
                

            }
            else
            {

                movingRight = true;
                newX = (StartPosition.X + moveDistance);
                //StartPosition.X = newpos.X;

                pred.SetPointPosition(MousePosition.X + moveDistance*speed, 600);
                MousePosition.X = MousePosition.X + moveDistance*speed;
                StartPosition = newpos;
                return;

            }
            if (tc >= 2)
            {
                PushState(State.RightDown);
                tc = 0;
            }
            switch (this.getActualState())
            {
                case 0:
                    //pred.SetColor(Colors.Red);
                    PushState(State.Moving);
                    break;
                case 3:
                   // PushState(State.RightDown);
                    tc++;
                    // pred.SetColor(Colors.Yellow);
                    break;
                case 5:
                    tc++;
                  //  PushState(State.RightDown);
                    //  pred.SetColor(Colors.Yellow);
                    break;
                default:
                    if(tc > 0)
                    {
                        tc--;
                    }
                   
                    break;
            }
            return;
            float dy = newpos.Y - StartPosition.Y;
            if(dy>0)
            {
                movingUp = true;
                newpos.Y = (StartPosition.Y + speed * speed);
                StartPosition.Y = newpos.Y;
            }
            else
            {
                newY = (StartPosition.Y - speed * speed);
                StartPosition.Y = newpos.Y;
            }
                
            pred.SetPointPosition(newX, newY);

            this.position = newpos;
        }
        */
        private int actualState = 0;
        public int getActualState()
        {
            return actualState;
        }

        /*
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
            int i1 = 0;
            foreach (float f in re)
            {
                p[i1++] = f;
            }
            float max = p.Max();
            int m = Array.IndexOf(p, max);
            //StatusText = m.ToString();
            // Debug.WriteLine(m);
            actualState = m;

            //Debug.WriteLine(states_stack[states_stack.Count - 1].GetStateName());
            /*
            if (m >= 5 && states_stack[states_stack.Count - 1].GetStateName() != State.RightDown)
            {
                PushState(State.RightDown);
            }
            else if(m<5 && states_stack[states_stack.Count - 1].GetStateName() != State.Moving)
            {
                PushState(State.Moving);
            }*/
            /*
            if(m== 0)
            {
                
            }else
            if(m==3)
            {
               
            }*/
          /*  if (states_stack.Count > 0)
            {
                states_stack[states_stack.Count - 1].Update();
            }*/

            /* switch (m)
             {
                 case 0:
                     pred.SetColor(Colors.Black);
                     break;
                 case 3:
                     pred.SetColor(Colors.Red);
                     break;
             }*/

            //  Debug.WriteLine(Position);
            /*
            if (isFirst )
            {
                int i = 0;
                foreach (float f in re)
                {
                    p[i++] = f;
                }
                isFirst = false;
            }
            else
            {
                int i = 0;
                foreach (float f in re)
                {
                    p[i++] = (p[i] + f)/2;
                }
            }


            foreach (float f in p)
            {
                Debug.Write(f + " ");
            }
            Debug.WriteLine("");
            
        }

    */
      
    }
}
