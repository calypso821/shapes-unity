using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup OptionPanel;
    [SerializeField] private PauseMenuScript pauseMenuScript;

    public void PlayGame(){
        Time.timeScale =  1f;
        SceneManager.LoadScene(2);
    }

    public void Instructions()
    {
        SceneManager.LoadScene(1);
    }

    public void Option(){
        OptionPanel.alpha = 1;
        OptionPanel.blocksRaycasts = true;
    }

    public void Back(){
        if (pauseMenuScript)
        {
            pauseMenuScript.isPaused = false;
        }
        SceneManager.LoadScene(0);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
