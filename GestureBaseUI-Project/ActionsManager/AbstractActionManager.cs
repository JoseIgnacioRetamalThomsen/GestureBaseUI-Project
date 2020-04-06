using System;
using System.Collections.Generic;
using System.Text;

namespace GestureBaseUI_Project.ActionsManager
{
    public abstract class AbstractActionManager : IActions
    {
        /// <summary>
        /// Map actions.
        /// </summary>
        public Dictionary<string, Action> actions;

        public AbstractActionManager()
        {
            InitActions();
        }


        public abstract void Close();
        public abstract void Cow();
        public abstract void Four();
        public abstract void Moving();
        public abstract void One();
        public abstract void Ready();
        public abstract void Surf();
        public abstract void Three();
        public abstract void Two();
        public abstract void WaveDown();
        public abstract void WaveLeft();
        public abstract void WaveRigth();
        public abstract void Waveup();

        public abstract void Update(int next);
        
        public void InitActions()
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

      
    }
}
