using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Cycle : MonoBehaviour
{
    [Header("Configure")]
    [SerializeField] float cycleLength = 20f;

    [Header("Reference")]
    [SerializeField] TextMeshProUGUI dayText = null;
    [SerializeField] Animator animator = null;
    [SerializeField] AudioMod audioMod = null;

    //Cache
    public static UnityEvent newCycleEvent = new UnityEvent();
    float cycleTimer = 0f;
    public static int cycle = 1;

    private void Start()
    {
        cycleTimer = 0f;
        cycle = 1;
    }

    void Update()
    {
        if(cycleTimer > cycleLength)
        {
            NewDay();
        }
        else
        {
            cycleTimer += Time.deltaTime;
        }

        //todo -- display (skybox, lighting, sun/moon) - must be able to "load"/set using value
    }

    public void NewDay()
    {
        animator.SetTrigger("newDay");
        cycleTimer = 0f;
    }

    //public void SkipToNextCycle()
    //{
    //    cycleTimer = 0f;
    //    IncrementCycleCount();
    //    InvokeCycleEvent();
    //}

    public void InvokeCycleEvent()
    {
        if(audioMod != null)
        {
            audioMod.PlayAudioClip(0);
        }

        newCycleEvent.Invoke();
    }

    public void IncrementCycleCount()
    {
        if(dayText != null)
        {
            cycle++;
            dayText.text = cycle.ToString();
        }
    }
}
