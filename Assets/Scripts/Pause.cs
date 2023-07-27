using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    public static bool gameIsPaused = false;

    private void Awake()
    {
        pausePanel.SetActive(false);
    }

    public void SetPause()
    {
        gameIsPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void PauseOff()
    {
        gameIsPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
