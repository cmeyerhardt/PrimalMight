using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static BoolEvent PauseEvent = new BoolEvent();
    public static bool paused = false;
    public static bool Paused
    {
        set
        {
            paused = value;
            Time.timeScale = paused ? 0f : 1f;
            PauseEvent.Invoke(paused);
        }
        get => paused;

    }

    public void Pause()
    {
        paused = true;
        Time.timeScale = 0f;
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        paused = false;
    }

    public void Reload()
    {
        UnPause();
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        UnPause();
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
