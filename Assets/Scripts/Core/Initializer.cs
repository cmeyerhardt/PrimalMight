using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Initializer : MonoBehaviour
{
    [Header("Wins")]
    [SerializeField] WinScreenMapping[] winScreenMappings;

    [Header("Cursors")]
    [SerializeField] CursorMapping[] cursorMappings = null;

    [Header("Corpses")]
    [SerializeField] Transform corpseTransform = null;
    [SerializeField] CorpseContainer corpseContainer = null;

    private void Awake()
    {
        //initialize static variables with instance information
        PlayerController.AssembleCursorDictionary(cursorMappings);
        PlayerController.SetWinScreens(winScreenMappings);

        CorpseContainer.corpseTransform = corpseTransform;

        Health.InitializeCorpseContainer(corpseContainer);
        
        //free up memory
        Destroy(this);
    }
}
