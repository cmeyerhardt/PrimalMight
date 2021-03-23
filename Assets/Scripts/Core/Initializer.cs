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
    [SerializeField] Transform unitDeathTransform = null;
    ////x - dont need anymore
    //[SerializeField] CorpseMapping[] corpseMappings = null;

    [Header("Units")]
    [SerializeField] UnitSpawner factionA = null;
    [SerializeField] UnitSpawner factionB = null;

    private void Awake()
    {
        //initialize values
        PlayerController.AssembleCursorDictionary(cursorMappings);
        PlayerController.SetWinScreens(winScreenMappings);

        CorpseContainer.corpseTransform = corpseTransform;
        //CorpseContainer.InitializeCorpseDicitonary(corpseMappings);
        Health.InitializeCorpseContainer(corpseContainer);
        if(factionA == null || factionB == null)
        {
            Debug.LogWarning("UnitSpawner references are null!");
        }
        else
        {
            PlayerController.SetUnitSpawners(factionA, factionB);
        }

        Character.unitDeathTransform = unitDeathTransform;

        //free up memory
        Destroy(this);
    }
}
