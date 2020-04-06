using GestureBaseUI_Project.Camera;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Diagnostics;
using System.Windows.Input;
using GestureBaseUI_Project.Helper;
using System.IO;
using System.Threading;

namespace GestureBaseUI_Project.ViewModel
{
    public class DataCollectionViewModel : BaseViewModel
    {
        /// <summary>
        /// Bitmap to display main camera
        /// </summary>
        private readonly WriteableBitmap bitmapColorCamera;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSourceColor
        {
            get
            {
                return this.bitmapColorCamera;
            }
        }


        private readonly WriteableBitmap _finalImageSource;

        public ImageSource FinalImageSource
        {
            get
            {
                return this._finalImageSource;
            }
        }
        public HandGestures[] GestureTypeBoxSource { get; }

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

        private int _numberOfPhotos = 20;
        public string NumberOfPhotos
        {
            get
            {
                return _numberOfPhotos.ToString();
            }
            set
            {              
                SetValue(ref _numberOfPhotos, Int32.Parse(value));
            }
        }

        private HandGestures _selectedHandGesture;
        public HandGestures SelectedHandGesture
        {
            get { return _selectedHandGesture; }

            set
            {
                SetGestureImage((int)value);
                SetValue(ref _selectedHandGesture, value);

            }
        }


        private Color _recordFedbackFill = Colors.Green;

        public  SolidColorBrush RecordFedbackFill
        {
            get { return new SolidColorBrush(_recordFedbackFill); }

            set
            {
                SetValue(ref _recordFedbackFill, value.Color);
            }
        }

        public ICommand RecordClick
        {
            get
            {
                return new DelegateCommand(() =>
                {                   
                    cam.StartRecording(_numberOfPhotos, _selectedHandGesture);
                    Thread t = new Thread(new ThreadStart(cam.Take));
                    t.Start();
                    
                });
            }
        }

        private void SetGestureImage(int value)
        {
            Uri uri = new Uri(Images[value].FullName);
            BitmapImage bm = new BitmapImage(uri);
            GestureTypeImage = bm;
        }

        private FileInfo[] Images;

        DataCollectionCamera cam;
        public DataCollectionViewModel()
        {

            // box enum and select the first
            // https://stackoverflow.com/a/1167367
            GestureTypeBoxSource = (HandGestures[])Enum.GetValues(typeof(HandGestures));

            //get images
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"Images/HandGestureIcons"));
            Images = di.GetFiles("*.png");
         
            //set image
            SetGestureImage(0);


            cam = new DataCollectionCamera(ref bitmapColorCamera, this);

        }
    }
}
