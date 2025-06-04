using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.PXR;
using UnityEngine;

public class CalibrationEyesWriter : MonoBehaviour
{
    [SerializeField] bool initOnStart = false, isOn = false, writeHeadPos = true, writeEuler = true, writeHit = false;
    [SerializeField] GameObject head = null, origin = null;
    [SerializeField] LineRenderer ray;
    [SerializeField] public List<GameObject> targets, targetsStanding, targetsLying;
    private Vector3 endPos = default;

    public string filepath = "", filename = "Eyedata";
    private Vector3 leftPos = default, rightPos = default, centerPos = default;
    private Quaternion leftRot = default, rightRot = default, centerRot = default;
    private EyeTrackingStartInfo startInfo;
    private EyeTrackingStopInfo stopInfo;
    private EyeTrackingDataGetInfo getInfo;
    private EyeTrackingData data;
    private float leftOpenness = default, rightOpenness = default; 
    private StreamWriter writer = null;

    private bool supported;
    private int supportedModesCount;
    private EyeTrackingMode[] supportedModes;

    private StreamWriter writerHead;



    private void Start()
    {
        if (initOnStart)
        {
            Init();
#if UNITY_EDITOR
            var filepath = $@"Assets/Data";
#else
            var filepath = $@"{Application.persistentDataPath}";
#endif
            string filename = "HeadData" + ".csv";
            string fullpath = Path.Combine(filepath, filename);
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            writerHead = new StreamWriter(fullpath, true, System.Text.Encoding.UTF8);
            writerHead.WriteLine("Timestamp;Position.x;Position.y;Position.z;Rotation.x;Rotation.y;Rotation.z;Rotation.w");
        }
        ray.gameObject.SetActive(false);
    }

    public void Init()
    {

        try
        {
            int startSuccess = PXR_MotionTracking.StartEyeTracking(ref startInfo);
            if (startSuccess == 0)
            {
                Debug.Log("start eye tracking successfull!");

                PXR_MotionTracking.GetEyeTrackingSupported(ref supported, ref supportedModesCount, ref supportedModes);
            }
            else
            {
                Debug.LogWarning($"start eye tracking failed, error code - {startSuccess}");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }


#if UNITY_EDITOR
        var filepath = $@"Assets/Data";
#else
        var filepath = $@"{Application.persistentDataPath}";
#endif
        string filename = this.filename + ".csv";
        string fullpath = Path.Combine(filepath, filename);

        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

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
        if (writerHead != null)
        {
            var pos = head.transform.position;
            var rot = head.transform.rotation;
            writerHead.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffffff")};" +
            $"{pos.x};{pos.y};{pos.z};{rot.x};{rot.y};{rot.z};{rot.w}");
        }
        try
        {
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
    

    private void OnApplicationQuit()
    {
        StopWriter();
    }

    public void StopWriter()
    {
        PXR_MotionTracking.StopEyeTracking(ref stopInfo);
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
        }
        writer = null;
        isOn = false;

        Debug.Log("eye tracking stopped");
        ray.gameObject.SetActive(false);
        if (writerHead != null)
        {
            writerHead.Flush();
            writerHead.Close();
        }
    }
}
