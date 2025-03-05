using UnityEngine;
using Unity.XR.PXR;
using System;
using System.IO;
using System.Collections.Generic;


public class SuperPicoEyeTracker : MonoBehaviour
{
    [SerializeField] bool initOnStart = false, isOn = false, writeHeadPos = true, writeEuler = true, writeHit = false;
    [SerializeField] GameObject head = null, origin = null;
    [SerializeField] LineRenderer ray;
    [SerializeField] public List<GameObject> targets, targetsStanding, targetsLying;
    private Vector3 endPos = default;

    public string filepath = "", filename = "eyedata";
    private Vector3 leftPos = default, rightPos = default, centerPos = default;
    private Quaternion leftRot = default, rightRot = default, centerRot = default;
    public DataPathCreator pathCreator;
    private EyeTrackingStartInfo startInfo;
    private EyeTrackingStopInfo stopInfo;
    // private EyePupilInfo eyePupilPosition;
    private EyeTrackingDataGetInfo getInfo;
    private EyeTrackingData data;
    //private Posef leftEyePose, rightEyePose;
    //private long timestamp;
    private float leftOpenness = default, rightOpenness = default; // leftPupDiameter = default, rightPupDiameter = default, leftPupPos, rightPupPos;
    private StreamWriter writer = null;
    private bool useVrDebug = false;

    private bool supported;
    private int supportedModesCount;
    private EyeTrackingMode[] supportedModes;


    private void Start()
    {
        if (initOnStart)
        {
            Init();
        }
        ray.gameObject.SetActive(false);
    }

    public void Init()
    {
        if (PlayerPrefs.GetInt("Is_write_data") == 0)
        {
            return;
        }
        
        //string filename = DateTime.Now.ToString("ddMMyyyy_HHmmss_") + this.filename + ".csv";
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
        string headers = "Timestamp;" +
            "leftPos.x;leftPos.y;leftPos.z;leftRot.x;leftRot.y;leftRot.z;leftRot.w;leftOpenness;" +
            "rightPos.x;rightPos.y;rightPos.z;rightRot.x;rightRot.y;rightRot.z;rightRot.w;rightOpenness;" +
            "centerPos.x;centerPos.y;centerPos.z;centerRot.x;centerRot.y;centerRot.z;centerRot.w";
        if (writeEuler)
        {
            headers += ";leftRotEuler.x;leftRotEuler.y;leftRotEuler.z";
            headers += ";rightRotEuler.x;rightRotEuler.y;rightRotEuler.z";
            headers += ";centerRotEuler.x;centerRotEuler.y;centerRotEuler.z";
        }
        if (writeHeadPos)
        {
            headers += ";headPos.x;headPos.y;headPos.z;headRot.x;headRot.y;headRot.z;headRot.w";
            if (writeEuler)
            {
                headers += ";headRotEuler.x;headRotEuler.y;headRotEuler.z";
            }
        }
        

        writer.WriteLine(headers);
        isOn = true;
    }

    private void FixedUpdate()
    {
        if (!isOn)
        {
            return;
        }

        try
        {
            // here we get individual data for each eye
            //
            //int getPerEyePose = PXR_MotionTracking.GetPerEyePose(ref timestamp, ref leftEyePose, ref rightEyePose);
            //if (getPerEyePose == 0)
            //{
            //    try
            //    {
            //        PXR_MotionTracking.GetEyeOpenness(ref leftOpenness, ref rightOpenness);
            //        leftPos = leftEyePose.Position.ToVector3();
            //        rightPos = rightEyePose.Position.ToVector3();
            //        leftRot = leftEyePose.Orientation.ToQuat();
            //        rightRot = rightEyePose.Orientation.ToQuat();
            //    }
            //    catch (Exception e)
            //    {
            //        if (useVrDebug) VRDebugField.Write($"failed to get per eye data - {e}");
            //        Debug.LogException(e);
            //    }
            //}
            //else
            //{
            //    //if (useVrDebug) VRDebugField.Write($"getPerEyePose returned error, code - {getPerEyePose}");
            //    Debug.LogWarning($"getPerEyePose returned error, code - {getPerEyePose}");
            //}
            //
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
                    leftPos = new Vector3(data.eyeDatas[0].pose.position.x, data.eyeDatas[0].pose.position.y, data.eyeDatas[0].pose.position.z);
                    rightPos = new Vector3(data.eyeDatas[1].pose.position.x, data.eyeDatas[1].pose.position.y, data.eyeDatas[1].pose.position.z);
                    //string dataline = $"centerPos = {centerPos}";
                    //if (useVrDebug) VRDebugField.Write(dataline);
                }
                catch (Exception e)
                {
                    
                    Debug.LogException(e);
                }
            }
            else
            {
                
                Debug.LogWarning($"getEyeTrackingData returned error, code - {getEyeTrackingData}");
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
                    leftRot = new Quaternion(data.eyeDatas[0].pose.orientation.x, data.eyeDatas[0].pose.orientation.y, data.eyeDatas[0].pose.orientation.z,
                        data.eyeDatas[0].pose.orientation.w);
                    rightRot = new Quaternion(data.eyeDatas[1].pose.orientation.x, data.eyeDatas[1].pose.orientation.y, data.eyeDatas[1].pose.orientation.z,
                        data.eyeDatas[1].pose.orientation.w);
                    //string dataline = $"centerRot = {centerRot}";
                    //if (useVrDebug) VRDebugField.Write(dataline);
                }
                catch (Exception e)
                {
                    
                    Debug.LogException(e);
                }
            }
            else
            {
                
                Debug.LogWarning($"getEyeTrackingData returned error, code - {getEyeTrackingData}");
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
            //        if (useVrDebug) VRDebugField.Write($"pupil to string = {eyePupilPosition.ToString()}");
            //
            //        // this values are fixed floats, that could be used only with unsafe code
            //        //leftPupPos = eyePupilPosition.leftEyePupilPosition;
            //        //rightPupPos = eyePupilPosition.rightEyePupilPosition;
            //    }
            //    catch (Exception e)
            //    {
            //        if (useVrDebug) VRDebugField.Write($"failed to get pupils data - {e}");
            //    }
            //}
            //else
            //{
            //    if (useVrDebug) VRDebugField.Write($"getPupilInfo returned error, code - {getPerEyePose}");
            //}


            //string datalineDebug = $"timestamp = {timestamp.ToString()}, datetime = {DateTime.Now.ToString("HH:mm:ss.ffffff")}, leftEyePos = {leftPos.ToString()}, leftEyeRot = {leftRot.ToString()}," +
            //    $" leftOpenness = {leftOpenness}, rightEyePos = {rightPos.ToString()}, rightEyeRot = {rightRot.ToString()}, rightOpenness = {rightOpenness}";
            //if (useVrDebug) VRDebugField.Write(datalineDebug);
            string dataline = $"{DateTime.Now.ToString("HH:mm:ss.ffffff")};" +
                    $"{leftPos.x};{leftPos.y};{leftPos.z};{leftRot.x};{leftRot.y};{leftRot.z};{leftRot.w};{leftOpenness};" +
                    $"{rightPos.x};{rightPos.y};{rightPos.z};{rightRot.x};{rightRot.y};{rightRot.z};{rightRot.w};{rightOpenness};" +
                    $"{centerPos.x};{centerPos.y};{centerPos.z};{centerRot.x};{centerRot.y};{centerRot.z};{centerRot.w}";

            if (writeEuler)
            {
                Vector3 leftRotEuler = leftRot.eulerAngles;
                Vector3 rightRotEuler = rightRot.eulerAngles;
                Vector3 centerRotEuler = centerRot.eulerAngles;
                dataline += $";{leftRotEuler.x};{leftRotEuler.y};{leftRotEuler.z}";
                dataline += $";{rightRotEuler.x};{rightRotEuler.y};{rightRotEuler.z}";
                dataline += $";{centerRotEuler.x};{centerRotEuler.y};{centerRotEuler.z}";
            }
            if (writeHeadPos)
            {
                Vector3 headPos = head.transform.position;
                Quaternion headRot = head.transform.rotation;
                Vector3 headRotEuler = head.transform.eulerAngles;
                dataline += $";{headPos.x};{headPos.y};{headPos.z};{headRot.x};{headRot.y};{headRot.z};{headRot.w}";
                if (writeEuler)
                {
                    dataline += $";{headRotEuler.x};{headRotEuler.y};{headRotEuler.z}";
                }
            }

            if (writeHit)
            {
                try
                {
                    Vector3 raycastHit = endPos;
                    dataline += $";{raycastHit.x};{raycastHit.y};{raycastHit.z}";
                }
                catch (Exception e)
                {
                    
                    Debug.LogException(e);
                }
            }

            if (writer != null)
            {
                writer.WriteLine(dataline);
            }

        }
        catch (Exception e)
        {
            
            Debug.LogException(e);
        }

 
    }

    private void DrawDebugRay()
    {
        try
        {
            

            Vector3 newEyesRot = centerRot.eulerAngles;
            newEyesRot = new Vector3(-newEyesRot.x, -newEyesRot.y, newEyesRot.z);
            ray.transform.eulerAngles = newEyesRot;

            Vector3 newEyesPos = new Vector3(centerPos.x, centerPos.y, -centerPos.z);
            ray.transform.position = newEyesPos;

            endPos = ray.transform.TransformPoint(Vector3.forward * 35);
            RaycastHit hitPos;
            if (Physics.Linecast(newEyesPos, endPos, out hitPos))
            {
                endPos = hitPos.point;
                ray.startColor = Color.green;
                ray.endColor = Color.green;
            }
            else
            {
                ray.startColor = Color.blue;
                ray.endColor = Color.blue;
            }
            
            ray.SetPosition(0, newEyesPos);
            ray.SetPosition(1, endPos);
        }
        catch (Exception e)
        {
            
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
        
        Debug.Log("eye tracking stopped");
        ray.gameObject.SetActive(false);
    }
}
