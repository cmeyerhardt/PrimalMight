using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item, IInteractable
{
    [SerializeField] int damage;
    [SerializeField] int range;
    
    public int GetDamage() { return damage; }
    public int GetRange() { return range; }

    public override CursorType GetCursorType(Character c)
    {
        return CursorType.Weapon;
    }
}
