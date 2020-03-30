using System;
using System.Collections.Generic;
using System.Text;

namespace GestureBaseUI_Project
{
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
