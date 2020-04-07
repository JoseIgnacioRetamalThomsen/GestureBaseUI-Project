using GestureBaseUI_Project.ActionsManager;
using GestureBaseUI_Project.Camera;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GestureBaseUI_Project.ViewModel
{
    public class PracticeTutorialViewModel : BaseViewModel
    {
        /// <summary>
        /// Manage the action of the tutorial, read the inputs.
        /// </summary>
        private readonly PracticeActionManager am;
        /// <summary>
        /// Runs the camera for get imagess.
        /// </summary>
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
        /// Color of the circle.
        /// </summary>
        private Color _circleColor = Colors.Yellow;

        public SolidColorBrush CircleColor
        {
            get { return new SolidColorBrush(_circleColor); }

            set
            {
                SetValue(ref _circleColor, value.Color);
            }
        }

        /// <summary>
        /// Color of the circle.
        /// </summary>
        private Color _circleColor1 = Colors.Yellow;

        public SolidColorBrush CircleColor1
        {
            get { return new SolidColorBrush(_circleColor1); }

            set
            {
                SetValue(ref _circleColor1, value.Color);
            }
        }

        /// <summary>
        /// Gesture icon image
        /// </summary>
        private ImageSource _gestureTypeImage;
        public ImageSource GestureTypeImage
        {
            get
            {
                return this._gestureTypeImage;
            }
            set
            {
                SetValue(ref _gestureTypeImage, value);
            }
        }

        /// <summary>
        /// Gesture icon image
        /// </summary>
        private ImageSource _gestureTypeImage1;
        public ImageSource GestureTypeImage1
        {
            get
            {
                return this._gestureTypeImage1;
            }
            set
            {
                SetValue(ref _gestureTypeImage1, value);
            }
        }

        /// <summary>
        /// Path to all images.
        /// </summary>
        private List<string> _imagesPaths = new List<string>();

        /// <summary>
        /// Control runnig of thread that take images from queue.
        /// </summary>
        private bool isRunning = true;

        private int _sliderValue = 10;
        public int SliderValue
        {
            get { return _sliderValue; }
            set
            {
                SetValue(ref _sliderValue, value);
                am.SetMinForChange(value);
            }
        }

        public PracticeTutorialViewModel()
        {
            //Start queues.
            this.images = new BlockingCollection<float[,]>();
            this.bodyData = new BlockingCollection<BodyData>();

            //get images pahts
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"Images/HandGestureIcons"));
            FileInfo[] imagesPath = di.GetFiles("*.png");

            // set all images path
            foreach (var i in imagesPath)
            {
                _imagesPaths.Add(i.FullName);
            }

            SetReadyImage(false);
            SetGestureImage(-1);

            // create action manageer
            am = new PracticeActionManager(this);
            SetupActionManager();

            //start and create predictor
            Task.Factory.StartNew(() => { predictor = new MainCamera(this.images, bodyData); });

            // start thread that take images from queue.
            new Thread(() =>
            {
                while (isRunning)
                {
                    float[,] im = images.Take();

                    //if (!isImage) continue;
                    am.AddImage(im);
                }
            }).Start();


        }

        private void SetupActionManager()
        {
            am.SetMinForChange(App.userdata.Speed);
        }

        private const int TOTAL_IMAGES = 13;

        /// <summary>
        /// Map image number to the one using in the tutorial page.
        /// </summary>
        private int[] imagesMap = new int[TOTAL_IMAGES] { 5, 0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12 };

        public void SetReady(bool isReady)
        {
            if (isReady)
            {
                CircleColor = new SolidColorBrush(Colors.Green);
                SetReadyImage(true);
            }
            else
            {
                CircleColor = new SolidColorBrush(Colors.Yellow);
                SetReadyImage(false);
            }
        }

        public void SetActionDone(int gest)
        {
            if (gest >= 0)
            {
                SetGestureImage(gest);
                CircleColor1 = new SolidColorBrush(Colors.Green);
            }
            else
            {
                CircleColor1 = new SolidColorBrush(Colors.Yellow);
                SetGestureImage(gest);
            }

        }

        /// <summary>
        /// Set the gesture image in the view.
        /// </summary>
        /// <param name="value"></param>
        private void SetReadyImage(bool isReady)
        {
            if (isReady)
            {
                Uri uri = new Uri(_imagesPaths[5]);
                BitmapImage temp = new BitmapImage(uri);
                //need to freze image because is call from another thread.
                temp.Freeze();
                GestureTypeImage = temp;
            }
            else
            {
                Uri uri = new Uri(Path.Combine(Environment.CurrentDirectory, @"Images/Icons/one.png"));
                BitmapImage temp = new BitmapImage(uri);
                //need to freze image because is call from another thread.
                temp.Freeze();
                GestureTypeImage = temp;
            }
        }

        private void SetGestureImage(int imageNum)
        {
            if (imageNum >= 0)
            {
                Uri uri = new Uri(_imagesPaths[imagesMap[imageNum]]);
                BitmapImage temp = new BitmapImage(uri);
                //need to freze image because is call from another thread.
                temp.Freeze();
                GestureTypeImage1 = temp;
            }
            else
            {
                Uri uri = new Uri(Path.Combine(Environment.CurrentDirectory, @"Images/Icons/two.png"));
                BitmapImage temp = new BitmapImage(uri);
                //need to freze image because is call from another thread.
                temp.Freeze();
                GestureTypeImage1 = temp;
            }
        }

    }

}

