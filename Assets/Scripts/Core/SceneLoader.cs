using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static bool paused = false;

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
