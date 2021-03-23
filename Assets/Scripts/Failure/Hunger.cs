using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hunger : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] float hungerPercentage = 1f;
    [SerializeField] float hungerDecayRate = .2f; //daily decay rate

    [SerializeField] [Range(-.01f, 1f)] float aggressionThreshold = .1f;
    public UnityEvent starveEvent = new UnityEvent();

    public BoolEvent aggressiveEvent = new BoolEvent();

    Character myCharacter = null;
    //todo -- eating food of same faction will increase aggressionTheshold/cannibal chance
    [SerializeField] Transform hungerBarToScale = null;
    [SerializeField] GameObject hungerBarGO = null;

    //private void Awake()
    //{
    //    Cycle.newCycleEvent.AddListener(DecreaseHunger);
    //}

    public void SetCharacter(Character c)
    {
        myCharacter = c;
    }


    public void IncreaseAggression()
    {
        aggressionThreshold = Mathf.Clamp(aggressionThreshold + .1f, 0f, 1f);
    }

    public void DecreaseAggression()
    {
        aggressionThreshold = Mathf.Clamp(aggressionThreshold - .1f, 0f, 1f);
    }

    public void DisableBar()
    {
        hungerBarGO.SetActive(false);
    }

    public void Eat()
    {
        hungerPercentage = Mathf.Min(hungerPercentage + (hungerDecayRate + .05f), 1f);
        myCharacter.health.GainHealth();
        UpdateHungerDisplay();
        //if (hungerPercentage > aggressionThreshold)
        //{
        //    aggressiveEvent.Invoke(false);
        //}
    }

    public void Eat(Character c)
    {
        c.health.Die(CauseOfDeath.Eaten);
        Eat();
    }

    public void DecreaseHunger()
    {
        Debug.Log("Hunger grows");

        hungerPercentage -= (hungerDecayRate + .05f);

        UpdateHungerDisplay();

        if (hungerPercentage <= 0f)
        {
            starveEvent.Invoke();
        }
        else if (hungerPercentage <= aggressionThreshold)
        {
            aggressiveEvent.Invoke(true);
            Debug.Log(name + " is really hungry now");
        }
    }

    private void UpdateHungerDisplay()
    {
        if (hungerPercentage <= 0f)
        {
            hungerBarGO.SetActive(false);
        }
        else
        {
            hungerBarGO.SetActive(true);
            hungerBarToScale.localScale = new Vector3(Mathf.Clamp(hungerPercentage, 0f, 1f), 1f, 1f);
        }
    }
}
