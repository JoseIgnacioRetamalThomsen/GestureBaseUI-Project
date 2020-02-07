using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Azure.Kinect.Sensor;

namespace GestureBaseUI_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Device kinect = null;

        private readonly WriteableBitmap bitmap = null;
        private string StatusText = null;
        private readonly int colorWidth = 0;//1080
        private readonly int colorHeight = 0;//1920
        private bool runnig = true;

        private readonly Microsoft.Azure.Kinect.Sensor.Transformation transform = null;

        public MainWindow()
        {
            InitializeComponent();

            this.kinect = Device.Open();

            this.kinect.StartCameras(new DeviceConfiguration
            {
                ColorFormat = Microsoft.Azure.Kinect.Sensor.ImageFormat.ColorBGRA32,
                ColorResolution = ColorResolution.R1080p,
                DepthMode = DepthMode.NFOV_2x2Binned,
                SynchronizedImagesOnly = true
            });

            this.transform = this.kinect.GetCalibration().CreateTransformation();

            this.colorWidth = this.kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
            this.colorHeight = this.kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;

            this.bitmap = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
            //change PixelFormat BGR£" for no depth
            //this.bitmap = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgra32, null);

            this.DataContext = this;


            this.InitializeComponent();
            this.Loaded += MainWindow_Loaded;


        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                using (Microsoft.Azure.Kinect.Sensor.Image transformedDepth = new Microsoft.Azure.Kinect.Sensor.Image(
                    Microsoft.Azure.Kinect.Sensor.ImageFormat.Depth16, colorWidth, colorHeight, colorWidth * sizeof(UInt16)))
                using (Capture capture = await Task.Run(() => { return this.kinect.GetCapture(); }))
                {
                    this.StatusText = "Received Capture:" + capture.Depth.DeviceTimestamp;
                    Text.Content = this.StatusText;

                    this.transform.DepthImageToColorCamera(capture, transformedDepth);

                    this.bitmap.Lock();
                    var color = capture.Color;
                    var region = new Int32Rect(0, 0, color.WidthPixels, color.HeightPixels);
                    unsafe
                    {
                        using (var pin = color.Memory.Pin())
                        {
                            this.bitmap.WritePixels(region, (IntPtr)pin.Pointer, (int)color.Size, color.StrideBytes);

                            uint* colorPixels = (uint*)this.bitmap.BackBuffer;
                            ushort* depthPixels = (ushort*)transformedDepth.Memory.Pin().Pointer;

                            int closest = 501;
                            for(int i = 0; i < this.colorHeight * this.colorWidth; i++)
                            {
                                if(depthPixels[i]<500 && depthPixels[i] != 0)
                                {
                                    if(depthPixels[i] < closest)
                                    {
                                        closest = i;
                                        
                                    }
                                    continue;
                                }
                                colorPixels[i] = 0;
                            }
                            Debug.WriteLine(this.colorHeight);
                            Debug.WriteLine(this.colorWidth);
                            Debug.WriteLine(closest);

                        }

                    }
                    this.bitmap.AddDirtyRect(region);
                    this.bitmap.Unlock();
                    MyIm.Source = this.bitmap;
                }

            }
        }
    }
}
