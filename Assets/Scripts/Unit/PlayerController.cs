using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("State")]
    public bool inControl = true;
    public NPC m_toControl = null;
    public bool pendingControl = false;
    bool isMoving = false;

    [Header("Configure")]
    [SerializeField] float jumpForce = 60f;
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float turnSpeed = 5f;

    [Header("Reference")]
    [SerializeField] AudioMix audioMix = null;
    [SerializeField] AudioMod audioMod = null;
    [SerializeField] Player player = null;
    [SerializeField] CameraControl cameraControl = null;
    [SerializeField] public UnitSpawner factionSpawnerA = null;
    [SerializeField] public UnitSpawner factionSpawnerB = null;
    
    //Static References
    public static PlayerController Instance;
    private static Dictionary<CursorType, Texture2D> cursorDictionary = new Dictionary<CursorType, Texture2D>();
    private static Dictionary<WinCondition, Tuple<Window, ObjectiveDisplay>> winDictionary = new Dictionary<WinCondition, Tuple<Window, ObjectiveDisplay>>();

    //! Initialize
    public static void AssembleCursorDictionary(CursorMapping[] mappings)
    {
        foreach (CursorMapping mapping in mappings)
        {
            if (!cursorDictionary.ContainsKey(mapping.type))
                cursorDictionary.Add(mapping.type, mapping.cursor);
        }
    }
    
    public static void SetWinScreens(WinScreenMapping[] mappings)
    {
        foreach (WinScreenMapping map in mappings)
        {
            if (!winDictionary.ContainsKey(map.winCondition))
            {
                winDictionary.Add(map.winCondition, new Tuple<Window, ObjectiveDisplay>(map.winScreen, map.objectiveDisplay));
            }
        }
    }

    //public static void SetUnitSpawners(UnitSpawner a, UnitSpawner b)
    //{
    //    factionSpawnerA = a;
    //    factionSpawnerB = b;
    //}

    private void Awake()
    {
        if (cameraControl == null)
        {
            cameraControl = GetComponent<CameraControl>();
        }
        Player.playerController = this;
        SceneLoader.PauseEvent.AddListener(TransitionPaused);
    }

    public void TransitionPaused(bool p)
    {
        if(p)
        {
            audioMix.TransitionPaused();
        }
        else
        {
            audioMix.Transition(isMoving, player? player.inCombat : false);
            //audioMix.TransitionBack();
        }
    }

    private void Update()
    {
        if (player == null && m_toControl == null)
        {
            CheckForNewUnitAndVictoryCondition();
            return;
        }

        if(inControl && player != null && player.health.isAlive)
        {
            ProcessInventoryCommands();
            ProcessMovement();
        }


        if (EventSystem.current.IsPointerOverGameObject())
        {
            SetCursor(CursorType.UI);
            return;
        }

        if (!InteractedWithSomething())
        {
            SetCursor(CursorType.Default);
            return;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.transform.position;
        }
    }

    private void ProcessInventoryCommands()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            DropItem();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ConsumeFood();
        }
    }

    public void ConsumeFood()
    {
        if (player != null && player.inventory.HasFood())
        {
            player.hunger.Eat();
            player.inventory.ConsumeFood();
            audioMod.PlayAudioClip(0);
        }
    }

    public void DropItem()
    {
        if (player != null)
        {
            player.inventory.DropItem("");
        }
    }

    private void ProcessMovement()
    {
        // Determine Magnitude of Input to apply to movement
        Vector3 movementVector = Vector3.zero;
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            movementVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        }
        else if(Input.GetMouseButton(1) && Input.GetMouseButton(0))
        {
            movementVector = Vector3.forward;
        }

        if(Input.GetAxis("Rotate") != 0f)
        {
            transform.Rotate(Vector3.up, Input.GetAxis("Rotate") * turnSpeed);
        }


        // If moving, determine direction of movement
        Vector3 relativeForward = player.animator.transform.forward.normalized;
        if (movementVector != Vector3.zero)
        {
            isMoving = true;

            // Get Camera forward
            Transform reference = cameraControl.GetPivot();
            if (reference != null)
            {
                relativeForward = reference.GetRelativeDirectionWithMagnitude(movementVector.x, movementVector.z).normalized;
                player.animator.transform.forward = relativeForward;
            }

            relativeForward *= playerSpeed * Time.deltaTime;// * new Vector3(relativeForward.x * movementVector.x, 0f, relativeForward.z * movementVector.z);//relativeForward.Get.normalized;
            if (!player.movement.isGrounded)
            {
                relativeForward *= .5f;
            }

            player.transform.position += relativeForward;
            //player.movement.MoveInDirection(relativeForward, false);

            if (player.animator != null)
            {
                player.animator.SetFloat("speed", playerSpeed);
            }
  
            //audioMix.Transition(isMoving, player.inCombat);
        }
        else //not moving
        {
            isMoving = false;
            //Debug.Log("Not Moving");
            if (player.animator != null)
            {
                player.animator.SetFloat("speed", 0f);
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump");
            player.rb.AddForce(Vector3.up * jumpForce + relativeForward, ForceMode.Impulse);
        }

        audioMix.Transition(isMoving, player.inCombat);
    }


    

    //! WIN/LOSE CONDITIONS

    public void PlayerDied()
    {
        //Debug.Log("Player died");
        //audioMod.PlayAudioClip(0);
        player.transform.parent = null;
        //CheckForNewUnitAndVictoryCondition();
        pendingControl = false;
        player = null;
    }

    // new unit exists? (assign to member ref if so)
    public void CheckForNewUnitAndVictoryCondition()
    {
        //Debug.Log("CheckForNewUnitAndVictoryCondition: " + pendingControl);
        if (!pendingControl)
        {
            Debug.Log("Checking for new unit");
            bool winThisTime = false;
            m_toControl = null;

            winThisTime = CheckWinCondition(WinCondition.FirstDeath);

            // if there are more friendly units to possess
            NPC[] a = factionSpawnerA.unitTransform.GetComponentsInChildren<NPC>();
            Debug.Log("A: " + a.Length);
            if (a.Length > 0)
            {
                //Debug.Log("A: " + factionSpawnerA.unitTransform.childCount);
                m_toControl = a[0];//0/*factionSpawnerA.unitTransform.childCount - 1*/);
                if (m_toControl == null)
                {
                    // something went terribly wrong, the transform HAS children but NPC component was not found.  
                    Debug.Log("Broken Object: " + a[0].gameObject.name);
                }
                else
                {
                    pendingControl = true;
                }
                //pendingControl = true;
            }
            else
            {
                Debug.Log("No A");
                winThisTime = winThisTime | CheckWinCondition(WinCondition.NoMoreA);
            }

            if (m_toControl == null)
            {
                NPC[] b = factionSpawnerB.unitTransform.GetComponentsInChildren<NPC>();
                Debug.Log("B: " + b.Length);
                if (b.Length > 0)
                {
                    //Debug.Log("B: " + factionSpawnerB.unitTransform.childCount);
                    m_toControl = b[0];//m_toControl = factionSpawnerB.unitTransform.GetChild(0/*factionSpawnerB.unitTransform.childCount - 1*/).GetComponent<NPC>();
                    if (m_toControl == null)
                    {
                        // something went terribly wrong, the transform HAS children but NPC component was not found. 
                        Debug.Log("Broken Object: " + b[0].name);
                    }
                    else
                    {
                        pendingControl = true;
                    }
                }
                else
                {
                    Debug.Log("No B");
                    winThisTime = winThisTime | CheckWinCondition(WinCondition.NoMoreB);
                }
            }

            if (m_toControl == null)
            {
                m_toControl = FindObjectOfType<NPC>();
                if (m_toControl == null)
                {
                    Debug.Log("No A or B");
                    winThisTime = winThisTime | CheckWinCondition(WinCondition.NoMoreAnyone);
                }
                else
                {
                    pendingControl = true;
                }
                //if (factionSpawnerA.unitTransform.childCount <= 0 && factionSpawnerB.unitTransform.childCount <= 0)
                //{

                //}
            }
            
            if(m_toControl != null)
            {
                Debug.Log("Found new unit: " + m_toControl.name + ", win? " + winThisTime);
                if(!winThisTime)
                {
                    pendingControl = true;
                    AssignPlayerNewUnit();
                }
            }
        }

        Debug.Log("Done finding new unit");
    }

    public void AssignPlayerNewUnit()
    {
        if(pendingControl)
        {
            if (m_toControl != null)
            {
                player = m_toControl.gameObject.AddComponent<Player>();
                player.faction = m_toControl.faction;
                cameraControl.SetModel(m_toControl.animator.transform);
                m_toControl = null;

                Player.playerController = this;
                player.gameObject.tag = "Player";
                player.detectors = new List<Character>();

                player.rb.mass = 20f;
                player.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;


                inControl = true;
                pendingControl = false;
                audioMod.PlayAudioClip(1);
            }
            else
            {
                CheckForNewUnitAndVictoryCondition();
            }
        }
    }

    public void FindWinFood()
    {
        _ = CheckWinCondition(WinCondition.FoundFood);
    }


    public bool CheckWinCondition(WinCondition win)
    {
        //Debug.Log("Checking Win: " + win);
        if (winDictionary.ContainsKey(win))
        {
            Debug.Log("Getting Win: " + win);
            Window.windowManager.OpenWindow(winDictionary[win].Item1);

            winDictionary[win].Item2.gameObject.SetActive(true);
            winDictionary[win].Item2.MarkComplete();
            winDictionary.Remove(win);

            //if(win == WinCondition.NoMoreA || win == WinCondition.NoMoreB)
            //{
            //    winDictionary[WinCondition.NoMoreAnyone].Item2.gameObject.SetActive(true);
            //}
            //Debug.Log("Wins left:" + winDictionary.Count);
            return true;
        }
        return false;
    }

    public void CheckWinCondition(Faction faction)
    {
        if(faction == Faction.A)
        {
            _ = CheckWinCondition(WinCondition.NoMoreA);
        }
        else if (faction == Faction.B)
        {
            _ = CheckWinCondition(WinCondition.NoMoreB);
        }
        else
        {
            Debug.Log("Faction is not defined");
        }
    }





    //! RAYCASTING/INTERACTING/CURSOR

    private bool InteractedWithSomething()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, 100f);
        if (hits.Length > 0)
        {
            hits = hits.SortHitsByDistance();
            foreach (RaycastHit hit in hits)
            {
                IInteractable i = hit.collider.GetComponent<IInteractable>();
                if (i != null)
                {
                    //Debug.Log("Mouse over: " + i);
                    SetCursor(i.GetCursorType(player));
                    if (Input.GetMouseButtonDown(0))
                    {
                        i.Interact(player);
                    }
                    return true;
                }
                else
                {
                    i = hit.collider.GetComponentInParent<IInteractable>();
                    if (i != null)
                    {
                        //Debug.Log("Mouse over (p): " + i);
                        SetCursor(i.GetCursorType(player));
                        if (Input.GetMouseButtonDown(0))
                        {
                            i.Interact(player);
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    private void SetCursor(CursorType type)
    {
        if (cursorDictionary.ContainsKey(type))
        {
            //Debug.Log("Setting cursor type: " + type);
            Cursor.SetCursor(cursorDictionary[type], Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Debug.LogWarning("Cursor type missing: " + type);
        }
    }

}



public enum CursorType { Default, UI, Food, Weapon, PickUp, Combat, FoodCannibal, Offering, OfferingCannibal }

[System.Serializable]
public class CursorMapping
{
    public CursorType type;
    public Texture2D cursor;
}

[System.Serializable]
public class WinScreenMapping
{
    public WinCondition winCondition;
    public Window winScreen;
    public ObjectiveDisplay objectiveDisplay;
}

public enum WinCondition { NoMoreA, NoMoreB, NoMoreAnyone, FoundFood, FirstDeath }