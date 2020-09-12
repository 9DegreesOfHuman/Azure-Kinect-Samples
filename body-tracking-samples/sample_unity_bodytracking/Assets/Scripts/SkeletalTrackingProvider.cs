// using System;
// using System.Threading.Tasks;
// using Microsoft.Azure.Kinect.Sensor;
// using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class SkeletalTrackingProvider : MonoBehaviour
{
    // public void StartClientThread()
    // void Start()
    // {
    //     Task.Run(() => RunBackgroundThreadAsync());
    // }

    // protected void RunBackgroundThreadAsync()
    // {
    //     try
    //     {
    //         UnityEngine.Debug.Log("Starting body tracker background thread.");

    //         // Buffer allocations.
    //         // Open device.
    //         const int id = 0;
    //         using (Device device = Device.Open(id))
    //         {
    //             device.StartCameras(new DeviceConfiguration()
    //             {
    //                 CameraFPS = FPS.FPS30,
    //                 ColorResolution = ColorResolution.Off,
    //                 DepthMode = DepthMode.NFOV_Unbinned,
    //                 WiredSyncMode = WiredSyncMode.Standalone,
    //             });

    //             UnityEngine.Debug.Log("Open K4A device successful. id " + id + "sn:" + device.SerialNum );

    //             var deviceCalibration = device.GetCalibration();

    //             using (Tracker tracker = Tracker.Create(deviceCalibration, new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
    //             {
    //                 UnityEngine.Debug.Log("Body tracker created.");
    //                 // while (m_runBackgroundThread)
    //                 while (true)
    //                 {
    //                     using (Capture sensorCapture = device.GetCapture())
    //                     {
    //                         // Queue latest frame from the sensor.
    //                         tracker.EnqueueCapture(sensorCapture);
    //                     }

    //                     // Try getting latest tracker frame.
    //                     using (Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
    //                     {
    //                         if (frame == null)
    //                         {
    //                             UnityEngine.Debug.Log("Pop result from tracker timeout!");
    //                         }
    //                         else
    //                         {
    //                             // IsRunning = true;
    //                             // Get number of bodies in the current frame.
    //                             // currentFrameData.NumOfBodies = frame.NumberOfBodies;

    //                             if (frame.NumberOfBodies != 1) { _print(true, $"Non-singlular # of bodies: {frame.NumberOfBodies}"); }
    //                             if (frame.NumberOfBodies == 1) {
    //                                 Microsoft.Azure.Kinect.BodyTracking.Body body = frame.GetBody(0);
    //                                 Microsoft.Azure.Kinect.BodyTracking.Skeleton skeleton = frame.GetBodySkeleton(0);

    //                                 int numJoints = Microsoft.Azure.Kinect.BodyTracking.Skeleton.JointCount;
    //                                 // _print(true, $"numJoints: {numJoints}"); // 32
    //                                 for (int jointId = 0; jointId < numJoints; jointId++)
    //                                 {
    //                                     Microsoft.Azure.Kinect.BodyTracking.Joint joint = skeleton.GetJoint(jointId);
    //                                     System.Numerics.Vector3 positionVector3 = joint.Position;
    //                                     var pos = joint.Position;
    //                                     // _print(true, "pos: " + (JointId)jointId + " " + pos[0] + " " + pos[1] + " " + pos[2]);
    //                                     _print(true, "pos: " + (JointId)jointId + " " + pos.X + " " + pos.Y + " " + pos.Z);
    //                                 }
    //                             }
    //                         }

    //                     }
    //                 }
    //                 // tracker.Dispose();
    //             }
    //             // device.Dispose();
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         UnityEngine.Debug.LogError(e.Message);
    //     }
    // }

    // void _print(bool shouldPrint, string msg) {
    //     if (shouldPrint) Debug.Log(msg);
    // }
}
