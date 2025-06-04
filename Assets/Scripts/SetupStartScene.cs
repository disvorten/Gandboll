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
    [SerializeField] private TMP_InputField name_input;

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
        name_input.onValueChanged.AddListener(ChangeName);

    }
    private void OpenScene()
    {
        PlayerPrefs.SetString("Difficulty", difficulty.options[difficulty.value].text);
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
