using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;
using System.Numerics;


namespace GestureBaseUI_Project
{
  

    /// <summary>
    /// Interaction logic for Recorder.xaml
    /// </summary>
    public partial class Recorder : Page
    {

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Azure Kinect sensor
        /// </summary>
        private readonly Device kinect = null;

        /// <summary>
        /// Bitmap to display main camera
        /// </summary>
        private readonly WriteableBitmap bitmapMain = null;

        /// <summary>
        /// Bitmap to display crooped camera
        /// </summary>
        private readonly WriteableBitmap bitmapCrop = null;

        /// <summary>
        /// Bitmap to display final iamge
        /// </summary>
        private readonly WriteableBitmap bitmapFinal = null;

        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;

        /// <summary>
        /// The width in pixels of the color image from the Azure Kinect DK
        /// </summary>
        private readonly int colorWidth = 0;

        /// <summary>
        /// The height in pixels of the color image from the Azure Kinect DK
        /// </summary>
        private readonly int colorHeight = 0;

        /// <summary>
        /// Status of the application
        /// </summary>
        private bool running = true;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSourceMain
        {
            get
            {
                return this.bitmapMain;
            }
        }

        public Recorder()
        {
            // Open the default device
            this.kinect = Device.Open();

            // Configure camera modes
            this.kinect.StartCameras(new DeviceConfiguration
            {
                ColorFormat = Microsoft.Azure.Kinect.Sensor.ImageFormat.ColorBGRA32,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.NFOV_2x2Binned,
                SynchronizedImagesOnly = true
            });

            this.colorWidth = this.kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
            this.colorHeight = this.kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;

            this.bitmapMain = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgra32, null);

            this.DataContext = this;

            InitializeComponent();
            this.Loaded += Recorder_Loaded;
            this.Unloaded += Recorder_Unloaded;



        }

        private void Recorder_Unloaded(object sender, RoutedEventArgs e)
        {
            running = false;

            if (this.kinect != null)
            {
                this.kinect.Dispose();
            }
        }

        private async void Recorder_Loaded(object sender, RoutedEventArgs e)
        {
            //body track 
            using (Tracker tracker = Tracker.Create(this.kinect.GetCalibration(), new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
            {

                while (running)
                {
                    // Image camera 
                    using (Capture capture = await Task.Run(() => { return this.kinect.GetCapture(); }))
                    {
                        //this.StatusText = "Received Capture: " + capture.Depth.DeviceTimestamp;

                        this.bitmapMain.Lock();

                        var color = capture.Color;
                        var region = new Int32Rect(0, 0, color.WidthPixels, color.HeightPixels);

                        unsafe
                        {
                            using (var pin = color.Memory.Pin())
                            {
                                this.bitmapMain.WritePixels(region, (IntPtr)pin.Pointer, (int)color.Size, color.StrideBytes);
                            }
                        }

                        this.bitmapMain.AddDirtyRect(region);
                        this.bitmapMain.Unlock();
                    }
                }
            }
        }



    }
}
