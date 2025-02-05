using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SetupConfig : MonoBehaviour
{
    public ConfigReader config;
    [SerializeField] private GameObject gates;
    public string full_path {  get; private set; }

    private void Awake()
    {
#if UNITY_EDITOR
        var path = $@"Assets/Configs";
#else
        var path = $@"{Application.persistentDataPath}/Configs";
#endif
        full_path = "";
        var difficulty = PlayerPrefs.GetString("Difficulty", "Легкая");
        foreach (var file in Directory.GetFiles(path))
        {
            using (StreamReader sw = File.OpenText(file))
            {
                string result = sw.ReadToEnd();
                string[] lines = result.Split("\n");
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("#"))
                    {
                        if (lines[i].Split("#")[1] == "Name_of_difficulty" && lines[i+1] == difficulty)
                        {
                            full_path = file;
                            break;
                        }
                    }
                }
            }
            if (full_path != "")
                break;
        }

        config.ReadConfig(full_path);

        gates.transform.position = new Vector3(0, (config.target_area[2] - config.target_area[1]) / 2, -config.target_area[3]);
        gates.transform.localScale = new Vector3(config.target_area[0] * 2, config.target_area[2] - config.target_area[1], gates.transform.localScale.z);
    }
}
