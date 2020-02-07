using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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




using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

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

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);



        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

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

                            int closest = 701;
                            int x = 0;
                            int y = 0;
                            int cx =0;
                            int cy = 0;
                            for(int i = 0; i < this.colorHeight * this.colorWidth; i++)
                            {
                                if (i > 1036255)
                                {
                                    colorPixels[i] = 0;
                                    continue;
                                }
                                x++;
                                if (i % 1920 == 0)
                                {
                                    x = 0;
                                    y++;
                                }
                                if (depthPixels[i]<700 && depthPixels[i] != 0)
                                {
                                   
                                    if(depthPixels[i] < closest)
                                    {
                                        if(ox == 0)
                                        {
                                            ox = cx;
                                            oy = cy;
                                        }
                                        closest = depthPixels[i];
                                        cx = x;
                                        cy = y;

                                    }
                                    continue;
                                }
                                colorPixels[i] = 0;
                               


                            }
                            Debug.WriteLine("x: "+ cx);
                            Debug.WriteLine("y: " + cy);
                            MoveEllipse(cx, cy);
                            Debug.WriteLine(closest);
                            if(closest == 701)
                            {
                                mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
                                mouse_event((uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
                            }
                        }

                    }
                    this.bitmap.AddDirtyRect(region);
                    this.bitmap.Unlock();
                    MyIm.Source = this.bitmap;
                }

            }
        }

        int ox =0;
        int oy= 0;
        private void MoveEllipse(int cx, int cy)
        {
            SetCursorPos(cx, cy);
            Debug.WriteLine("dif" + (cx - ox));
            if (cx-ox < 0 )//want to move right
            {

                MoveX(5);
            }else if(cx - ox > 0){
                MoveX(-5);
            }
            
            if(cy-oy < 0)
            {
                MoveY(-1);
            }else if (cy-oy > 0)
            {
                MoveY(1);
            }


        }
        private void MoveX(float amount)
        {

            Canvas.SetLeft(eli, Canvas.GetLeft(eli) + amount);
        }
        private void MoveY(float amount)
        {
            Canvas.SetTop(eli, Canvas.GetTop(eli) + amount);
        }
    }
}
