using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GestureBaseUI_Project.ActionsManager
{
    public class PracticeActionManager : AbstractActionManager
    {
        /// <summary>
        /// Model for predicition.
        /// </summary>
        private readonly Model model;

        /// <summary>
        /// Counts predictions for decide when to change
        /// </summary>
        private readonly ActionCounter counter = new ActionCounter();

        public PracticeActionManager()
        {
            model = new Model(System.IO.Path.Combine(Environment.CurrentDirectory, @"CNN\Model\gesture_model1.pb"));
        }
        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Cow()
        {
            throw new NotImplementedException();
        }

        public override void Four()
        {
            throw new NotImplementedException();
        }

        public override void Moving()
        {
            throw new NotImplementedException();
        }

        public override void One()
        {
            throw new NotImplementedException();
        }

        public override void Ready()
        {
            throw new NotImplementedException();
        }

        public override void Surf()
        {
            throw new NotImplementedException();
        }

        public override void Three()
        {
            throw new NotImplementedException();
        }

        public override void Two()
        {
            throw new NotImplementedException();
        }

       

        public override void WaveDown()
        {
            throw new NotImplementedException();
        }

        public override void WaveLeft()
        {
            throw new NotImplementedException();
        }

        public override void WaveRigth()
        {
            throw new NotImplementedException();
        }

        public override void Waveup()
        {
            throw new NotImplementedException();
        }

        private bool ready = false;
        private int actual = 0;

        public void AddImage(float[,] image)
        {
            Update(counter.Count(model.Predict(image)));
        }
        public override void Update(int next)
        {
            if (ready)
            {

                if (next == 5) return;
                ready = false;
                actual = next;
                this.actions[actual.ToString()]?.Invoke();
            }
            else
            {
                if (next == 5)
                {
                    ready = true;
                    Debug.WriteLine("Ready");
                    //Thread.Sleep(500);
                }
                else if (actual == 0)
                {
                    Moving();
                }
                else if (actual == 6)
                {
                    WaveDown();
                }
                else if (actual == 7)
                {
                    Waveup();
                }
            }
        }
    }
}
