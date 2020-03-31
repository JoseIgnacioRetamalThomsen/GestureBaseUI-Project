using GalaSoft.MvvmLight.Messaging;
using GestureBaseUI_Project.Camera;
using GestureBaseUI_Project.Models;
using GestureBaseUI_Project.View;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GestureBaseUI_Project.ViewModel
{
    public class MainAppViewModel : BaseViewModel
    {

        public ObservableCollection<ProcessLink> Links { get; set; }

        private MainCamera predictor;

        /// <summary>
        /// Queue for output images, images are just represent as an array of floats.
        /// </summary>
        private BlockingCollection<float[,]> images;

        /// <summary>
        /// Queue for output hand position
        /// </summary>
        private BlockingCollection<BodyData> bodyData;

        /// <summary>
        /// Manage the actions to perform after recognizing images
        /// </summary>
        private ActionManager actionManager;


        /// <summary>
        /// Control loop for taking images from queue
        /// </summary>
        private bool isRunnig = true;
        private MainApp mainApp;

        public MainAppViewModel(MainApp mainApp)
        {
            this.mainApp = mainApp;

            this.images = new BlockingCollection<float[,]>();
            this.bodyData = new BlockingCollection<BodyData>();

            Links = new ObservableCollection<ProcessLink>();
            LoadLinks();

            MyRect screen = new MyRect(
                0,
                0,
                (float)System.Windows.SystemParameters.PrimaryScreenWidth,
                (float)System.Windows.SystemParameters.PrimaryScreenHeight);



            MyRect moveArea = new MyRect(250, -50, 450, 100);

            HandPositionMapper mc = new HandPositionMapper(screen, moveArea);
            actionManager = new ActionManager(mc, this);

            Task.Factory.StartNew(() => { predictor = new MainCamera(images, bodyData); });



            new Thread(() =>
            {
                while (isRunnig)
                {
                    // Debug.WriteLine("Count " + images.Count);
                    float[,] im = images.Take();

                    actionManager.SetPosition(bodyData.Take().HandPosition);

                    actionManager.AddImage(im);

                }
            }).Start();


            Messenger.Default.Register<UpdateListRequest>(
                this,
                UpdateList);

        }


        private void UpdateList(UpdateListRequest obj)
        {
            LoadLinks();
        }

        public void LoadLinks()
        {

            Links = new ObservableCollection<ProcessLink>(WindowController.Instance.GetAllActiveWindows());
        }

        /// <summary>
        /// Stop all thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainApp_Unloaded(object sender, RoutedEventArgs e)
        {
            isRunnig = false;
            predictor.Close();
        }

    }
}
