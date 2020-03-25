using GestureBaseUI_Project.MouseControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace GestureBaseUI_Project
{
    public class ActionManager
    {
        int[] count = new int[10];

        private float[] prediction = new float[10];

        private Model model;

        private Prediction _mainPage;
        private HandPositionMapper inputMapper;

        Dictionary<string, Action> actions;

        private ActionCounter counter = new ActionCounter();
        public ActionManager(Prediction mainpage, HandPositionMapper mousecontroller)
        {
            _mainPage = mainpage;

            this.inputMapper = mousecontroller;

            model = new Model(System.IO.Path.Combine(Environment.CurrentDirectory, @"CNN\Model\gesture_model.pb"));

            InitActions();

        }

        private void InitActions()
        {
            this.actions = new Dictionary<string, Action>();
            this.actions.Add("0", Moving);
            this.actions.Add("1", Moving);
            this.actions.Add("2", Moving);
            this.actions.Add("3", Moving);
            this.actions.Add("4", Moving);
            this.actions.Add("5", Click);
            this.actions.Add("6", DoubleClick);
            this.actions.Add("7", FullHand);
            this.actions.Add("8", ScrollDown);
            this.actions.Add("9", ScrollUp);
        }


        private void Moving()
        {
            count[0]++;
            if (count[0] > 2) ResetAllCount();
            MouseController.Instance.SetPosition(lastPoint.X, lastPoint.Y);
            hasClick = false;
        }


        private void Click()
        {
            MouseController.Instance.Click();
            Debug.WriteLine("Clickkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk");
            return;
            if (count[5] == 0)
            {
                count[5]++;
            }
            else
              if (count[5] > 0 && !hasClick)
            {
                MouseController.Instance.Click();
                Debug.WriteLine("Clickkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk");
                count[5] = 0;
                hasClick = true;
            }
        }

        private void ScrollUp()
        {
            MouseController.Instance.ScrolUp(20);
            return;
            count[9]++;
            if (count[9] > 1)
            {
                MouseController.Instance.ScrolUp(20);
                this.actualState = 9;
            }
        }



        private void ScrollDown()
        {
            MouseController.Instance.ScrolDown(20);
            return;
        }
        bool hasChange = false;
        private void FullHand()
        {
            if (hasChange) return;
            Debug.WriteLine("Full nand.");
          var temp =  MouseController.Instance.NextWindows();
            hasChange = true;
            Debug.WriteLine("Full hand. " + temp );
        }

        private void DoubleClick()
        {
            Debug.WriteLine("Double click");
            // throw new NotImplementedException();
            hasChange = false;
        }




        int actualState = 0;
        bool hasClick = false;
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

            var prediction = model.Predict(imageLast);

            float[] prediction1d = new float[10];
            int i1 = 0;
            foreach (float f in prediction)
            {
                prediction1d[i1++] = f;
            }

            float max = prediction1d.Max();
            int m = Array.IndexOf(prediction1d, max);
           
            int next = counter.Count(m);
          //  Debug.WriteLine("m : " + m + " a : " + next);
            this.actions[next.ToString()]?.Invoke();

        }

        private Win32Point lastPoint = new Win32Point();
        public void SetPosition(Vector3 newpos)
        {
            Win32Point temp = inputMapper.getNextPosition(new Vector2(newpos.X, newpos.Y));
            //pred.SetPointPosition(1920, 1200);
            //  pred.SetPointPosition(temp);
            //MouseController.Instance.SetPosition(temp.X, temp.Y);
            lastPoint = temp;

            return;
        }


        private void ResetAllCount()
        {
            count = new int[10];
        }
        public void PrintCount()
        {
            foreach (int i in count)
            {
                Debug.WriteLine(i);
            }
        }
    }


    class ActionCounter
    {
        private int[] MIN_FOR_CHANGE = { 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3 };
        private int[] count = new int[10];
        private int last = 0;
        private int actual = 0;


        public int Count(int value)
        {
           
            //keep the same
            if (actual == value)
            {
                return actual;
            }
            else if (last == value)
            {
                count[value]++;
                ///means we change
                if (count[value]>= MIN_FOR_CHANGE[value])
                {
                    count[value] = 0;
                    actual = value;
                    return value;
                }
                else
                {
                    return actual;
                }
            }
            else
            {
                last = value;
                count[value]++;
                if (count[value] >= MIN_FOR_CHANGE[value])
                {
                    count[value] = 0;
                    actual = value;
                    return value;
                }
                else
                {
                    return actual;
                }
            }
        }
    }
}
