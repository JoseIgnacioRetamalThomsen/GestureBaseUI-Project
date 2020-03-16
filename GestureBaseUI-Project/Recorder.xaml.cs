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
using System.Globalization;
using TensorFlow;


namespace GestureBaseUI_Project
{


    /// <summary>
    /// Interaction logic for Recorder.xaml
    /// </summary>
    public partial class Recorder : Page
    {
        const int PICTURE_NUMBER = 200;

        private readonly Microsoft.Azure.Kinect.Sensor.Transformation transform = null;
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
        private BitmapSource bitmapCrop = null;

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
        public ImageSource ImageSourceCrop
        {
            get
            {
                return this.bitmapCrop;
            }
        }
        public ImageSource ImageSourceFinal
        {
            get
            {
                return this.bitmapFinal;
            }
        }

        TFGraph graph;
        TFSession session;
        TFSession.Runner runner;
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

            this.transform = this.kinect.GetCalibration().CreateTransformation();

            this.colorWidth = this.kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
            this.colorHeight = this.kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;

            this.bitmapMain = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgra32, null);

            // this.bitmapCrop =  new Bitmap(300, 300);

            this.DataContext = this;

            InitializeComponent();
            this.Loaded += Recorder_Loaded;
            this.Unloaded += Recorder_Unloaded;


            box.ItemsSource = Enum.GetValues(typeof(HandGestures));


            graph = new TFGraph();
            graph.Import(File.ReadAllBytes(@"C:\Users\Public\TestFolder\my_model.pb"));
            session = new TFSession(graph);
            runner = session.GetRunner();


        }

        private void Recorder_Unloaded(object sender, RoutedEventArgs e)
        {
            running = false;

            if (this.kinect != null)
            {
                this.kinect.Dispose();
            }
        }


        WriteableBitmap bitmapReco = null;
        Bitmap bm = null;
        Vector3 handpositionGlobal;
        bool saveNext = false;

        int total = 100;
        int actual = PICTURE_NUMBER;
        private async void Recorder_Loaded(object sender, RoutedEventArgs e)
        {

            //body track 
            using (Tracker tracker = Tracker.Create(this.kinect.GetCalibration(), new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))


            // sensor camera
            using (Microsoft.Azure.Kinect.Sensor.Image transformedDepth = new Microsoft.Azure.Kinect.Sensor.Image(
            Microsoft.Azure.Kinect.Sensor.ImageFormat.Depth16, colorWidth, colorHeight, colorWidth * sizeof(UInt16)))
            {
                // Image camera 
                uint trackedBodyId = 0;
                JointConfidenceLevel conf = JointConfidenceLevel.Low;

                while (running)
                {
                    using (Capture capture = await Task.Run(() => { return this.kinect.GetCapture(); }))
                    {


                        // for final image
                        uint[,] foto = new uint[300, 300];
                        float[,] fotoS = new float[30, 30];
                        for (int i = 0; i < 30; i++)
                        {
                            for (int j = 0; j < 30; j++)
                            {
                                fotoS[i, j] = 1;
                            }
                        }


                        // for crop image
                        bm = new Bitmap(300, 300);

                        // get depth 
                        this.transform.DepthImageToColorCamera(capture, transformedDepth);

                        //this.StatusText = "Received Capture: " + capture.Depth.DeviceTimestamp;

                        this.bitmapMain.Lock();

                        // show normal camera
                        var color = capture.Color;

                        var region = new Int32Rect(0, 0, color.WidthPixels, color.HeightPixels);

                        // Queue latest frame from the sensor.
                        tracker.EnqueueCapture(capture);


                        //get position of the hand
                        using (Microsoft.Azure.Kinect.BodyTracking.Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                        {
                            //get hand position
                            if (frame != null && frame.NumberOfBodies >= 1)
                            {
                                if (trackedBodyId <= 0)
                                {
                                    trackedBodyId = frame.GetBody(0).Id;
                                }
                                var numOfSkeletos = frame.NumberOfBodies;
                                // Debug.WriteLine("num of bodies: " + numOfSkeletos);

                                var body = frame.GetBodySkeleton(0);
                                var pos = body.GetJoint(JointId.HandRight).Position;
                                conf = body.GetJoint(JointId.HandRight).ConfidenceLevel;
                                Debug.WriteLine(conf);
                                /*
                                if (conf == JointConfidenceLevel.Low)
                                {
                                    this.bitmapMain.AddDirtyRect(region);
                                    this.bitmapMain.Unlock();

                                    this.Crop.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                    continue;
                                }*/

                                //  Debug.WriteLine("confidende level   : " + conf);
                                // transfor position to capture camera
                                var handposition = kinect.GetCalibration().TransformTo2D(pos, CalibrationDeviceType.Depth, CalibrationDeviceType.Color);
                                handpositionGlobal.X = handposition.Value.X;
                                handpositionGlobal.Y = handposition.Value.Y;
                                // z is the same
                                handpositionGlobal.Z = pos.Z;
                            }
                        }


                        // Debug.WriteLine(handpositionGlobal.X);

                        unsafe
                        {


                            using (var pin = color.Memory.Pin())
                            {
                                // put color camera in bitmap
                                this.bitmapMain.WritePixels(region, (IntPtr)pin.Pointer, (int)color.Size, color.StrideBytes);

                                //get pixel from color camera
                                uint* colorPixels = (uint*)this.bitmapMain.BackBuffer;
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


                                    //draw square around hand
                                    int color_data = 255 << 16; // R
                                    color_data |= 128 << 8;   // G
                                    color_data |= 255 << 0;   // B
                                    int ss = 150;

                                    if (x - ss == ((int)handpositionGlobal.X) && (y - ss) < ((int)handpositionGlobal.Y) && (y + ss) > ((int)handpositionGlobal.Y))
                                    {

                                        colorPixels[i] = (uint)color_data;
                                    }

                                    if (x + ss == ((int)handpositionGlobal.X) && (y - ss) < ((int)handpositionGlobal.Y) && (y + ss) > ((int)handpositionGlobal.Y))
                                    {

                                        colorPixels[i] = (uint)color_data;
                                    }
                                    if (y + ss == ((int)handpositionGlobal.Y) && (x - ss) < ((int)handpositionGlobal.X) && (x + ss) > ((int)handpositionGlobal.X))
                                    {

                                        colorPixels[i] = (uint)color_data;
                                    }

                                    if (y - ss == ((int)handpositionGlobal.Y) && (x - ss) < ((int)handpositionGlobal.X) && (x + ss) > ((int)handpositionGlobal.X))
                                    {

                                        colorPixels[i] = (uint)color_data;
                                    }

                                    //insede the square
                                    if ((y - ss) < ((int)handpositionGlobal.Y) && (y + ss) > ((int)handpositionGlobal.Y) && (x - ss) < ((int)handpositionGlobal.X) && (x + ss) > ((int)handpositionGlobal.X))
                                    {
                                        if (isFirst)
                                        {
                                            xref = x;
                                            yref = y;
                                            isFirst = false;
                                        }

                                        //select pixels insede the selected deep
                                        if (depthPixels[i] < handpositionGlobal.Z + 50 && depthPixels[i] > handpositionGlobal.Z - 150 && depthPixels[i] != 0)
                                        {


                                            int deep = (((int)depthPixels[i] - ((int)handpositionGlobal.Z - 150)) * 250) / 200;
                                            float deepF = deep / 255.0f;

                                            foto[y - yref, x - xref] = (uint)deep;

                                            uint xs = (uint)((x - xref) / 10);
                                            uint ys = (uint)((y - yref) / 10);

                                            if (deepF > fotoS[ys, xs] || fotoS[ys, xs]==1)
                                            {
                                                fotoS[ys, xs] = deepF;
                                            }

                                            //Debug.WriteLine(deep);
                                            var temp = System.Drawing.Color.FromArgb(deep, deep, deep);
                                            bm.SetPixel(x - xref, y - yref, temp);
                                            //Debug.WriteLine(temp + "  " + (x - xref) + "  " + (y-yref));
                                        }
                                        else
                                        {
                                            foto[y - yref, x - xref] = 1;

                                            uint xs = (uint)((x - xref) / 10);
                                            uint ys = (uint)((y - yref) / 10);


                                            // fotoS[ys, xs] = 0;



                                            // bm.SetPixel(x - xref, y - yref, System.Drawing.Color.FromArgb((int)(depthPixels[i] / 100), (int)(depthPixels[i] / 100), (int)(depthPixels[i]) / 100));
                                            bm.SetPixel(x - xref, y - yref, System.Drawing.Color.White);
                                        }
                                    }

                                }
                            }
                        }

                        this.bitmapMain.AddDirtyRect(region);
                        this.bitmapMain.Unlock();

                        this.Crop.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                        if (saveNext && actual > 0 && conf != JointConfidenceLevel.Low)
                        {


                            HandGestures status;
                            Enum.TryParse<HandGestures>(box.SelectedValue.ToString(), out status);
                            WriteToFile(fotoS, status);
                            if (actual == 1)
                            {
                                saveNext = false;
                                feedback.Fill = new SolidColorBrush(Colors.Blue);
                            }
                            actual--;
                        }
                        //Predict(fotoS);
                    }
                }
            }
        }


        private void Predict(uint[,] foto)
        {/*
            var input = new TFTensor(TFDataType.Float, new long[] { 30, 30 }, 30*30 * sizeof(float));
            var runner = session.GetRunner();
            runner.AddInput(graph["input"][0], new TFTensor(foto));*/
         // runner.Fetch(graph["output"][0]);

            //var output = runner.Run();
            //Debug.WriteLine(output);
        }

        private void WriteToFile(float[,] foto, HandGestures gest)
        {

            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"C:\Users\Public\gesture\image.txt", true))
            {
                for (int i = 0; i < 30; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        file.Write(foto[i, j] + " ");
                    }
                }
                file.WriteLine();


            }

            using (System.IO.StreamWriter file =
         new System.IO.StreamWriter(@"C:\Users\Public\gesture\labels.txt", true))
            {

                file.Write((int)gest);
                file.WriteLine();

            }


        }



        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a render target to which we'll render our composite image
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)CompositeImage.ActualWidth, (int)CompositeImage.ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(CompositeImage);
                dc.DrawRectangle(brush, null, new System.Windows.Rect(new System.Windows.Point(), new System.Windows.Size(CompositeImage.ActualWidth, CompositeImage.ActualHeight)));
            }

            renderBitmap.Render(dv);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            string path = System.IO.Path.Combine(myPhotos, "KinectScreenshot-" + time + ".png");

            // Write the new file to disk
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    encoder.Save(fs);
                }

                //this.StatusText = string.Format(Properties.Resources.SavedScreenshotStatusTextFormat, path);
            }
            catch (IOException)
            {
                //this.StatusText = string.Format(Properties.Resources.FailedScreenshotStatusTextFormat, path);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(Take));
            t.Start();



        }

        async void Take()
        {
            System.Threading.Thread.Sleep(2000);
            saveNext = true;
            actual = PICTURE_NUMBER;
            Debug.Write("Happens");

            Dispatcher.Invoke(new Action(() => { feedback.Fill = new SolidColorBrush(Colors.Red); }));


        }
    }
    enum HandGestures
    {
        StraitVertial, StraitHorizontal, FistVertical, FistHorizotan, TombIndexVertical, TombIndexHorizontal, SlapDown, SlapUp, SlapFront, FullHand
    }
}
