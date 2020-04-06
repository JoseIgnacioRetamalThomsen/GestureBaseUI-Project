using System;
using System.Collections.Generic;
using System.Text;

namespace GestureBaseUI_Project
{
    /// <summary>
    /// Counts the predicted gestures, after a predcition reach a minimun amout of
    /// positives it will be returned and mark as actual.
    /// </summary>
    public class ActionCounter
    {
        /// <summary>
        /// Amout of good predictions for accept the reading
        /// </summary>
        private int[] MIN_FOR_CHANGE = { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

        /// <summary>
        /// Count of each prediction
        /// </summary>
        private int[] count = new int[13];

        /// <summary>
        /// The last input
        /// </summary>
        private int last = 0;

        /// <summary>
        /// The prediction that is accepted, after one prediction it is it would
        /// be returning until there is a new one
        /// </summary>
        private int actual = 0;

        /// <summary>
        /// Create object with 10 matcher for accept the predictiion
        /// </summary>
        public ActionCounter()
        {
            SetAllMin(10);
        }

        /// <summary>
        /// Create object with the start prediction as actuasl
        /// </summary>
        /// <param name="start"></param>
        public ActionCounter(int start)
        {
            actual = start;

        }

        /// <summary>
        /// Change amount of positive read for all the gestures
        /// </summary>
        /// <param name="value"></param>
        public void SetAllMin(int value)
        {
            for (int i = 0; i < MIN_FOR_CHANGE.Length; i++)
            {
                MIN_FOR_CHANGE[i] = value;
            }
        }

        /// <summary>
        /// Set actual action
        /// </summary>
        /// <param name="num"></param>
        public void SetAction(int num)
        {
            count[actual] = 0;
            actual = num;
            last = num;
        }

        /// <summary>
        /// Count amount of prediction.
        /// If the prediciton reach the minimun the actual would
        /// be cahnged to that.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
