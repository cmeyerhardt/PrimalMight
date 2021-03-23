using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CorpseMapping
{
    public CorpseTypeEnum type;
    public Corpse model;
}

public enum CorpseTypeEnum { Flesh, Scorch, /*Flattened,*/ Bones, Block }
public enum CorpseAction { Nothing, Destroy, Advance, Deplete }

public class CorpseContainer : MonoBehaviour, IInteractable
{
    [SerializeField] Corpse myCorpse = null;
    [SerializeField] ParticleSystem transitionFX = null;
    public Faction corpseFaction;
    int numHits = 3;


    [SerializeField] internal CorpseTypeEnum corpseType;
    [SerializeField] public CorpseTypeEnum CorpseType
    {
        get => corpseType;
        set
        {
            //old method of changing out model - bad performance hit
            //if (myCorpse != null /*&& value != myCorpse.corpseType*/)
            //{
            //    myCorpse.transform.parent = null;
            //    myCorpse.DestroySelf();
            //    myCorpse = null;
            //}
            //myCorpse = Instantiate(corpseDictionary[value], transform.position, transform.rotation, transform);

            //!new method: just SetActive children models 
            if(myCorpse != null)
            {
                myCorpse.gameObject.SetActive(false);
            }

            if(transitionFX != null)
            {
                transitionFX.Play();
            }
            currentCorpseStage++;

            if(transform.childCount > 0)
            {
                Transform t = transform.GetChild(currentCorpseStage%transform.childCount);
                if (t != null)
                {
                    myCorpse = t.GetComponent<Corpse>();
                }
            }
            
            if(myCorpse != null)
            {
                myCorpse.gameObject.SetActive(true);
            }

            corpseType = value;
            SetCorpseSettings();
        }
    }

    int currentCorpseStage = 0;
    
    //Static References
    public static Transform corpseTransform;
    //public static Dictionary<CorpseTypeEnum, Corpse> corpseDictionary = new Dictionary<CorpseTypeEnum, Corpse>();
    
    //Cache
    private int birthday = 0;
    Rigidbody rb = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();



        birthday = Cycle.cycle;
    }

    void Start()
    {
        if (myCorpse == null)
        {
            CorpseType = corpseType;
        }
        Cycle.newCycleEvent.AddListener(CheckDecay);
    }

    public void CheckDecay()
    {
        //Debug.Log("Decay");
        AdvanceCorpseCycle();
        //if (Cycle.cycle - birthday > 3)
        //{
            
        //}
    }

    public void SetCorpseTransform(Transform c)
    {
        corpseTransform = c;
    }

    public void SetFaction(Faction faction)
    {
        corpseFaction = faction;
    }

    //public static void InitializeCorpseDicitonary(CorpseMapping[] mappings)
    //{
    //    foreach (CorpseMapping map in mappings)
    //    {
    //        if (!corpseDictionary.ContainsKey(map.type))
    //        {
    //            corpseDictionary.Add(map.type, map.model);
    //        }
    //    }
    //}

    public void SetCorpseType(CauseOfDeath c)
    {
        switch (c)
        {
            //case CauseOfDeath.Trampling:
            //    CorpseType = CorpseTypeEnum.Flattened;
            //    break;

            case CauseOfDeath.Eaten:
                CorpseType = CorpseTypeEnum.Bones;
                break;

            case CauseOfDeath.Scorched:
                CorpseType = CorpseTypeEnum.Scorch;
                break;

            case CauseOfDeath.Starvation:
            case CauseOfDeath.Default:
            default:
                CorpseType = CorpseTypeEnum.Flesh;
                break;
        }
    }

    public void AdvanceCorpseCycle()
    {
        switch (corpseType)
        {
            case CorpseTypeEnum.Scorch:
            case CorpseTypeEnum.Flesh:
                CorpseType = CorpseTypeEnum.Bones;
                break;
            //case CorpseTypeEnum.Flattened:
            //    break;
            case CorpseTypeEnum.Bones:
                CorpseType = CorpseTypeEnum.Block;
                break;
            case CorpseTypeEnum.Block:
                Debug.Log("AdvanceCorpseCycle: Block");
                break;
        }
    }

    public void SetCorpseSettings()
    {
        switch (corpseType)
        {
            case CorpseTypeEnum.Flesh:
                rb.mass = 10;
                break;
            //case CorpseTypeEnum.Flattened:
            //    rb.mass = 1;
            //    break;
            case CorpseTypeEnum.Bones:
                rb.mass = 8;
                break;
            case CorpseTypeEnum.Scorch:
                rb.mass = 3;
                break;
            case CorpseTypeEnum.Block:
                rb.mass = 20;

                //myCorpse.transform.parent = null;
                myCorpse.gameObject.tag = "Terrain";
                foreach(Transform child in transform)
                {
                    child.gameObject.tag = "Terrain";
                }
                myCorpse.Decouple();
                if(corpseTransform.childCount > 5)
                {
                    Destroy(corpseTransform.GetChild(0).gameObject);
                }
                //Debug.Log("Destroy block now");
                Destroy(gameObject);
                break;
        }
    }

    public void Interact(Character c)
    {
        if (c != null && c.IsInRange(transform.position))
        {
            if (myCorpse.item != null)
            {
                c.inventory.TakeItem(Instantiate(myCorpse.item));
                switch (myCorpse.interactAction)
                {
                    case CorpseAction.Advance:
                        AdvanceCorpseCycle();
                        break;
                    case CorpseAction.Destroy:
                        Destroy(gameObject);
                        break;
                }
            }
            else if (c.inventory.HasWeapon() && myCorpse.corpseType == CorpseTypeEnum.Block)
            {
                numHits--;
                if(numHits == 0)
                {
                    Destroy(gameObject);
                }
            }
        }

    }

    public CursorType GetCursorType(Character c)
    {
        if(myCorpse == null) { return CursorType.Default; }
        switch (myCorpse.corpseType)
        {
            case CorpseTypeEnum.Flesh:
            case CorpseTypeEnum.Scorch:
                return CursorType.Food;

            //case CorpseTypeEnum.Flattened:
            //    return CursorType.PickUp;

            case CorpseTypeEnum.Bones:
                return CursorType.Weapon;

            case CorpseTypeEnum.Block:
                return CursorType.Combat;

            default:
                return CursorType.Default;
        }
    }
}
