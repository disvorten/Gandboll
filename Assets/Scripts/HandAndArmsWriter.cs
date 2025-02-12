using System;
using System.IO;
using UnityEngine;

public class HandAndArmsWriter : MonoBehaviour
{
    [SerializeField] private SuperPicoEyeTracker eyeTracker;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject right_arm;
    [SerializeField] private GameObject left_arm;
    private bool isOn = false;
    private StreamWriter writerHead;
    private StreamWriter writerLA;
    private StreamWriter writerRA;
    void Start()
    {
        if (PlayerPrefs.GetInt("Is_write_data") == 0)
        {
            return;
        }
        string filepath = eyeTracker.pathCreator.data_path;
        string filename = "HeadData" + ".csv";
        string fullpath = Path.Combine(filepath, filename);
        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }
        writerHead = new StreamWriter(fullpath, true, System.Text.Encoding.UTF8);
        writerHead.WriteLine("Timestamp;Position.x;Position.y;Position.z;Rotation.x;Rotation.y;Rotation.z;Rotation.w");
        if(left_arm.activeSelf)
        {
            filename = "LeftArmData" + ".csv";
            fullpath = Path.Combine(filepath, filename);
            writerLA = new StreamWriter(fullpath, true, System.Text.Encoding.UTF8);
            writerLA.WriteLine("Timestamp;Position.x;Position.y;Position.z;Rotation.x;Rotation.y;Rotation.z;Rotation.w");
        }
        if(right_arm.activeSelf)
        {
            filename = "RightArmData" + ".csv";
            fullpath = Path.Combine(filepath, filename);
            writerRA = new StreamWriter(fullpath, true, System.Text.Encoding.UTF8);
            writerRA.WriteLine("Timestamp;Position.x;Position.y;Position.z;Rotation.x;Rotation.y;Rotation.z;Rotation.w");
        }
        isOn = true;
    }


    void FixedUpdate()
    {
        if (!isOn || PlayerPrefs.GetInt("Is_write_data") == 0)
        {
            return;
        }
        if(writerHead != null)
        {
            var pos = head.transform.position;
            var rot = head.transform.rotation;
            writerHead.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffffff")};" +
            $"{pos.x};{pos.y};{pos.z};{rot.x};{rot.y};{rot.z};{rot.w}");
        }
        if (writerLA != null)
        {
            var pos = left_arm.transform.position;
            var rot = left_arm.transform.rotation;
            writerLA.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffffff")};" +
            $"{pos.x};{pos.y};{pos.z};{rot.x};{rot.y};{rot.z};{rot.w}");
        }
        if (writerRA != null)
        {
            var pos = right_arm.transform.position;
            var rot = right_arm.transform.rotation;
            writerRA.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffffff")};" +
            $"{pos.x};{pos.y};{pos.z};{rot.x};{rot.y};{rot.z};{rot.w}");
        }
    }
    private void OnApplicationQuit()
    {
        StopWriter();
    }

    private void StopWriter()
    {
        if (writerHead != null)
        {
            writerHead.Close();
        }
        if (writerLA != null)
        {
            writerLA.Close();
        }
        if (writerRA != null)
        {
            writerRA.Close();
        }
    }
}
