using GestureBaseUI_Project.Camera;
using GestureBaseUI_Project.MouseControl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GestureBaseUI_Project.ViewModel
{
    public class MainAppViewModel : BaseViewModel
    {
        /// <summary>
        /// Queue for output images, images are just represent as an array of floats.
        /// </summary>
        private BlockingCollection<float[,]> images;

        /// <summary>
        /// Queue for output hand position
        /// </summary>
        private BlockingCollection<BodyData> bodyData;



        private ActionManager actionManager;

        private HandPositionMapper inputMapper;


        public MainAppViewModel()
        {
            this.images = new BlockingCollection<float[,]>() ;
            this.bodyData = new BlockingCollection<BodyData>() ;


            MyRect screen = new MyRect(
                0,
                0,
                (float)System.Windows.SystemParameters.PrimaryScreenWidth,
                (float)System.Windows.SystemParameters.PrimaryScreenHeight);



            MyRect moveArea = new MyRect(100, 250, -300, 50);

            HandPositionMapper mc = new HandPositionMapper(screen, moveArea);
            actionManager = new ActionManager(mc);

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


        }
    }
}
