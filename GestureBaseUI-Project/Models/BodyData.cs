using Microsoft.Azure.Kinect.BodyTracking;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GestureBaseUI_Project
{
    // Body data for controlling the mouse.
    public class BodyData
    {
        public Vector3 HandPosition { get; set; }
        public JointConfidenceLevel HandConfidence { get; set; }

        public Vector3 ElvowPosition { get; set; }
    }
}
