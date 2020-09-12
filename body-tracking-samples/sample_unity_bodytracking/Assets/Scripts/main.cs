﻿using System;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class main : MonoBehaviour
{
    void Start()
    {
        Task.Run(() => RunBackgroundThreadAsync());
    }

    protected void RunBackgroundThreadAsync()
    {
        Device device = null;
        Calibration deviceCalibration = new Calibration();
        try
        {
            const int id = 0;
            device = Device.Open(id);
            device.StartCameras(new DeviceConfiguration()
            {
                CameraFPS = FPS.FPS30,
                ColorResolution = ColorResolution.Off,
                DepthMode = DepthMode.NFOV_Unbinned,
                WiredSyncMode = WiredSyncMode.Standalone,
            });

            UnityEngine.Debug.Log("Open K4A device successful. id " + id + "sn:" + device.SerialNum);

            deviceCalibration = device.GetCalibration();

        } catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }

        Tracker tracker = null;
        try
        {
            TrackerConfiguration calibration = new TrackerConfiguration() {
                ProcessingMode = TrackerProcessingMode.Gpu,
                SensorOrientation = SensorOrientation.Default
            };
            tracker = Tracker.Create(deviceCalibration, calibration);

            UnityEngine.Debug.Log("Body tracker created.");
        } catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
        

        int loopCount = 0;
        while(loopCount < 100)
        {
            try
            {
                Capture sensorCapture = device.GetCapture();
                tracker.EnqueueCapture(sensorCapture);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }

            Frame frame = null;
            try
            {
                frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false);
                if (frame == null)
                {
                    UnityEngine.Debug.Log("Pop result from tracker timeout!");
                    continue;
                }

                if (frame.NumberOfBodies != 1) {
                    _print(true, $"Non-singlular # of bodies: {frame.NumberOfBodies}");
                    continue;
                }

                Microsoft.Azure.Kinect.BodyTracking.Body body = frame.GetBody(0);
                Microsoft.Azure.Kinect.BodyTracking.Skeleton skeleton = frame.GetBodySkeleton(0);

                int numJoints = Microsoft.Azure.Kinect.BodyTracking.Skeleton.JointCount;
                // _print(true, $"numJoints: {numJoints}"); // 32
                for (int jointId = 0; jointId < numJoints; jointId++)
                {
                    Microsoft.Azure.Kinect.BodyTracking.Joint joint = skeleton.GetJoint(jointId);
                    System.Numerics.Vector3 positionVector3 = joint.Position;
                    var pos = joint.Position;
                    // _print(true, "pos: " + (JointId)jointId + " " + pos[0] + " " + pos[1] + " " + pos[2]);
                    _print(true, "pos: " + (JointId)jointId + " " + pos.X + " " + pos.Y + " " + pos.Z);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }

            loopCount += 1;
        }

        _print(true, $"Ran {loopCount} times, disposing");

        tracker.Dispose();
        device.Dispose();
        
    }

    void _print(bool shouldPrint, string msg)
    {
        if (shouldPrint) Debug.Log(msg);
    }
}
