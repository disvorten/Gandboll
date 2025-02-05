using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetAndBack : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void GoBack()
    {
        SceneManager.LoadScene("LoadingScene");
    }
    public void ResetScene()
    {
        SceneManager.LoadScene("LoadingScene");
        SceneManager.LoadScene("MainScene");
    }
}
