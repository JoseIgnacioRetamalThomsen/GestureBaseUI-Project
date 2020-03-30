using GalaSoft.MvvmLight.Messaging;
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
    public class ActionManager
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
        Dictionary<string, Action> actions;

        /// <summary>
        /// Counts predictions for decide when to change
        /// </summary>
        private readonly ActionCounter counter = new ActionCounter();

        MainAppViewModel _viewModel;
        public ActionManager(HandPositionMapper mousecontroller, MainAppViewModel vm)
        {

            _viewModel = vm;
            this.inputMapper = mousecontroller;

            model = new Model(System.IO.Path.Combine(Environment.CurrentDirectory, @"CNN\Model\gesture_model1.pb"));

            InitActions();
            
        }

        int actual = 5;
        bool ready = false;
        bool moving = false;

        public void Update(int next)
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



        private void Moving()
        {
            Debug.WriteLine("moving");
            MouseController.Instance.SetPosition(lastPoint.X, lastPoint.Y);
        }

        private void One()
        {
            Debug.WriteLine("click");
            MouseController.Instance.Click();
            ready = false;
        }

        private void Two()
        {
            Debug.WriteLine("Two");
            ready = false;

        }
        private void Three()
        {

            //C:\Users\pepe\eclipse\java-2019-09\eclipse\eclipse.exe
            Debug.WriteLine("Three");
            Process.Start("C:/Program Files (x86)/Google/Chrome/Application/chrome.exe");

            AskUpdate();
            ready = false;
        }
        private void Four()
        {
            Debug.WriteLine("four");
        }

        private void Ready()
        {
            Debug.WriteLine("Ready");
        }

        private void WaveDown()
        {
            MouseController.Instance.ScrolDown(40);
            Debug.WriteLine("WaveDown");

        }
        private void Waveup()
        {
            MouseController.Instance.ScrolUp(40);
            Debug.WriteLine("Wave up");

        }
        private void WaveLeft()
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
            int nextPosition = (position + 1) % _viewModel.Links.Count;
            IntPtr nextP = _viewModel.Links[nextPosition].Windows;
            WindowController.Instance.SetActiveWindows(nextP);
            Debug.WriteLine("waveLeft");
        }
        private void WaveRigth()
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
        private void Surf()
        {
            Debug.WriteLine("surf");
        }
        private void Cow()
        {
            Debug.WriteLine("cow");
        }
        private void Close()
        {
            Debug.WriteLine("Close");
        }


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


    public class ActionCounter
    {
        private int[] MIN_FOR_CHANGE = { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
        private int[] count = new int[13];
        private int last = 0;
        private int actual = 0;


        public ActionCounter()
        {
            setAllMin(10);
        }
        public ActionCounter(int start)
        {
            actual = start;

        }

        private void setAllMin(int value)
        {
            for (int i = 0; i < MIN_FOR_CHANGE.Length; i++)
            {
                MIN_FOR_CHANGE[i] = value;
            }
        }
        public void SetAction(int num)
        {
            count[actual] = 0;
            actual = num;
            last = num;
        }

        public int Count(int value)
        {
            // no change
            if (value == actual)
            {

            }
            else
            if (value != last)
            {
                count[last] = 0;
                count[value]++;
                last = value;

            }
            else
            {
                count[value]++;
                if (count[value] >= MIN_FOR_CHANGE[value])
                {
                    actual = value;
                }
            }
            return actual;


        }
    }
}
