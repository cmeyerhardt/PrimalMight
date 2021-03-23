using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Window : MonoBehaviour
{
    [SerializeField] public UnityEvent openEvent;
    [SerializeField] public UnityEvent closeEvent;

    public bool hideMenu = false;
    public static WindowManager windowManager = null;
}
