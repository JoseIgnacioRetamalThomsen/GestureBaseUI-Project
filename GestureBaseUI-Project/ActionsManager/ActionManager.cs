using GalaSoft.MvvmLight.Messaging;
using GestureBaseUI_Project.ActionsManager;
using GestureBaseUI_Project.Models;
using GestureBaseUI_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace GestureBaseUI_Project
{
    public class ActionManager : AbstractActionManager
    {
        /// <summary>
        /// Model for predicition.
        /// </summary>
        private readonly Model model;

        /// <summary>
        /// Maps the hand position to mouse in the screen.
        /// </summary>
        private HandPositionMapper inputMapper;

        /// <summary>
        /// Map actions.
        /// </summary>
       // Dictionary<string, Action> actions;

        /// <summary>
        /// Counts predictions for decide when to change
        /// </summary>
        private readonly ActionCounter counter = new ActionCounter();

        MainAppViewModel _viewModel;

        /// <summary>
        /// Acttion on use.
        /// </summary>
        private int actual = 5;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mousecontroller"></param>
        /// <param name="vm"></param>
        public ActionManager(HandPositionMapper mousecontroller, MainAppViewModel vm) : base()
        {

            //InitActions();
            _viewModel = vm;
            this.inputMapper = mousecontroller;

            model = new Model(System.IO.Path.Combine(Environment.CurrentDirectory, @"CNN\Model\gesture_model1.pb"));

            WindowController.Instance.SetAppWindow();

        }


        bool ready = false;
        bool moving = false;

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
        /*
        private void InitActions()
        {
            this.actions = new Dictionary<string, Action>();
            this.actions.Add("0", Moving);
            this.actions.Add("1", One);
            this.actions.Add("2", Two);
            this.actions.Add("3", Three);
            this.actions.Add("4", Four);
            this.actions.Add("5", Ready);
            this.actions.Add("6", WaveDown);
            this.actions.Add("7", Waveup);
            this.actions.Add("8", WaveLeft);
            this.actions.Add("9", WaveRigth);
            this.actions.Add("10", Surf);
            this.actions.Add("11", Cow);
            this.actions.Add("12", Close);

        }
        */


        #region Actions Methdos
        public override void Moving()
        {
            MouseController.Instance.SetPosition(lastPoint.X, lastPoint.Y);
        }

        public override void One()
        {
            MouseController.Instance.Click();
            ready = false;
        }

        public override void Two()
        {
            MouseController.Instance.DoubleClick();
            ready = false;
        }

        public override void Three()
        {

            //C:\Users\pepe\eclipse\java-2019-09\eclipse\eclipse.exe
            Debug.WriteLine("Three");
           // Process.Start("C:/Program Files (x86)/Google/Chrome/Application/chrome.exe");

           // AskUpdate();
            ready = false;
        }

        public override void Four()
        {
            WindowController.Instance.CloseActualWindow();
            Debug.WriteLine("four");
        }

        public override void Ready()
        {
            Debug.WriteLine("Ready");
        }

        public override void WaveDown()
        {
            MouseController.Instance.ScrolDown(40);
            Debug.WriteLine("WaveDown");

        }

        public override void Waveup()
        {
            MouseController.Instance.ScrolUp(40);
            Debug.WriteLine("Wave up");

        }

        public override void WaveLeft()
        {
            IntPtr wp = WindowController.Instance.GetActiveWindow();



            int position = 0;
            for (int i = 0; i < _viewModel.Links.Count; i++)
            {
                if (_viewModel.Links[i].Windows == wp)
                {
                    position = i;
                    break;
                }
            }
            int nextPosition = (position + 1) % _viewModel.Links.Count;
            IntPtr nextP = _viewModel.Links[nextPosition].Windows;
            WindowController.Instance.SetActiveWindows(nextP);
            Debug.WriteLine("waveLeft");
        }

        public override void WaveRigth()
        {

            IntPtr wp = WindowController.Instance.GetActiveWindow();

            Debug.WriteLine("count " + _viewModel.Links.Count);

            int position = 0;
            for (int i = 0; i < _viewModel.Links.Count; i++)
            {
                if (_viewModel.Links[i].Windows == wp)
                {
                    position = i;
                    break;
                }
            }
            int nextPosition = (position + 1 + _viewModel.Links.Count) % _viewModel.Links.Count;
            IntPtr nextP = _viewModel.Links[nextPosition].Windows;
            WindowController.Instance.SetActiveWindows(nextP);
            Debug.WriteLine("WaveRight");
        }


        public override void Surf()
        {
            WindowController.Instance.OpenNew(App.userdata.ShortCut1);
        }

        public override void Cow()
        {
            WindowController.Instance.OpenNew(App.userdata.ShortCut2);
        }

        public override void Close()
        {
            if (WindowController.Instance.SetAppWindowsActive())
            {
                Messenger.Default.Send(new NavigateRequest());
            }
            Debug.WriteLine("close");
        }

        #endregion

        bool hasChange = false;

        public void AddImage(float[,] image)
        {
            Update(counter.Count(model.Predict(image)));
        }

        private Win32Point lastPoint = new Win32Point();
        public void SetPosition(Vector3 newpos)
        {
            Win32Point temp = inputMapper.getNextPosition(new Vector2(newpos.X, newpos.Y));
            //pred.SetPointPosition(1920, 1200);
            //  pred.SetPointPosition(temp);
            // MouseController.Instance.SetPosition(temp.X, temp.Y);
            lastPoint = temp;

            return;
        }


        public void AskUpdate()
        {
            var update = new UpdateListRequest()
            {
                Update = true
            };
            Messenger.Default.Send(update);
        }


    }
}
