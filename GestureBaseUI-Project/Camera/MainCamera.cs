using GestureBaseUI_Project;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GestureBaseUI_Project.Camera
{
    class MainCamera
    {
        /// <summary>
        /// Queue for output images, images are just represent as an array of floats.
        /// </summary>
        private BlockingCollection<float[,]> images;

        /// <summary>
        /// Queue for output hand position
        /// </summary>
        private BlockingCollection<BodyData> bodyData;

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

        public MainCamera(BlockingCollection<float[,]> images, BlockingCollection<BodyData> bodyData)
        {
            this.images = images;
            this.bodyData = bodyData;

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


            StartCamera();
        }

        private const int actualPhotoWidht = 30;
        private const int actualPhotoHeight = 30;
        private const int squareRadious = 150;
        private async void StartCamera()
        {
            //Start body track and sensor camera
            using (Tracker tracker = Tracker.Create(this.kinect.GetCalibration(), new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
            using (Microsoft.Azure.Kinect.Sensor.Image transformedDepth = new Microsoft.Azure.Kinect.Sensor.Image(Microsoft.Azure.Kinect.Sensor.ImageFormat.Depth16, colorWidth, colorHeight, colorWidth * sizeof(UInt16)))
            {
                // Local variables 
                //
                // photo for prediction
                float[,] actualPhoto;

                // hand position in color camera
                Vector3 handPositionColor = new Vector3();

                //hand position in deepth camera
                Vector3 HandPositionDepth = Vector3.Zero;

                //Elbow depth position
                Vector3 ElbowPositionDepth = Vector3.Zero;

                //tracked body id
                uint trackedBodyId = 0;

                // hand reading confidence level
                JointConfidenceLevel handConf = JointConfidenceLevel.Low;

                // elbow reading confindence level
                JointConfidenceLevel elbowConf = JointConfidenceLevel.Low;

                int x, y;

                //body skeleton
                Skeleton body;

                while (running)
                {
                    // create reset prediction photo
                    actualPhoto = new float[actualPhotoWidht, actualPhotoHeight];
                    for (int i = 0; i < 30; i++)
                    {
                        for (int j = 0; j < 30; j++)
                        {
                            actualPhoto[i, j] = 255;
                        }
                    }

                    //get capture
                    using (Capture capture = await Task.Run(() => { return this.kinect.GetCapture(); }))
                    {

                        //get depth transform
                        this.transform.DepthImageToColorCamera(capture, transformedDepth);

                        // Queue latest frame from the sensor.
                        tracker.EnqueueCapture(capture);

                        //get position of the hand
                        using (Microsoft.Azure.Kinect.BodyTracking.Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                        {
                            //check if we have bodies
                            if (frame != null && frame.NumberOfBodies >= 1)
                            {
                                if (trackedBodyId <= 0)
                                {
                                    trackedBodyId = frame.GetBody(0).Id;
                                }

                                //get body
                                body = frame.GetBodySkeleton(0);

                                //check confidence
                                // we will use data only of confidence is medium or high
                                handConf = body.GetJoint(JointId.HandRight).ConfidenceLevel;

                                //skip if confidnece is not high or medium
                                if (handConf == JointConfidenceLevel.Low || handConf == JointConfidenceLevel.None)
                                {
                                    continue;
                                }

                                // get hand position
                                HandPositionDepth = body.GetJoint(JointId.HandRight).Position;

                                // elbow confidence and position
                                ElbowPositionDepth = body.GetJoint(JointId.ElbowRight).Position;
                                elbowConf = body.GetJoint(JointId.ElbowRight).ConfidenceLevel;

                                // get hand position in color camera
                                var handPositionColorQ = kinect.GetCalibration().TransformTo2D(HandPositionDepth, CalibrationDeviceType.Depth, CalibrationDeviceType.Color);

                                handPositionColor.X = handPositionColorQ.Value.X;
                                handPositionColor.Y = handPositionColorQ.Value.Y;
                                handPositionColor.Z = HandPositionDepth.Z;

                                // loop thorugh all color, and select depth that are insede the requerid square

                                // show normal camera
                                var color = capture.Color;

                                // we need unsafe for use pointer to array
                                unsafe
                                {
                                    // get memory handler
                                    using (var pin = color.Memory.Pin())
                                    {
                                        // get pointer to depth capture, this is basically a array
                                        ushort* depthPixels = (ushort*)transformedDepth.Memory.Pin().Pointer;

                                        // staring point of the hand square
                                        bool isFirst = true;
                                        int xref = 0, yref = 0;
                                        x = 0; y = 0;
                                        //loop through the depht pixel arrat
                                        for (int i = 0; i < this.colorHeight * this.colorWidth; i++)
                                        {
                                            //kepp coubnt of x and y for 2d array
                                            x++;
                                            if (i % 1280 == 0)
                                            {
                                                x = 0;
                                                y++;
                                            }
                                            //insede the square
                                            if ((y - squareRadious) < ((int)handPositionColor.Y) && (y + squareRadious) > ((int)handPositionColor.Y) && (x - squareRadious) < ((int)handPositionColor.X) && (x + squareRadious) > ((int)handPositionColor.X))
                                            {
                                                //get corner of the square if is the first
                                                if (isFirst)
                                                {
                                                    xref = x;
                                                    yref = y;
                                                    isFirst = false;
                                                }

                                                //select pixels insede the selected deep
                                                if (depthPixels[i] < handPositionColor.Z + 50 && depthPixels[i] > handPositionColor.Z - 150 && depthPixels[i] != 0)
                                                {
                                                    //scale actual depth
                                                    float deep = (((int)depthPixels[i] - ((int)handPositionColor.Z - 150)) * 250) / 200;
                                                   

                                                    uint xs = (uint)((x - xref) / 10);
                                                    uint ys = (uint)((y - yref) / 10);

                                                    // we only add the max or the first, is first when is 255
                                                    if ((uint)deep > actualPhoto[ys, xs] || actualPhoto[ys, xs] == 255)
                                                    {
                                                        actualPhoto[ys, xs] = deep;

                                                    }
                                                }
                                            }

                                        }

                                    }
                                }//unsafe

                            }
                        }
                    }

                    //add to queues
                    images.Add(actualPhoto);
                    bodyData.Add(new BodyData()
                    {
                        HandConfidence = handConf,
                        HandPosition = HandPositionDepth,
                        ElvowPosition = ElbowPositionDepth
                    }) ;

                }//while (running)
            }

        }

        public void Close()
        {
            running = false;
            // wait 0.5 second for be sure the last loop was done.
            Thread.Sleep(500);
            if (this.kinect != null)
            {
                this.kinect.Dispose();
            }
        }
    }
}
