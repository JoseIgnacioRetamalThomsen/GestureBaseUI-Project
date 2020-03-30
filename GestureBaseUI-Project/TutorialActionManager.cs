using GestureBaseUI_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace GestureBaseUI_Project
{
    public class TutorialActionManager
    {
        /// <summary>
        /// Model for predicition
        /// </summary>
        private readonly Model model;

        /// <summary>
        /// Map actions
        /// </summary>
        Dictionary<string, Action> actions;

        /// <summary>
        /// Counts predictions for decide when to change
        /// </summary>
        private readonly ActionCounter counter = new ActionCounter();

        private TutorialViewModel _viewModel;

        public TutorialActionManager(TutorialViewModel viewModel)
        {
            _viewModel = viewModel;

            model = new Model(System.IO.Path.Combine(Environment.CurrentDirectory, @"CNN\Model\gesture_model1.pb"));

        }

        public void AddImage(float[,] image)
        {
            Update(counter.Count(model.Predict(image)));
        }


        private bool isReady = true;
        public void MakeReady()
        {
            this.isReady = true;
            Debug.WriteLine("isred");
        }

        private void Update(int v)
        {
            Debug.WriteLine(isReady);
            if (this.isReady)
            {
                Debug.WriteLine(v);
                if (_viewModel.PageNumber == 0)
                {
                    if (v == 5)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(12);


                    }

                }
                else if (_viewModel.PageNumber == 1)
                {
                    if (v == 0)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(5);

                    }
                }
                else if (_viewModel.PageNumber == 2)
                {
                    if (v == 1)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }
                else if (_viewModel.PageNumber == 3)
                {
                    if (v == 2)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }
                else if (_viewModel.PageNumber == 4)
                {
                    if (v == 3)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }
                else if (_viewModel.PageNumber == 5)
                {
                    if (v == 4)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }
                else if (_viewModel.PageNumber == 6)
                {
                    if (v == 6)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }

                else if (_viewModel.PageNumber == 7)
                {
                    if (v == 7)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }
                else if (_viewModel.PageNumber == 8)
                {
                    if (v == 8)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }
                else if (_viewModel.PageNumber == 9)
                {
                    if (v == 9)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }
                else if (_viewModel.PageNumber == 10)
                {
                    if (v == 10)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }
                else if (_viewModel.PageNumber == 11)
                {
                    if (v == 11)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }
                else if (_viewModel.PageNumber == 12)
                {
                    if (v == 12)
                    {
                        isReady = false;

                        _viewModel.GestureDone();
                        counter.SetAction(0);

                    }
                }
            }
        }
    }
}
