using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitSpawner : MonoBehaviour, IInteractable
{
    [Header("State")]
    [SerializeField] Faction foodFaction = Faction.A;
    [SerializeField] int foodCharges = 0;
    [SerializeField] int numSpawned = 0;
    [SerializeField] bool empty = false;

    [Header("Configure")]
    [SerializeField] Character toSpawn = null;
    [SerializeField] Faction factionToSpawn = Faction.A;

    [Header("Display")]
    [SerializeField] TextMeshProUGUI chargesText = null;
    [SerializeField] TextMeshProUGUI unitsText = null;
    [SerializeField] TextMeshProUGUI objectiveText = null;

    [Header("Reference")]
    [SerializeField] Food food = null;
    [SerializeField] GameObject offering = null;
    [SerializeField] Renderer table = null;
    [SerializeField] public Transform unitTransform = null;
    [SerializeField] public Transform spawnLocation = null;
    //[SerializeField] Renderer rockHome = null;

    private void Awake()
    {
        offering.SetActive(foodCharges > 0);
        UpdateFoodChargeDisplay();

        // Set Faction Color for text and offering table
        Color color = new Color();
        switch (factionToSpawn)
        {
            case Faction.A:
                foodFaction = Faction.B;
                color = Random.Range(0, 2) > 0 ? Color.cyan : new Color(0f, 1f, .5f);
                break;
            case Faction.B:
                foodFaction = Faction.A;
                color = Random.Range(0, 2) > 0 ? Color.yellow : new Color(1f, .5f, 0f);
                break;
        }
        table.SetColor(color);

        if(objectiveText != null)
        {
            objectiveText.color = color;
        }
        if(unitsText != null)
        {
            unitsText.color = color;
        }

    }

    void Start()
    {
        Cycle.newCycleEvent.AddListener(NewCycle);
        StartCoroutine(SpawnStartingUnits());
    }

    private void Update()
    {
        if (!empty && unitTransform.childCount <= 0)
        {
            Debug.Log(name + " is empty");
            empty = true;
            FindObjectOfType<PlayerController>().CheckWinCondition(factionToSpawn);
            enabled = false;
        }
        UpdateNumChildrenDisplay();

    }


    public void NewCycle()
    {
        if(unitTransform.childCount <= 0)
        {
            return;
        }

        if (foodCharges > 0)
        {
            foreach (NPC c in unitTransform.GetComponentsInChildren<NPC>())
            {
                c.hunger.Eat();
            }
            
            foodCharges--;
            UpdateFoodChargeDisplay();
            if (foodCharges <= 0)
            {
                offering.SetActive(false);
                foodFaction = Faction.Default;
            }
        }
        else
        {
            Debug.Log("No food");
            //will anyone die ?
            foreach (Character c in unitTransform.GetComponentsInChildren<Character>())
            {
                c.hunger.DecreaseHunger();
            }
        }

        //make new people
        if (unitTransform.childCount > 1)
        {
            CreateNewUnit();
        }
    }

    //! UNITS
    private IEnumerator SpawnStartingUnits()
    {
        for (int i = 0; i < 2; i++)
        {
            CreateNewUnit();
            yield return new WaitForSeconds(1f);
        }
    }

    private void CreateNewUnit()
    {
        //Debug.Log("Creating New Unit");

        Character c = Instantiate(toSpawn, spawnLocation.position, unitTransform.rotation, unitTransform);
        c.faction = factionToSpawn;
        c.movement.MoveToDestination(unitTransform.position);

        numSpawned++;
        c.name = factionToSpawn.ToString() + numSpawned.ToString();


        UpdateNumChildrenDisplay();
    }

    //! FOOD
    public bool AddOfferring(int value)
    {
        offering.SetActive(true);
        foodCharges += value;
        UpdateFoodChargeDisplay();
        return true;
    }

    public Food RemoveOffering()
    {
        Food f = Instantiate(food);
        f.value = 1;
        f.SetFoodFaction(foodFaction);

        foodCharges--;
        UpdateFoodChargeDisplay();
        if (foodCharges <= 0)
        {
            offering.SetActive(false);
        }
        return f;
    }




    //! DISPLAY

    private void UpdateFoodChargeDisplay()
    {
        chargesText.text = foodCharges.ToString();
        if (foodCharges == 0)
        {
            chargesText.color = Color.red;
        }
        else
        {
            chargesText.color = Color.white;
        }
    }

    private void UpdateNumChildrenDisplay()
    {
        int numChildren = unitTransform.childCount;

        unitsText.text = unitTransform.childCount.ToString();
        if (numChildren == 0)
        {
            unitsText.color = Color.red;
        }
    }








    //! INTERACTION

    public void Interact(Character c)
    {
        if (c != null && c.IsInRange(transform.position))
        {
            Inventory i = c.inventory;
            if (i != null)
            {
                if(i.HasFood())
                {
                    Food f = i.CheckFood();
                    if (f != null)
                    {
                        if (f.GetFoodFaction() != factionToSpawn)
                        {
                            if (AddOfferring(f.value))
                            {
                                Destroy(f.gameObject);
                            }
                        }
                        else
                        {
                            if (AddOfferring(f.value))
                            {
                                foodFaction = Faction.Mixed;
                                Destroy(f.gameObject);
                            }
                        }
                    }
                }
                else
                {
                    if(foodCharges > 0)
                    {
                        c.inventory.TakeItem(RemoveOffering());
                    }
                }
            }
        }
    }

    public CursorType GetCursorType(Character c)
    {
        if (c != null)
        {
            Inventory i = c.inventory;
            if(i != null)
            {
                if(i.HasFood())
                {
                    Food f = i.CheckFood();
                    if (f != null)
                    {
                        if (f.GetFoodFaction() != factionToSpawn)
                        {
                            Debug.Log("Not Cannibal food");
                            return CursorType.Offering;
                        }
                        else
                        {
                            Debug.Log("Cannibal food");
                            return CursorType.OfferingCannibal;
                        }
                    }
                }
                else
                {
                    return CursorType.Food;
                }
            }
        }
        return CursorType.Default;
    }
}
