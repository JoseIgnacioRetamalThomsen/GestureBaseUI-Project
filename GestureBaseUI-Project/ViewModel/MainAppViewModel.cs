using GalaSoft.MvvmLight.Messaging;
using GestureBaseUI_Project.Camera;
using GestureBaseUI_Project.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GestureBaseUI_Project.ViewModel
{
    public class MainAppViewModel : BaseViewModel
    {
        
        public ObservableCollection<ProcessLink> Links { get; set; }

        /// <summary>
        /// Queue for output images, images are just represent as an array of floats.
        /// </summary>
        private BlockingCollection<float[,]> images;

        /// <summary>
        /// Queue for output hand position
        /// </summary>
        private BlockingCollection<BodyData> bodyData;



        private ActionManager actionManager;

       


        public MainAppViewModel()
        {
   
            this.images = new BlockingCollection<float[,]>() ;
            this.bodyData = new BlockingCollection<BodyData>() ;

            Links = new ObservableCollection<ProcessLink>();
            LoadLinks();

            MyRect screen = new MyRect(
                0,
                0,
                (float)System.Windows.SystemParameters.PrimaryScreenWidth,
                (float)System.Windows.SystemParameters.PrimaryScreenHeight);



            MyRect moveArea = new MyRect(250, -50, 450,100);

            HandPositionMapper mc = new HandPositionMapper(screen, moveArea);
            actionManager = new ActionManager(mc, this);

            Task.Factory.StartNew(() => { MainCamera predictor = new MainCamera(images,bodyData);});



            new Thread(() =>
            {
                while (true)
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
            /*
            Links.Clear();
            foreach(var i in WindowController.Instance.GetAllActiveWindows())
            {
                Links.Add(i);
            }*/
            
            Links = new ObservableCollection<ProcessLink>(WindowController.Instance.GetAllActiveWindows());
     
        }


    }
}
