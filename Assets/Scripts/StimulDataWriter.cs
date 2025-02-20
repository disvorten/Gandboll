using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class StimulDataWriter : MonoBehaviour
{
    public string config_path { private get; set; }
    public int stimul_number { private get; set; }
    public string data_path { private get; set; }
    private string stimul_path;
    void Start()
    {
        if(PlayerPrefs.GetInt("Is_write_data") == 1)
        {
            stimul_path = data_path + "/StimulsData";
            Directory.CreateDirectory(stimul_path);
            //Debug.Log(stimul_path);
            //Debug.Log(config_path);
            //Debug.Log(data_path);
#if UNITY_EDITOR
            var cfg = data_path + @$"/{config_path.Split("\\")[^1]}";
#else
            var cfg = data_path + @$"/{config_path.Split("/")[^1]}";
#endif
            File.CreateText(cfg).Close();
            using (StreamWriter sw = File.AppendText(stimul_path + $"/{stimul_number}_stimul.csv"))
            {
                sw.WriteLine($"Point`s of stimul`s start time :{DateTime.Now:HH:mm:ss.fffff}: Delay before stimul appears :{GetComponent<Shooter_controller>().delta_before_shoot}: Velocity :{GetComponent<Shooter_controller>().velocity}:");
                sw.WriteLine("Timestamp;Position.x;Position.y;Position.z;Is_false_stimul;Is_catched");
                sw.Close();
            }
            File.Copy(config_path, cfg, true);
        }
    }

    void FixedUpdate()
    {
        if (PlayerPrefs.GetInt("Is_write_data") == 1)
        {
            if (transform.childCount >= 2)
            {
                using (StreamWriter sw = File.AppendText(stimul_path + $"/{stimul_number}_stimul.csv"))
                {
                    var position = transform.GetChild(1).position;
                    var rotation = transform.GetChild(1).rotation;
                    int false_stim = GetComponent<Shooter_controller>().is_false_stimul == true ? 1 : 0;
                    int is_catched = GetComponent<Shooter_controller>().is_catched == true ? 1 : 0;
                    sw.WriteLine($"{DateTime.Now:HH:mm:ss.fffff};{position.x};{position.y};{position.z};{false_stim};{is_catched}");
                    sw.Close();
                }
            }
        }
    }
}
