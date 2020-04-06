using GestureBaseUI_Project.ViewModel;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GestureBaseUI_Project.Camera
{
    /// <summary>
    /// Collect data for training the neuron network
    /// Also shows the main camera and a preview of the recorder image.
    /// </summary>
    public class DataCollectionCamera
    {
        /// <summary>
        /// Azure Kinect sensor
        /// </summary>
        protected readonly Device kinect = null;

        /// <summary>
        /// Tranform data from diferent cameras
        /// </summary>
        private readonly Transformation transform = null;

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

        private readonly WriteableBitmap bitmapColorCamera = null;


        private BitmapSource bitmapFinal = null;

        public void StartRecording(int number, HandGestures hg)
        {
            status = hg;
            actual = number;
        }

        public  void Take()
        {

           
           System.Threading.Thread.Sleep(2000);
            saveNext = true;
            
            _main.RecordFedbackFill = new SolidColorBrush(Colors.Red);


            // Dispatcher.Invoke(new Action(() => { feedback.Fill = new SolidColorBrush(Colors.Red); }));


        }

        DataCollectionViewModel _main;
        public DataCollectionCamera(ref WriteableBitmap colorReference, DataCollectionViewModel main)
        {

            _main = main;

            // Open the default device
            this.kinect = Device.Open();

            // Configure camera 
            this.kinect.StartCameras(new DeviceConfiguration
            {
                ColorFormat = Microsoft.Azure.Kinect.Sensor.ImageFormat.ColorBGRA32,
                ColorResolution = ColorResolution.R720p
                ,
                DepthMode = DepthMode.NFOV_Unbinned,
                SynchronizedImagesOnly = true,
                CameraFPS = FPS.FPS30
            });

            // get transform
            this.transform = this.kinect.GetCalibration().CreateTransformation();

            // get color camera dimensions
            this.colorWidth = this.kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
            this.colorHeight = this.kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;


            this.bitmapColorCamera = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgra32, null);
            // this.bitmapFinal = new WriteableBitmap();

            colorReference = bitmapColorCamera;


            StartCamera();
        }


        WriteableBitmap bitmapReco = null;

        Vector3 handpositionGlobal;
        bool saveNext = false;

        int total = 100;
        int actual = 0;

        private async void StartCamera()
        {

            //body track 
            using (Tracker tracker = Tracker.Create(this.kinect.GetCalibration(), new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
            // sensor camera
            using (Microsoft.Azure.Kinect.Sensor.Image transformedDepth = new Microsoft.Azure.Kinect.Sensor.Image(
            Microsoft.Azure.Kinect.Sensor.ImageFormat.Depth16, colorWidth, colorHeight, colorWidth * sizeof(UInt16)))
            {
                //local variables


                // photo for prediction
                float[,] lastImage;
                // for show lastImage
                Bitmap bm = null;

                // Image camera 
                uint trackedBodyId = 0;
                JointConfidenceLevel conf = JointConfidenceLevel.Low;

                while (running)
                {
                    using (Capture capture = await Task.Run(() => { return this.kinect.GetCapture(); }))
                    {

                        bm = new Bitmap(30, 30);
                        // for final image
                        uint[,] foto = new uint[300, 300];
                        lastImage = new float[30, 30];
                        for (int i = 0; i < 30; i++)
                        {
                            for (int j = 0; j < 30; j++)
                            {
                                lastImage[i, j] = 1;
                                bm.SetPixel(i, j, System.Drawing.Color.Red);
                            }
                        }




                        // get depth 
                        this.transform.DepthImageToColorCamera(capture, transformedDepth);

                        //this.StatusText = "Received Capture: " + capture.Depth.DeviceTimestamp;

                        this.bitmapColorCamera.Lock();


                        // show normal camera
                        var color = capture.Color;

                        var region = new Int32Rect(0, 0, color.WidthPixels, color.HeightPixels);
                        var region1 = new Int32Rect(0, 0, 30, 30);

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

                                var body = frame.GetBodySkeleton(0);
                                var pos = body.GetJoint(JointId.HandRight).Position;
                                conf = body.GetJoint(JointId.HandRight).ConfidenceLevel;
                                var handposition = kinect.GetCalibration().TransformTo2D(pos, CalibrationDeviceType.Depth, CalibrationDeviceType.Color);
                                handpositionGlobal.X = handposition.Value.X;
                                handpositionGlobal.Y = handposition.Value.Y;
                                // z is the same
                                handpositionGlobal.Z = pos.Z;
                            }
                        }

                        unsafe
                        {

                            using (var pin = color.Memory.Pin())
                            {
                                // put color camera in bitmap
                                this.bitmapColorCamera.WritePixels(region, (IntPtr)pin.Pointer, (int)color.Size, color.StrideBytes);

                                //get pixel from color camera
                                uint* colorPixels = (uint*)this.bitmapColorCamera.BackBuffer;
                                ushort* depthPixels = (ushort*)transformedDepth.Memory.Pin().Pointer;


                                int closest = 501;
                                int x = 0;
                                int y = 0;
                                int cx = 0;
                                int cy = 0;
                                // Debug.WriteLine(pos.X);
                                bool isFirst = true;
                                int xref = 0, yref = 0;
                                int finalcount = 0;
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


                                            //scale actual depth
                                            float deep = (((int)depthPixels[i] - ((int)handpositionGlobal.Z - 150)) * 250) / 200;
                                            float deepF = deep / 255.0f;

                                            foto[y - yref, x - xref] = (uint)deep;

                                            uint xs = (uint)((x - xref) / 10);
                                            uint ys = (uint)((y - yref) / 10);

                                            if (deepF > lastImage[ys, xs] || lastImage[ys, xs] == 1)
                                            {
                                                lastImage[ys, xs] = deepF;
                                                var temp = System.Drawing.Color.FromArgb((int)deep, (int)deep, (int)deep);
                                                bm.SetPixel((int)ys, (int)xs, temp);



                                            }



                                        }
                                        else
                                        {
                                            foto[y - yref, x - xref] = 1;

                                            uint xs = (uint)((x - xref) / 10);
                                            uint ys = (uint)((y - yref) / 10);






                                            // bm.SetPixel(x - xref, y - yref, System.Drawing.Color.FromArgb((int)(depthPixels[i] / 100), (int)(depthPixels[i] / 100), (int)(depthPixels[i]) / 100));
                                        }
                                    }

                                }
                            }
                        }

                        this.bitmapColorCamera.AddDirtyRect(region);
                        this.bitmapColorCamera.Unlock();

                        //this.bitmapFinal = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                        if (saveNext && actual > 0 && conf != JointConfidenceLevel.Low)
                        {



                            WriteToFile(lastImage, status);
                            if (actual == 1)
                            {
                                saveNext = false;
                                _main.RecordFedbackFill = new SolidColorBrush(Colors.Green);

                            }
                            actual--;
                        }

                    }
                }
            }
        }


        HandGestures status;
        public void Close()
        {
            running = false;

            if (this.kinect != null)
            {
                this.kinect.Dispose();
            }
        }

        private void WriteToFile(float[,] foto, HandGestures gest)
        {

            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"C:\Users\Public\gesture\images15.txt", true))
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
         new System.IO.StreamWriter(@"C:\Users\Public\gesture\labels15.txt", true))
            {

                file.Write((int)gest);
                file.WriteLine();

            }


        }
    }
}
