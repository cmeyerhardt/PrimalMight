using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] AudioMix audioMix = null;
    [SerializeField] AudioMod audioMod = null;
    [SerializeField] Player player = null;
    [SerializeField] CameraControl cameraControl = null;
    [SerializeField] float jumpForce = 60f;
    public bool inControl = true;
    //Static References
    public static PlayerController Instance;
    private static Dictionary<CursorType, Texture2D> cursorDictionary = new Dictionary<CursorType, Texture2D>();
    public static UnitSpawner factionSpawnerA = null;
    public static UnitSpawner factionSpawnerB = null;

    public NPC toControl = null;
    bool pendingControl = false;

    //! Initialize
    public static void AssembleCursorDictionary(CursorMapping[] mappings)
    {
        foreach (CursorMapping mapping in mappings)
        {
            if (!cursorDictionary.ContainsKey(mapping.type))
                cursorDictionary.Add(mapping.type, mapping.cursor);
        }
    }
    public static void SetUnitSpawners(UnitSpawner a, UnitSpawner b)
    {
        factionSpawnerA = a;
        factionSpawnerB = b;
    }

    private void Awake()
    {
        if (cameraControl == null)
        {
            cameraControl = GetComponent<CameraControl>();
        }
        Player.playerController = this;
    }

    private void Update()
    {
        if (player == null)
        {
            CheckForNewUnitAndVictoryCondition();
            return;
        }

        if(inControl)
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
            if (player != null)
            {
                player.inventory.DropItem("");
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (player != null && player.inventory.HasFood())
            {
                player.hunger.Eat();
                player.inventory.ConsumeFood();
                audioMod.PlayAudioClip(0);
            }
        }
    }
    
    private void ProcessMovement()
    {
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            Vector3 forward = cameraControl.GetPivot().transform.forward;
            Vector3 right = cameraControl.GetPivot().transform.right;

            // zero out y-axis
            forward.y = 0f;
            right.y = 0f;

            Vector3 relativeForward = forward.normalized * Input.GetAxis("Vertical") + right.normalized * (Input.GetAxis("Horizontal"));
            player.animator.transform.forward = forward;

            player.movement.UpdateAnimatorSpeed();

            Vector3 movementVector = 10f * Time.deltaTime * relativeForward.normalized;
            if (!player.movement.isGrounded)
            {
                movementVector *= .5f;
            }

            player.transform.position += movementVector;
            audioMix.Transition(true, player.inCombat);
        }
        else if (Input.GetMouseButton(1) && Input.GetMouseButton(0))
        {
            Vector3 forward = cameraControl.GetPivot().transform.forward;
            Vector3 right = cameraControl.GetPivot().transform.right;

            // zero out y-axis
            forward.y = 0f;
            right.y = 0f;

            Vector3 relativeForward = forward.normalized/* + right.normalized*/;
            player.animator.transform.forward = forward;

            player.movement.UpdateAnimatorSpeed();

            Vector3 movementVector = 10f * Time.deltaTime * relativeForward.normalized;
            if (!player.movement.isGrounded)
            {
                movementVector *= .5f;
            }

            player.transform.position += movementVector;
            audioMix.Transition(true, player.inCombat);
        }
        else
        {
            player.animator.SetFloat("speed", 0f);
            audioMix.Transition(false, player.inCombat);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Jump");
            player.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }


    

    //! WIN/LOSE CONDITIONS
    
    private static Dictionary<WinCondition, Window> winDictionary = new Dictionary<WinCondition, Window>();
    public static void SetWinScreens(WinScreenMapping[] mappings)
    {
        foreach(WinScreenMapping map in mappings)
        {
            if(!winDictionary.ContainsKey(map.winCondition))
            {
                winDictionary.Add(map.winCondition, map.winScreen);
            }
        }
    }

    public void PlayerDied()
    {
        //Debug.Log("Player died");
        //audioMod.PlayAudioClip(0);
        player.transform.parent = null;
        CheckForNewUnitAndVictoryCondition();
        pendingControl = true;
        player = null;
    }

    public void CheckForNewUnitAndVictoryCondition()
    {
        Debug.Log("CheckForNewUnitAndVictoryCondition: " + pendingControl);
        if (!pendingControl)
        {
            Debug.Log("Checking for new unit");
            bool winThisTime = false;
            toControl = null;

            winThisTime = CheckWinCondition(WinCondition.FirstDeath);
            //todo -- if multiple wins per turn, only one win condition should get a new unit

            // if there are more friendly units to possess
            if (factionSpawnerA.unitTransform.childCount > 0)
            {
                Debug.Log("A: " + factionSpawnerA.unitTransform.childCount);
                toControl = factionSpawnerA.unitTransform.GetChild(factionSpawnerA.unitTransform.childCount - 1).GetComponent<NPC>();
                pendingControl = true;
            }
            else
            {
                Debug.Log("No A");
                winThisTime = CheckWinCondition(WinCondition.NoMoreA);
            }

            if (toControl == null)
            {
                if (factionSpawnerB.unitTransform.childCount > 0)
                {
                    Debug.Log("B: " + factionSpawnerB.unitTransform.childCount);
                    toControl = factionSpawnerB.unitTransform.GetChild(factionSpawnerB.unitTransform.childCount - 1).GetComponent<NPC>();
                    pendingControl = true;
                }
                else
                {
                    Debug.Log("No B");
                    winThisTime = CheckWinCondition(WinCondition.NoMoreB);
                }
            }

            if (toControl == null)
            {
                if(factionSpawnerA.unitTransform.childCount <= 0 && factionSpawnerB.unitTransform.childCount <= 0)
                {
                    toControl = FindObjectOfType<NPC>();
                    if(toControl == null)
                    {
                        Debug.Log("No A or B");
                        winThisTime = CheckWinCondition(WinCondition.NoMoreAnyone);
                    }
                }
            }
            
            if(toControl != null)
            {
                
                Debug.Log("Found new unit: " + toControl.name);
            }

            if(toControl != null && winThisTime == false)
            {
                AssignPlayerNewUnit();
            }
        }
    }

    public void AssignPlayerNewUnit()
    {
        if(pendingControl)
        {
            if (toControl != null)
            {
                player = toControl.gameObject.AddComponent<Player>();
                player.faction = toControl.faction;
                cameraControl.SetModel(toControl.animator.transform);
                toControl = null;

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
            Window.windowManager.OpenWindow(winDictionary[win]);
            winDictionary.Remove(win);
            Debug.Log("Wins left:" + winDictionary.Count);
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
}

public enum WinCondition { NoMoreA, NoMoreB, NoMoreAnyone, FoundFood, FirstDeath }