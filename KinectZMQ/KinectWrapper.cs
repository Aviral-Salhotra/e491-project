﻿using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System;


namespace KinectZMQ
{
    class KinectWrapper
    {
        static void Main()
        {
            ZMQServer server = new ZMQServer(12345);
            // Open device.
            using (Device device = Device.Open())
            {
                device.StartCameras(new DeviceConfiguration()
                {
                    CameraFPS = FPS.FPS30,
                    ColorResolution = ColorResolution.Off,
                    DepthMode = DepthMode.NFOV_Unbinned,
                    WiredSyncMode = WiredSyncMode.Standalone,
                });

                var deviceCalibration = device.GetCalibration();

                using (Tracker tracker = Tracker.Create(deviceCalibration, new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
                {
                    while (true)
                    {
                        using (Capture sensorCapture = device.GetCapture())
                        {
                            // Queue latest frame from the sensor.
                            tracker.EnqueueCapture(sensorCapture);
                        }

                        // Try getting latest tracker frame.
                        using (Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                        {
                            if (frame != null)
                            {
                                if (frame.NumberOfBodies != 1)
                                {
                                    continue;
                                }
                                Skeleton skeleton = frame.GetBodySkeleton(0);

                                // Find Right Eye Of User - can later me modified to left eye or
                                // midpoint of left and right can also be calculated
                                Joint eye = skeleton.GetJoint(JointId.EyeRight);
                                server.PublishData(eye.Position, eye.Quaternion);
                            }
                        }
                    }
                }
            }
        }
    }
}