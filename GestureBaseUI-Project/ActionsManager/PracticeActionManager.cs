using GestureBaseUI_Project.ViewModel;
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

        public PracticeActionManager(PracticeTutorialViewModel practiceTutorialViewModel)
        {
            this.practiceTutorialViewModel = practiceTutorialViewModel;
            model = new Model(System.IO.Path.Combine(Environment.CurrentDirectory, @"CNN\Model\gesture_model1.pb"));
        }


        public override void Close()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(12);
           
        }

        public override void Cow()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(11);
        }

        public override void Four()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(5);
        }

        public override void Moving()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(1);

        }

        public override void One()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(2);
        }

        public override void Ready()
        {
            practiceTutorialViewModel.SetReady(true);
            practiceTutorialViewModel.SetActionDone(-1);
        }

        public override void Surf()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(10);
        }

        public override void Three()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(4);
        }

        public override void Two()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(3);
        }

       

        public override void WaveDown()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(6);
        }

        public override void WaveLeft()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(8);
        }

        public override void WaveRigth()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(9);
        }

        public override void Waveup()
        {
            practiceTutorialViewModel.SetReady(false);
            practiceTutorialViewModel.SetActionDone(7);
        }

        private bool ready = false;
        private int actual = 5;
        private PracticeTutorialViewModel practiceTutorialViewModel;

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
                    this.Ready();
                    ready = true;
                   
                    //this.actions[actual.ToString()]?.Invoke();
                    //Thread.Sleep(500);
                }/*
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
                }*/
            }
        }

        public void SetMinForChange(int min)
        {
            counter.SetAllMin(min);
        }
    }
}
