using Unity.VisualScripting;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    public bool isPaused = false;

    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                Resume();
            } else {
                Pause();
            }
        }
    }

    void Resume(){
        pauseMenuUI.SetActive(false);
        Time.timeScale =  1f;
        isPaused = false;

    }

    void Pause(){
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
}
