using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

[CreateAssetMenu]
public class ConfigReader : ScriptableObject
{
    public Vector3 throw_area { get; private set; }
    public Vector2 throw_area_depth { get; private set; }
    public float number_of_stimuls { get; private set; }
    public float value_of_velocity_increase { get; private set; }
    public float mass_of_stimul { get; private set; }
    public Vector2 delta_t { get; private set; }
    public Vector2 delta_before_shoot { get; private set; }
    public Vector2 stimuls_velocity { get; private set; }
    public bool is_false_stimuls_exists { get; private set; }
    public Vector4 target_area { get; private set; }
    public bool use_gravity { get; private set; }

    private string delimeter = " ";
    public float false_stimuls_percentage { get; private set; }
    public float reflection_percentage { get; private set; }
    public float diameter_of_stimul { get; private set; }
    public void ReadConfig(string name)
    {
        using(StreamReader sw  = File.OpenText(name))
        {
            string result = sw.ReadToEnd();
            string[] lines = result.Split("\n");
            CultureInfo culture = CultureInfo.InvariantCulture;
            for(int i=0; i<lines.Length; i++)
            {
                if(lines[i].StartsWith("#"))
                {
                    Debug.Log(lines[i]);
                    Debug.Log(lines[i + 1]);
                    switch (lines[i].Split("#")[1])
                    {
                        case "throw_area":
                            throw_area = new Vector3(float.Parse(lines[i + 1].Split(delimeter)[0], culture), float.Parse(lines[i + 1].Split(delimeter)[1], culture), float.Parse(lines[i + 1].Split(delimeter)[2], culture));
                            throw_area_depth = new Vector2(float.Parse(lines[i + 1].Split(delimeter)[3], culture), float.Parse(lines[i + 1].Split(delimeter)[4], culture));
                            break;
                        case "number_of_stimuls":
                            number_of_stimuls = float.Parse(lines[i + 1], culture);
                            break;
                        case "diameter_of_stimul":
                            diameter_of_stimul = float.Parse(lines[i + 1], culture);
                            break;
                        case "value_of_velocity_increase":
                            value_of_velocity_increase = float.Parse(lines[i + 1], culture);
                            break;
                        case "false_stimuls_percentage":
                            false_stimuls_percentage = float.Parse(lines[i + 1], culture);
                            break;
                        case "reflection_percentage":
                            reflection_percentage = float.Parse(lines[i + 1], culture);
                            break;
                        case "mass_of_stimul":
                            mass_of_stimul = float.Parse(lines[i + 1], culture);
                            break;
                        case "delta_t":
                            delta_t = new Vector2(float.Parse(lines[i + 1].Split(delimeter)[0], culture), float.Parse(lines[i + 1].Split(delimeter)[1], culture));
                            break;
                        case "delta_before_shoot":
                            delta_before_shoot = new Vector2(float.Parse(lines[i + 1].Split(delimeter)[0], culture), float.Parse(lines[i + 1].Split(delimeter)[1], culture));
                            break;
                        case "stimuls_velocity":
                            stimuls_velocity = new Vector2(float.Parse(lines[i + 1].Split(delimeter)[0], culture), float.Parse(lines[i + 1].Split(delimeter)[1], culture));
                            break;
                        case "is_false_stimuls_exists":
                            is_false_stimuls_exists = bool.Parse(lines[i + 1]);
                            break;
                        case "target_area":
                            target_area = new Vector4(float.Parse(lines[i + 1].Split(delimeter)[0], culture), float.Parse(lines[i + 1].Split(delimeter)[1], culture), float.Parse(lines[i + 1].Split(delimeter)[2], culture), float.Parse(lines[i + 1].Split(delimeter)[3], culture));
                            break;
                        case "use_gravity":
                            use_gravity = bool.Parse(lines[i + 1]);
                            break;
                    }
                    
                }
            }
        }
    }
}
