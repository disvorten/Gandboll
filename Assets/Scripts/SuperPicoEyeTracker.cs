using UnityEngine;
using Unity.XR.PXR;
using System;
using System.IO;

public class SuperPicoEyeTracker : MonoBehaviour
{
    [SerializeField] bool initOnStart = false;
    [SerializeField] bool isOn = false;

    public DataPathCreator pathCreator;
    public string filename = "eyedata";
    

    private EyeTrackingStartInfo startInfo;
    private EyeTrackingStopInfo stopInfo;
    // private EyePupilInfo eyePupilPosition;
    private EyeTrackingDataGetInfo getInfo;
    private EyeTrackingData data;
    private Vector3 leftPos = default, rightPos = default, centerPos = default;
    private Quaternion leftRot = default, rightRot = default, centerRot = default;
    private Posef leftEyePose, rightEyePose;
    private long timestamp;
    private float leftOpenness = default, rightOpenness = default; // leftPupDiameter = default, rightPupDiameter = default, leftPupPos, rightPupPos;
    private StreamWriter writer = null;


    private void Start()
    {
        if (initOnStart)
        {
            Init();
        }
    }

    public void Init()
    {
        if (PlayerPrefs.GetInt("Is_write_data") == 0)
        {
            return;
        }
        try
        {
            int startSuccess = PXR_MotionTracking.StartEyeTracking(ref startInfo);
            if (startSuccess == 0)
            {
                //VRDebugField.Write("start eye tracking successfull!");
                Debug.Log("start eye tracking successfull!");
            }
            else
            {
                //VRDebugField.Write($"start eye tracking failed, error code - {startSuccess}");
                Debug.LogWarning($"start eye tracking failed, error code - {startSuccess}");
            }
        }
        catch (Exception e)
        {
            //VRDebugField.Write($"start eye exception - {e}");
            Debug.LogException(e);
        }
        string filepath = pathCreator.data_path;
        string filename = this.filename + ".csv";
        string fullpath = Path.Combine(filepath, filename);

        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        //VRDebugField.Write($"writing to: {fullpath}");
        Debug.Log($"writing to: {fullpath}");

        writer = new StreamWriter(fullpath, true, System.Text.Encoding.UTF8);
        writer.WriteLine("Timestamp;" +
            "leftPos.x;leftPos.y;leftPos.z;leftRot.x;leftRot.y;leftRot.z;leftRot.w;leftOpenness;" +
            "rightPos.x;rightPos.y;rightPos.z;rightRot.x;rightRot.y;rightRot.z;rightRot.w;rightOpenness;" +
            "centerPos.x;centerPos.y;centerPos.z;centerRot.x;centerRot.y;centerRot.z;centerRot.w");
        isOn = true;
    }

    private void FixedUpdate()
    {
        if (!isOn || PlayerPrefs.GetInt("Is_write_data") == 0)
        {
            return;
        }

        try
        {
            // here we get individual data for each eye
            int getPerEyePose = PXR_MotionTracking.GetPerEyePose(ref timestamp, ref leftEyePose, ref rightEyePose);
            if (getPerEyePose == 0)
            {
                try
                {
                    PXR_MotionTracking.GetEyeOpenness(ref leftOpenness, ref rightOpenness);
                    leftPos = leftEyePose.Position.ToVector3();
                    rightPos = rightEyePose.Position.ToVector3();
                    leftRot = leftEyePose.Orientation.ToQuat();
                    rightRot = rightEyePose.Orientation.ToQuat();
                }
                catch (Exception e)
                {
                    //VRDebugField.Write($"failed to get per eye data - {e}");
                    Debug.LogException(e);
                }
            }
            else
            {
                //VRDebugField.Write($"getPerEyePose returned error, code - {getPerEyePose}");
                Debug.LogWarning($"getPerEyePose returned error, code - {getPerEyePose}");
            }

            // here we get central eye data, this method could be used for individual eye too, but seems less stable
            // for position
            getInfo = new EyeTrackingDataGetInfo();
            getInfo.flags = EyeTrackingDataGetFlags.PXR_EYE_POSITION;
            int getEyeTrackingData = PXR_MotionTracking.GetEyeTrackingData(ref getInfo, ref data);
            if (getEyeTrackingData == 0)
            {
                try
                {
                    centerPos = new Vector3(data.eyeDatas[2].pose.position.x, data.eyeDatas[2].pose.position.y, data.eyeDatas[2].pose.position.z);
                    //string dataline = $"centerPos = {centerPos}";
                    //VRDebugField.Write(dataline);
                }
                catch (Exception e)
                {
                    //VRDebugField.Write($"failed to get central eye position - {e}");
                    Debug.LogException(e);
                }
            }
            else
            {
                //VRDebugField.Write($"getEyeTrackingData returned error, code - {getPerEyePose}");
                Debug.LogWarning($"getEyeTrackingData returned error, code - {getPerEyePose}");
            }
            // for rotation
            getInfo = new EyeTrackingDataGetInfo();
            getInfo.flags = EyeTrackingDataGetFlags.PXR_EYE_ORIENTATION;
            getEyeTrackingData = PXR_MotionTracking.GetEyeTrackingData(ref getInfo, ref data);
            if (getEyeTrackingData == 0)
            {
                try
                {
                    centerRot = new Quaternion(data.eyeDatas[2].pose.orientation.x, data.eyeDatas[2].pose.orientation.y, data.eyeDatas[2].pose.orientation.z,
                        data.eyeDatas[2].pose.orientation.w);
                    //string dataline = $"centerRot = {centerRot}";
                    //VRDebugField.Write(dataline);
                }
                catch (Exception e)
                {
                    //VRDebugField.Write($"failed to get central eye rotation - {e}");
                    Debug.LogException(e);
                }
            }
            else
            {
                //VRDebugField.Write($"getEyeTrackingData returned error, code - {getPerEyePose}");
                Debug.LogWarning($"getEyeTrackingData returned error, code - {getPerEyePose}");
            }

            // This should work in enterprise version

            //int getPupilInfo = PXR_MotionTracking.GetEyePupilInfo(ref eyePupilPosition);
            //if (getPupilInfo == 0)
            //{
            //    try
            //    {
            //        leftPupDiameter = eyePupilPosition.leftEyePupilDiameter;
            //        rightPupDiameter = eyePupilPosition.rightEyePupilDiameter;
            //
            //        VRDebugField.Write($"pupil to string = {eyePupilPosition.ToString()}");
            //
            //        // this values are fixed floats, that could be used only with unsafe code
            //        //leftPupPos = eyePupilPosition.leftEyePupilPosition;
            //        //rightPupPos = eyePupilPosition.rightEyePupilPosition;
            //    }
            //    catch (Exception e)
            //    {
            //        VRDebugField.Write($"failed to get pupils data - {e}");
            //    }
            //}
            //else
            //{
            //    VRDebugField.Write($"getPupilInfo returned error, code - {getPerEyePose}");
            //}


            string dataline = $"timestamp = {timestamp.ToString()}, datetime = {DateTime.Now.ToString("HH:mm:ss.ffffff")}, leftEyePos = {leftPos.ToString()}, leftEyeRot = {leftRot.ToString()}," +
                $" leftOpenness = {leftOpenness}, rightEyePos = {rightPos.ToString()}, rightEyeRot = {rightRot.ToString()}, rightOpenness = {rightOpenness}";
            //VRDebugField.Write(dataline);

            if (writer != null)
            {
                writer.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffffff")};" +
                    $"{leftPos.x};{leftPos.y};{leftPos.z};{leftRot.x};{leftRot.y};{leftRot.z};{leftRot.w};{leftOpenness};" +
                    $"{rightPos.x};{rightPos.y};{rightPos.z};{rightRot.x};{rightRot.y};{rightRot.z};{rightRot.w};{rightOpenness};" +
                    $"{centerPos.x};{centerPos.y};{centerPos.z};{centerRot.x};{centerRot.y};{centerRot.z};{centerRot.w}");
            }

        }
        catch (Exception e)
        {
            //VRDebugField.Write($"update failed - {e}");
            Debug.LogException(e);
        }
    }

    private void OnApplicationQuit()
    {
        StopWriter();
    }

    public void StopWriter()
    {
        PXR_MotionTracking.StopEyeTracking(ref stopInfo);
        if (writer != null)
        {
            writer.Close();
        }
        writer = null;
        isOn = false;
        //VRDebugField.Write("eye tracking stopped");
        Debug.Log("eye tracking stopped");
    }
}
