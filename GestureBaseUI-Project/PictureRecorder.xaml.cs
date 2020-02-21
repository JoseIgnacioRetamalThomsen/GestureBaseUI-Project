using System;
using System.Collections.Generic;
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
    /// Interaction logic for PictureRecorder.xaml
    /// </summary>
    public partial class PictureRecorder : Page
    {
        private readonly Device kinect = null;

        private readonly WriteableBitmap bitmap = null;
        private string StatusText = null;
        private readonly int colorWidth = 0;//1080
        private readonly int colorHeight = 0;//1920
        private bool runnig = true;

        private readonly Microsoft.Azure.Kinect.Sensor.Transformation transform = null;

        //https://www.elitepvpers.com/forum/net-languages/118712-c-windows-api.html
        //http://pinvoke.net/default.aspx/user32/mouse_event.html?diff=y
        //https://stackoverflow.com/questions/8739523/directing-mouse-events-dllimportuser32-dll-click-double-click
        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        WriteableBitmap bitmapReco = null;
        Bitmap bm = null;

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
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            XDOWN = 0x00000080,
        }


        public PictureRecorder()
        {
            InitializeComponent();

            this.kinect = Device.Open();

            this.kinect.StartCameras(new DeviceConfiguration
            {
                ColorFormat = Microsoft.Azure.Kinect.Sensor.ImageFormat.ColorBGRA32,
                ColorResolution = ColorResolution.R720p,
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

            this.Loaded += PictureRecorder_Loaded;
        }

        private async void  PictureRecorder_Loaded(object sender, RoutedEventArgs e)
        {
            using (Tracker tracker = Tracker.Create(this.kinect.GetCalibration(), new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
            {
                while (true)
                {
                    uint[,] foto = new uint[300, 300];

                    bm = new Bitmap(300, 300);

                    using (Microsoft.Azure.Kinect.Sensor.Image transformedDepth = new Microsoft.Azure.Kinect.Sensor.Image(
                    Microsoft.Azure.Kinect.Sensor.ImageFormat.Depth16, colorWidth, colorHeight, colorWidth * sizeof(UInt16)))
                    using (Capture capture = await Task.Run(() => { return this.kinect.GetCapture(); }))
                    {

                        Text.Content = this.StatusText;

                        this.transform.DepthImageToColorCamera(capture, transformedDepth);

                        this.bitmap.Lock();
                        var color = capture.Color;

                        var region = new Int32Rect(0, 0, color.WidthPixels, (int)(color.HeightPixels / 1));



                        // Queue latest frame from the sensor.
                        tracker.EnqueueCapture(capture);


                        unsafe
                        {
                            using (Microsoft.Azure.Kinect.BodyTracking.Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                            using (var pin = color.Memory.Pin())
                            {


                                //get hand position
                                if (frame != null && frame.NumberOfBodies >= 1)
                                {

                                    var body = frame.GetBodySkeleton(0);
                                    var pos = body.GetJoint(JointId.HandRight).Position;


                                    //  var temp = kinect.GetCalibration().TransformTo2D(new Vector2(pos.X,pos.Y),-pos.Z ,CalibrationDeviceType.,CalibrationDeviceType.Color);
                                    var temp = kinect.GetCalibration().TransformTo2D(pos, CalibrationDeviceType.Depth, CalibrationDeviceType.Color);
                                    if (temp != null)
                                        //Debug.WriteLine(temp.Value.X);

                                        this.bitmap.WritePixels(region, (IntPtr)pin.Pointer, (int)color.Size, color.StrideBytes);

                                    uint* colorPixels = (uint*)this.bitmap.BackBuffer;
                                    ushort* depthPixels = (ushort*)transformedDepth.Memory.Pin().Pointer;

                                    int closest = 501;
                                    int x = 0;
                                    int y = 0;
                                    int cx = 0;
                                    int cy = 0;
                                    // Debug.WriteLine(pos.X);
                                    bool isFirst = true;
                                    int xref = 0, yref = 0;
                                    for (int i = 0; i < this.colorHeight * this.colorWidth; i++)
                                    {
                                        //if (i > 512000)
                                        //{
                                        //    colorPixels[i] = 0;
                                        //    continue;
                                        //}
                                        x++;
                                        if (i % 1280 == 0)
                                        {
                                            x = 0;
                                            y++;
                                        }







                                        //colorPixels[i] = 0;
                                        //// Compute the pixel's color.
                                        int color_data = 255 << 16; // R
                                        color_data |= 128 << 8;   // G
                                        color_data |= 255 << 0;   // B
                                        int ss = 150;

                                        if (x - ss == ((int)temp.Value.X) && (y - ss) < ((int)temp.Value.Y) && (y + ss) > ((int)temp.Value.Y))
                                        {

                                            colorPixels[i] = (uint)color_data;
                                        }

                                        if (x + ss == ((int)temp.Value.X) && (y - ss) < ((int)temp.Value.Y) && (y + ss) > ((int)temp.Value.Y))
                                        {

                                            colorPixels[i] = (uint)color_data;
                                        }
                                        if (y + ss == ((int)temp.Value.Y) && (x - ss) < ((int)temp.Value.X) && (x + ss) > ((int)temp.Value.X))
                                        {

                                            colorPixels[i] = (uint)color_data;
                                        }

                                        if (y - ss == ((int)temp.Value.Y) && (x - ss) < ((int)temp.Value.X) && (x + ss) > ((int)temp.Value.X))
                                        {

                                            colorPixels[i] = (uint)color_data;
                                        }
                                        if ((y - ss) < ((int)temp.Value.Y) && (y + ss) > ((int)temp.Value.Y) && (x - ss) < ((int)temp.Value.X) && (x + ss) > ((int)temp.Value.X))
                                        {
                                            if (isFirst)
                                            {
                                                xref = x;
                                                yref = y;
                                                isFirst = false;
                                            }

                                            if (depthPixels[i] < pos.Z + 100 && depthPixels[i] > pos.Z - 100 && depthPixels[i] != 0)
                                            {

                                                foto[y - yref, x - xref] = colorPixels[i];
                                                int deep = (((int)depthPixels[i] - ((int)pos.Z - 100)) * 250) / 200;
                                                //Debug.WriteLine(deep);
                                                bm.SetPixel(x - xref, y - yref, System.Drawing.Color.FromArgb(deep, deep, deep));
                                            }
                                            else
                                            {
                                                foto[y - yref, x - xref] = 0;
                                                // bm.SetPixel(x - xref, y - yref, System.Drawing.Color.FromArgb((int)(depthPixels[i] / 100), (int)(depthPixels[i] / 100), (int)(depthPixels[i]) / 100));
                                                bm.SetPixel(x - xref, y - yref, System.Drawing.Color.White);
                                            }
                                        }

                                        if (depthPixels[i] < pos.Z + 100 && depthPixels[i] > pos.Z - 100 && depthPixels[i] != 0)
                                        {




                                            if (depthPixels[i] < closest)
                                            {

                                                if (ox == 0)
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
                                        //colorPixels[i] = 0;
                                    }
                                    // Debug.WriteLine("x: " + cx);
                                    // Debug.WriteLine("y: " + cy);
                                    MoveEllipse(cx, cy);
                                    // Debug.WriteLine(closest);
                                    if (closest == 701)
                                    {
                                        //mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
                                        // mouse_event((uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
                                    }
                                }
                            }

                        }
                        //Bitmap btm = new Bitmap(this.colorWidth, this.colorHeight);

                        this.bitmap.AddDirtyRect(region);

                        this.bitmap.Unlock();
                        Bitmap bitmap1;
                        unsafe
                        {
                            fixed (uint* intPtr = &foto[0, 0])
                            {
                                bitmap1 = new Bitmap(300, 300, 300 * 4, System.Drawing.Imaging.PixelFormat.Format32bppRgb, new IntPtr(intPtr));
                            }
                            MyIm.Source = this.bitmap;
                            var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap1.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            MyIm1.Source = bitmapSource;
                            var bitmapSource1 = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            MyIm2.Source = bitmapSource1;
                            // CreateThumbnail("file.png", this.bitmap.Clone());
                            // System.Windows.Application.Current.Shutdown();
                        }
                    }
                }
            }
        }




        int ox = 0;
        int oy = 0;
        private void MoveEllipse(int cx, int cy)
        {
            //SetCursorPos(cx, cy);
            //Debug.WriteLine("dif" + (cx - ox));
            if (cx - ox < 0)//want to move right
            {

                MoveX(5);
            }
            else if (cx - ox > 0)
            {
                MoveX(-5);
            }

            if (cy - oy < 0)
            {
                MoveY(-1);
            }
            else if (cy - oy > 0)
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

        void CreateThumbnail(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image5));
                    encoder5.Save(stream5);
                }
            }
        }
    }
}
