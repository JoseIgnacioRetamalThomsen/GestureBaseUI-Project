using System;
using System.Collections.Generic;
using System.Text;

namespace GestureBaseUI_Project.ActionsManager
{
    public interface IActions
    {
        /// <summary>
        /// Define the gesture for moving the mouse
        /// </summary>
        public abstract void Moving();

        /// <summary>
        /// Define gesture one
        /// </summary>
        public abstract void One();

        /// <summary>
        /// Gestrue two
        /// </summary>
        public abstract void Two();

        /// <summary>
        /// Gesture 3
        /// </summary>
        public abstract void Three();

        /// <summary>
        /// Gesture 4
        /// </summary>
        public abstract void Four();


        /// <summary>
        /// Gesture five
        /// </summary>
        public abstract void Ready();


        /// <summary>
        /// Wave downd gesture
        /// </summary>
        public abstract void WaveDown();

        /// <summary>
        /// Wave ip Gesture.
        /// </summary>
        public abstract void Waveup();

        /// <summary>
        /// Wave left gesture.
        /// </summary>
        public abstract void WaveLeft();

        /// <summary>
        /// Wave rieght gesture
        /// </summary>
        public abstract void WaveRigth();

        /// <summary>
        /// Gesture
        /// </summary>
        public abstract void Surf();

       /// <summary>
       /// Gesture
       /// </summary>
        public abstract void Cow();

        /// <summary>
        /// Close fist gesture
        /// </summary>
        public abstract void Close();

    }
}
