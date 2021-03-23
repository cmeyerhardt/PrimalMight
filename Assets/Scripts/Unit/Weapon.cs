using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item, IInteractable
{
    [SerializeField] float damage;
    [SerializeField] float range;
    
    public float GetDamage() { return damage; }
    public float GetRange() { return range; }

    public override void BeDropped()
    {
        base.BeDropped();
    }

    public override CursorType GetCursorType(Character c)
    {
        return CursorType.Weapon;
    }
}
