using System;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class main : MonoBehaviour
{
    public PhotonConnect photon;

    Tracker tracker;
    Device device = null;
    bool capture = true;

    void Start()
    {
        Task.Run(() => RunBackgroundThreadAsync());
    }

    protected void RunBackgroundThreadAsync()
    {
        var deviceAndCalibration = GetDeviceAndCalibration();

        device = deviceAndCalibration.deviceResult;
        Calibration deviceCalibration = deviceAndCalibration.calibrationResult;

        tracker = GetTracker(deviceCalibration);

        while (capture)
        {
            if (device == null)
            {
                UnityEngine.Debug.LogError("device null, continue to next loop");

                deviceAndCalibration = GetDeviceAndCalibration();
                device = deviceAndCalibration.deviceResult;
                deviceCalibration = deviceAndCalibration.calibrationResult;

                continue;
            }

            if (tracker == null)
            {
                UnityEngine.Debug.LogError("tracker null, continue to next loop");

                tracker = GetTracker(deviceCalibration);

                continue;
            }

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

                Microsoft.Azure.Kinect.BodyTracking.Body body = frame.GetBody(0);
                Microsoft.Azure.Kinect.BodyTracking.Skeleton skeleton = frame.GetBodySkeleton(0);

                int numJoints = Microsoft.Azure.Kinect.BodyTracking.Skeleton.JointCount;//32

                string SkeletalData = string.Empty;
                for (int jointId = 0; jointId < numJoints; jointId++)
                {
                    Microsoft.Azure.Kinect.BodyTracking.Joint joint = skeleton.GetJoint(jointId);
                    System.Numerics.Vector3 positionVector3 = joint.Position;
                    var pos = joint.Position;

                    //* split each joint ( splits joint ID from Vector 3 , split floats

                    if (jointId > 0)
                    {                    
                        SkeletalData += $"*";
                    }

                    SkeletalData += $"{jointId}({pos.X},{pos.Y},{pos.Z}";
                }
                _print(true, SkeletalData);
                photon.SendBodyTrackingEventData(data: SkeletalData);
            }
            catch (Exception e)
            {
                if (e == null)
                {
                    UnityEngine.Debug.LogError("No exception given weird error");
                }
                else
                {
                    UnityEngine.Debug.LogError(e.Message);
                }
            }
        }
    }

    (Device deviceResult, Calibration calibrationResult) GetDeviceAndCalibration()
    {
        device = null;
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

            return (device, deviceCalibration);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
            return (null, new Calibration());
        }
    }

    Tracker GetTracker(Calibration deviceCalibration)
    {
        try
        {
            TrackerConfiguration calibration = new TrackerConfiguration()
            {
                ProcessingMode = TrackerProcessingMode.Gpu,
                SensorOrientation = SensorOrientation.Default
            };
            UnityEngine.Debug.Log("Body tracker created.");

            return Tracker.Create(deviceCalibration, calibration);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
            return null;
        }
    }

    void OnDestroy()
    {
        capture = false;
        device.Dispose();
        tracker.Dispose();
        
    }
    
    void _print(bool shouldPrint, string msg)
    {
        if (shouldPrint) Debug.Log(msg);
    }
}
