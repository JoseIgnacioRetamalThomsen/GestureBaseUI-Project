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
        /// Decide when is ready for input new commands
        /// </summary>
        bool ready = false;

        /// <summary>
        /// Use for check when the user start moving the cursor.
        /// </summary>
        private bool isFirstMove = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mousecontroller"></param>
        /// <param name="vm"></param>
        public ActionManager(HandPositionMapper mousecontroller, MainAppViewModel vm) : base()
        {

            _viewModel = vm;
            this.inputMapper = mousecontroller;

            model = new Model(System.IO.Path.Combine(Environment.CurrentDirectory, @"CNN\Model\gesture_model1.pb"));

            WindowController.Instance.SetAppWindow();

        }

        /// <summary>
        /// Change state to not ready
        /// </summary>
        public void SetNotReady()
        {
            ready = false;
        }

        /// <summary>
        ///  Set minimun amount of prediction for decitions,
        ///  this is the gesture sensibility.
        /// </summary>
        /// <param name="min"></param>
        public void SetMinForChange(int min)
        {
            counter.SetAllMin(min);
        }

        /// <summary>
        /// Update user actions on interface
        /// </summary>
        /// <param name="next"></param>
        public override void Update(int next)
        {
            // if ready a new action can be performed
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
                    Ready();

                    if (!isFirstMove)
                    {
                        isFirstMove = true;
                    }

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

        #region Actions Methdos

        /// <summary>
        /// Move the cursor
        /// </summary>
        public override void Moving()
        {
            if (isFirstMove)
            {
                inputMapper.StartMoving(lastHand);
                isFirstMove = false;
            }
            else
            {
                var temp = inputMapper.GetNextDis(lastHand);
                MouseController.Instance.SetPosition(temp.X, temp.Y);
            }
        }

        /// <summary>
        /// Geture one click
        /// </summary>
        public override void One()
        {
            MouseController.Instance.Click();
            ready = false;
        }

        /// <summary>
        /// Double click
        /// </summary>
        public override void Two()
        {
            MouseController.Instance.DoubleClick();
            ready = false;
        }

        /// <summary>
        /// User shortcut
        /// </summary>
        public override void Three()
        {
                 
            Debug.WriteLine("Three");
         
            ready = false;
        }

        /// <summary>
        /// Close actual windows.
        /// </summary>
        public override void Four()
        {
            WindowController.Instance.CloseActualWindow();
            Debug.WriteLine("four");
        }

        public override void Ready()
        {
            ready = true;
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

        /// <summary>
        /// Add a new image, the images is given to the model wich returns a prediciton.
        /// Then the prediciton is counted and the results is use for update the interface.
        /// </summary>
        /// <param name="image"></param>
        public void AddImage(float[,] image)
        {
            Update(counter.Count(model.Predict(image)));
        }

        private Win32Point lastPoint = new Win32Point();
        private Vector2 lastHand = Vector2.Zero;
        public void SetPosition(Vector3 newpos)
        {
            Win32Point temp = inputMapper.getNextPosition(new Vector2(newpos.X, newpos.Y));

            lastHand = new Vector2(newpos.X, newpos.Y);
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
