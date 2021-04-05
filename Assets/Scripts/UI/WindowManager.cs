using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [Header("Open Windows")]
    [SerializeField] List<Window> openWindows = new List<Window>();

    [Header("Windows")]
    [SerializeField] Window escapeWindow = null;
    [SerializeField] Window tutorialWindow = null;
    [SerializeField] Window loseWindow = null;

    [Header("Windows Configure")]
    [SerializeField] GameObject menuToBeHidden = null;
    [SerializeField] Window menuToShowOnAwake = null;

    //[Header("Reference")]
    //[SerializeField] Transform windowsTransform = null;

    private void Awake()
    {
        if (Window.windowManager == null)
        {
            Window.windowManager = this;
        }
        else
        {
            Debug.LogWarning("Duplicate WindowManager found on " + gameObject.name);
            Destroy(this);
        }

        foreach (Window window in transform.GetComponentsInChildren<Window>())
        {
            CloseWindow(window, false);
            window.gameObject.SetActive(false);
        }

        ShowAwakeMenu();
    }

    public void ShowAwakeMenu()
    {
        if (menuToShowOnAwake != null)
        {
            OpenWindow(menuToShowOnAwake);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (openWindows.Count > 0)
            {
                CloseWindow(openWindows[openWindows.Count - 1], true);
            }
            else
            {
                OpenWindow(escapeWindow);
            }
        }
    }

    public void OpenEscapeWindow()
    {
        OpenWindow(escapeWindow);
    }

    public void OpenTutorialWindow()
    {
        OpenWindow(tutorialWindow);
    }

    public void CloseTutorialWindow()
    {
        CloseWindow(tutorialWindow);
    }

    public void OpenLoseWindow()
    {
        OpenWindow(loseWindow);
    }

    //public void OpenTitleWindow()
    //{
    //    OpenWindow(titleWindow);
    //}


    public void CloseWindow(Window windowToClose)
    {
        CloseWindow(windowToClose, true);
    }

    public void CloseWindow(Window windowToClose, bool doEvent)
    {
        //Debug.Log("CloseWindow:" + windowToClose.name);
        if (windowToClose == null) { return; }
        if (doEvent)
            windowToClose.closeEvent.Invoke();


        openWindows.Remove(windowToClose);
        windowToClose.gameObject.SetActive(false);

        //if(recover)
        //{
        //    Window w = RecoverWindow();
        //    if(w != null)
        //    {
        //        OpenWindow(RecoverWindow());
        //    }
        //}

        if (openWindows.Count <= 0)
        {
            SceneLoader.Paused = false;
            if (menuToBeHidden != null)
            {
                menuToBeHidden.SetActive(true);
            }
        }
    }

    //public void SaveEscapeWindow()
    //{
    //    SaveWindow(escapeWindow);
    //}

    //public void SaveWindow(Window windowToSave)
    //{
    //    //Debug.Log("SaveWindow:" + windowToSave);
    //    if (windowToSave != null)
    //    {
    //        windowsToReEnable.Push(windowToSave);
    //        openWindows.Remove(windowToSave);
    //        windowToSave.gameObject.SetActive(false);
    //        //CloseWindow(windowToSave, false);
    //    }
    //}

    //public Window RecoverWindow()
    //{
    //    if (windowsToReEnable.Count > 0)
    //    {
    //        //Debug.Log("Recovering");
    //        return windowsToReEnable.Pop();
    //    }
    //    return null;
    //}


    public void OpenWindow(Window windowToOpen)
    {
        if (windowToOpen == null) { return; }
        //do not reopen window if already open
        if (openWindows.Contains(windowToOpen)) { return; }

        openWindows.Add(windowToOpen);
        windowToOpen.gameObject.SetActive(true);

        if (/*windowToOpen.hideMenu &&*/ menuToBeHidden != null)
        {
            menuToBeHidden.SetActive(false);
        }

        if(openWindows.Count > 0)
        {
            SceneLoader.Paused = true;
        }

        windowToOpen.openEvent.Invoke();

    }

    public void CloseWindows(bool doEvent = true)
    {
        foreach (Window openWindow in openWindows)
        {
            if (doEvent)
                openWindow.closeEvent.Invoke();
            openWindow.gameObject.SetActive(false);
        }
        openWindows.Clear();

        if (openWindows.Count <= 0)
        {
            SceneLoader.Paused = false;
        }
        menuToBeHidden.SetActive(true);
    }
}
