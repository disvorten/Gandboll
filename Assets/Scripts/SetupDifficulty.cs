using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SetupDifficulty : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Dropdown diff;
    [SerializeField] private TMPro.TMP_Text text;
    void Awake()
    {
#if UNITY_EDITOR
        var path = $@"Assets/Configs";
#else
        var path = $@"{Application.persistentDataPath}/Configs";
#endif
        //text.text = path;
        List<TMPro.TMP_Dropdown.OptionData> options = new List<TMPro.TMP_Dropdown.OptionData>();
        foreach (var file in Directory.GetFiles(path))
        {
            //text.text += "  " + file;
            using (StreamReader sw = File.OpenText(file))
            {
                string result = sw.ReadToEnd();
                string[] lines = result.Split("\n");
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("#"))
                    {
                        if (lines[i].Split("#")[1] == "Name_of_difficulty")
                        {
                            TMPro.TMP_Dropdown.OptionData optionData = new(lines[i + 1]);
                            options.Add(optionData);
                            //text.text += lines[i + 1];
                            break;
                        }
                    }
                }
            }
        }
        diff.options = options;
    }

}
