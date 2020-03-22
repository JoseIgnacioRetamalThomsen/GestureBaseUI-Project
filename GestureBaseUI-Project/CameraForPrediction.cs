using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Threading.Tasks;
using System.Numerics;
using System.Diagnostics;
using System.Collections.Concurrent;
using GestureBaseUI_Project.model;

namespace GestureBaseUI_Project
{
    public class CameraForPrediction
    {

        BlockingCollection<float[,]> images; //= new LinkedList<uint[,]>();



        private readonly Transformation transform = null;



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

        private StateManager manager;

        public CameraForPrediction(BlockingCollection<float[,]> im, StateManager manager )
        {
            this.manager = manager;

            this.images = im;
           // Debug.WriteLine("Created");
            // Open the default device
            this.kinect = Device.Open();
            

            // Configure camera modes
            this.kinect.StartCameras(new DeviceConfiguration
            {
                ColorFormat = Microsoft.Azure.Kinect.Sensor.ImageFormat.ColorBGRA32,
                ColorResolution = ColorResolution.R720p
                ,
                DepthMode = DepthMode.NFOV_Unbinned,
                SynchronizedImagesOnly = true,
                CameraFPS = FPS.FPS30
            });


            /*
             *      
               this.kinect.StartCameras(new DeviceConfiguration
            {
                ColorFormat = Microsoft.Azure.Kinect.Sensor.ImageFormat.ColorBGRA32,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.NFOV_2x2Binned,
                SynchronizedImagesOnly = true,
                CameraFPS = FPS.FPS30
            });
             * */
            this.transform = this.kinect.GetCalibration().CreateTransformation();

            this.colorWidth = this.kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
            this.colorHeight = this.kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;

            this.bitmapMain = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgra32, null);

            this.Recorder_Loaded();

        }


        public void Close()
        {
            running = false;

            if (this.kinect != null)
            {
                this.kinect.Dispose();
            }
        }

        Vector3 handpositionGlobal;
        int ox = 0;
        int oy = 0;

        private async void Recorder_Loaded()
        {
           // Debug.WriteLine("in recorder");
            //body track 
            using (Tracker tracker = Tracker.Create(this.kinect.GetCalibration(), new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
            // sensor camera
            using (Microsoft.Azure.Kinect.Sensor.Image transformedDepth = new Microsoft.Azure.Kinect.Sensor.Image(Microsoft.Azure.Kinect.Sensor.ImageFormat.Depth16, colorWidth, colorHeight, colorWidth * sizeof(UInt16)))
            {
                uint trackedBodyId = 0;
                JointConfidenceLevel conf = JointConfidenceLevel.Low;

                while (running)
                {
                    //get capture
                    using (Capture capture = await Task.Run(() => { return this.kinect.GetCapture(); }))
                    {
                        //create output array that represent the image
                        float[,] fotoS = new float[30, 30];
                        for (int i = 0; i < 30; i++)
                        {
                            for (int j = 0; j < 30; j++)
                            {
                                fotoS[i, j] = 255;
                            }
                        }

                       
                        this.transform.DepthImageToColorCamera(capture, transformedDepth);

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
                                
                                var fpos = body.GetJoint(JointId.HandTipRight).Position;
                                var rpost = body.GetJoint(JointId.WristRight).Position;
                                conf = body.GetJoint(JointId.HandRight).ConfidenceLevel;
                                Quaternion orin = body.GetJoint(JointId.HandTipRight).Quaternion;
                                //Debug.WriteLine(orin);
                                //   Debug.WriteLine(pos);
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
                                var oldHand = handpositionGlobal;
                                //set hand for prediction square
                                var handposition = kinect.GetCalibration().TransformTo2D(pos, CalibrationDeviceType.Depth, CalibrationDeviceType.Color);
                                handpositionGlobal.X = handposition.Value.X;
                                handpositionGlobal.Y = handposition.Value.Y;
                                // z is the same
                                handpositionGlobal.Z = pos.Z;
                                //if(body.GetJoint(JointId.HandTipRight).ConfidenceLevel == JointConfidenceLevel.Low)
                                //{
                                //    manager.SetPosition(oldHand);
                                //}
                                //else {
                                //Debug.WriteLine(handpositionGlobal);
                                

                                // set position for mouse move

                                if(conf == JointConfidenceLevel.Medium || conf == JointConfidenceLevel.High)
                                    manager.SetPosition(pos);
                              //  }
                               
                            }
                        }


                        // show normal camera
                        var color = capture.Color;
                        unsafe
                        {
                            
                            
                            using (var pin = color.Memory.Pin())
                            {
                                // we only using deep now
                                ushort* depthPixels = (ushort*)transformedDepth.Memory.Pin().Pointer;

                                int closest = 501;
                                int x = 0;
                                int y = 0;
                                int cx = 0;
                                int cy = 0;
                                // Debug.WriteLine(pos.X);
                                bool isFirst = true;
                                int xref = 0, yref = 0;
                                int squareSize = 150;
                                
                                for (int i = 0; i < this.colorHeight * this.colorWidth; i++)
                                {
                                    x++;
                                    if (i % 1280 == 0)
                                    {
                                        x = 0;
                                        y++;
                                    }
                                    //insede the square
                                    if ((y - squareSize) < ((int)handpositionGlobal.Y) && (y + squareSize) > ((int)handpositionGlobal.Y) && (x - squareSize) < ((int)handpositionGlobal.X) && (x + squareSize) > ((int)handpositionGlobal.X))
                                    {
                                        if (isFirst)
                                        {
                                            xref = x;
                                            yref = y;
                                            isFirst = false;
                                        }


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


                                        //select pixels insede the selected deep
                                        if (depthPixels[i] < handpositionGlobal.Z + 50 && depthPixels[i] > handpositionGlobal.Z - 150 && depthPixels[i] != 0)
                                        {

                                            float deep = (((int)depthPixels[i] - ((int)handpositionGlobal.Z - 150)) * 250) / 200;
                                            //  Debug.WriteLine("deep" + (uint)deep);
                                            uint xs = (uint)((x - xref) / 10);
                                            uint ys = (uint)((y - yref) / 10);

                                            if ((uint)deep > fotoS[ys, xs] || fotoS[ys,xs] == 255)
                                            {
                                                fotoS[ys, xs] = deep;

                                            }
                                        }
                                    }

                                }

                               // manager.SetPosition(new Vector3(cx, cy, 0));
                            }
                            



                        }

                        if (conf != JointConfidenceLevel.Low)
                        {
                           
                            

                            images.Add(fotoS);
                           // Debug.WriteLine("add from queue");
                            if (images.Count > 10)
                            {
                                
                            }
                          
                       
                        }


                        //  Debug.WriteLine(fotoS);

                    }
                }
            }
        }
    }
}