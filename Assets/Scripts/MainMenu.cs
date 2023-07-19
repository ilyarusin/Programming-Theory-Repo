using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void OpenLevelList()
    {
        SceneManager.LoadScene(1);
    }

    public void Play()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
        Destroy(GameObject.Find("Audio Source"));
    }
}
