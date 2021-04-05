using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActiveChild : MonoBehaviour
{
    int index = 0;

    private void Awake()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        if(transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            index = 0;
        }
        else
        {
            Debug.LogWarning("No Children to Switch");
            enabled = false;
        }
    }

    public void Next()
    {
        transform.GetChild(index).gameObject.SetActive(false);

        if (index + 1 >= transform.childCount)
        {
            index = 0;
        }
        else
        {
            index++;
        }

        transform.GetChild(index).gameObject.SetActive(true);
    }

    public void Previous()
    {
        transform.GetChild(index).gameObject.SetActive(false);

        if (index - 1 < 0)
        {
            index = transform.childCount - 1;
        }
        else
        {
            index--;
        }

        transform.GetChild(index).gameObject.SetActive(true);
    }
}
