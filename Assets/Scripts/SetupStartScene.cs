using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetupStartScene : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown difficulty;
    [SerializeField] private Button start_button;
    [SerializeField] private Button custom_setup_button;
    [SerializeField] private TMP_InputField name_input;
    [SerializeField] private Toggle write_data;

    private void Start()
    {
        int i = 0;
        foreach(var option in difficulty.options)
        {
            if (option.text == PlayerPrefs.GetString("Difficulty", "Легкая"))
            {
                //Debug.Log(option.text);
                //Debug.Log(i);
                //Debug.Log(PlayerPrefs.GetString("Difficulty", "Легкая"));
                break;
            }    
            i++;
        }
        difficulty.value = i;
        name_input.text = PlayerPrefs.GetString("Name", "Имя");
        start_button.onClick.AddListener(() => OpenScene());
        custom_setup_button.onClick.AddListener(() => CustomSetup());
        name_input.onValueChanged.AddListener(ChangeName);
        if (PlayerPrefs.GetInt("Is_write_data", 1) == 1)
            write_data.isOn = true;
        else
            write_data.isOn = false;
    }
    private void OpenScene()
    {
        //Debug.Log(difficulty.value);
        //Debug.Log(difficulty.options[difficulty.value].text);
        PlayerPrefs.SetString("Difficulty", difficulty.options[difficulty.value].text);
        if(write_data.isOn)
            PlayerPrefs.SetInt("Is_write_data", 1);
        else
            PlayerPrefs.SetInt("Is_write_data", 0);
        SceneManager.LoadScene("MainScene");
    }
    private void CustomSetup()
    {
        SceneManager.LoadScene("CustomSetup");
    }

    private void ChangeName(string name)
    {
        PlayerPrefs.SetString("Name", name_input.text);
        //Debug.Log(PlayerPrefs.GetString("Name", "Имя"));
    }
}
