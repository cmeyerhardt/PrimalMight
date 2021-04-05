using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveDisplay : MonoBehaviour
{
    [SerializeField] GameObject checkMark = null;
    
    private void Awake()
    {
        MarkIncomplete();
    }

    public void MarkIncomplete()
    {
        checkMark.SetActive(false);
    }

    public void MarkComplete()
    {
        checkMark.SetActive(true);
    }

}
