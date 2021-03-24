using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CauseOfDeath { Default, Starvation, Eaten, Scorched }

public class Health : MonoBehaviour
{
    public bool canBeKilled = true;
    public bool isAlive = true;
    public int numLives = 2;
    public int maxNumLives = 2;

    [SerializeField] internal Character myCharacter = null;
    static CorpseContainer corpseContainer = null;
    CauseOfDeath causeOfDeath = CauseOfDeath.Default;
    [SerializeField] Transform healthBarToScale = null;
    [SerializeField] GameObject healthBarGO = null;

    private void Awake()
    {
        myCharacter = GetComponent<Character>();
        Hunger hunger = GetComponent<Hunger>();
        if(hunger != null)
        {
            hunger.starveEvent.AddListener(Starved);
        }
    }

    public void Starved()
    {
        if(canBeKilled)
            Die(CauseOfDeath.Starvation);
    }

    public void GainHealth()
    {
        numLives = Mathf.Clamp(numLives + 1, 0, maxNumLives);
    }
    
    public void LoseHealth(int numLost)
    {
        numLives = Mathf.Clamp(numLives - numLost, 0, maxNumLives);

        if(numLives <= 0)
        {
            healthBarGO.SetActive(false);
            myCharacter.Die();
        }
        else
        {
            UpdateDisplayBar();
        }
    }

    private void UpdateDisplayBar()
    {
        if(numLives <= 0)
        {
            healthBarGO.SetActive(false);
        }
        healthBarToScale.localScale = new Vector3(Mathf.Clamp(numLives / (maxNumLives *  1f), 0f, 1f), 1f, 1f);
    }

    public void DisableBar()
    {
        healthBarGO.SetActive(false);
    }

    public static void InitializeCorpseContainer(CorpseContainer c)
    {
        corpseContainer = c;
    }

    public void Die(CauseOfDeath cause)
    {
        if (!canBeKilled) { return; }

        //Debug.Log("Health / Die");
        isAlive = false;
        causeOfDeath = cause;
        DisableBar();
        myCharacter.inventory.DropItems();
        myCharacter.animator.SetTrigger("die");

    }

    public bool CreateCorpse()
    {

        //Debug.Log("Health / CreateCorpse");
        CorpseContainer c = Instantiate(corpseContainer, transform.position, transform.rotation, CorpseContainer.corpseTransform);
        c.SetCorpseType(causeOfDeath);
        c.SetFaction(myCharacter.faction);

        return true;
    }
}
