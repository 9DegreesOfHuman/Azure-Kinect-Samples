using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class SkeletalTrackingProvider : BackgroundDataProvider
{
    bool readFirstFrame = false;
    TimeSpan initialTimestamp;

    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter { get; set; } = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

    public Stream RawDataLoggingFile = null;

    protected override void RunBackgroundThreadAsync(int id)
    {
        try
        {
            UnityEngine.Debug.Log("Starting body tracker background thread.");

            // Buffer allocations.
            BackgroundData currentFrameData = new BackgroundData();
            // Open device.
            using (Device device = Device.Open(id))
            {
                device.StartCameras(new DeviceConfiguration()
                {
                    CameraFPS = FPS.FPS30,
                    ColorResolution = ColorResolution.Off,
                    DepthMode = DepthMode.NFOV_Unbinned,
                    WiredSyncMode = WiredSyncMode.Standalone,
                });

                UnityEngine.Debug.Log("Open K4A device successful. id " + id + "sn:" + device.SerialNum );

                var deviceCalibration = device.GetCalibration();

                using (Tracker tracker = Tracker.Create(deviceCalibration, new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
                {
                    UnityEngine.Debug.Log("Body tracker created.");
                    while (m_runBackgroundThread)
                    {
                        using (Capture sensorCapture = device.GetCapture())
                        {
                            // Queue latest frame from the sensor.
                            tracker.EnqueueCapture(sensorCapture);
                        }

                        // Try getting latest tracker frame.
                        using (Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                        {
                            if (frame == null)
                            {
                                UnityEngine.Debug.Log("Pop result from tracker timeout!");
                            }
                            else
                            {
                                IsRunning = true;
                                // Get number of bodies in the current frame.
                                currentFrameData.NumOfBodies = frame.NumberOfBodies;

                                if (frame.NumberOfBodies != 1) { _print(true, $"Non-singlular # of bodies: {frame.NumberOfBodies}"); }
                                if (frame.NumberOfBodies == 1) {
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
                            }

                        }
                    }
                    tracker.Dispose();
                }
                device.Dispose();
            }
            if (RawDataLoggingFile != null)
            {
                RawDataLoggingFile.Close();
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }

    void _print(bool shouldPrint, string msg) {
        if (shouldPrint) Debug.Log(msg);
    }
}
