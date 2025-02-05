using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataPathCreator : MonoBehaviour
{
    public string data_path {get; private set; }
    void Awake()
    {
#if UNITY_EDITOR
        data_path = $"Assets/Data";
#else
        data_path = Application.persistentDataPath + $"/Data";
#endif
        if(!Directory.Exists(data_path))
            Directory.CreateDirectory(data_path);
        data_path += @$"/{PlayerPrefs.GetString("Name", "Èìÿ")}_{DateTime.Now:yyyyMMdd_HHmmss}";
        if(PlayerPrefs.GetInt("Is_write_data") == 1)
        {
            Directory.CreateDirectory(data_path);
        }
    }

}
