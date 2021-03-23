using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour, IInteractable
{
    [SerializeField] public Item item = null;
    [SerializeField] bool destroyOnInteract = true;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public CursorType GetCursorType(Character c)
    {
        if(item is Weapon)
        {
            return CursorType.Weapon;
        }
        else if(item is Food)
        {
            return CursorType.Food;
        }
        return CursorType.PickUp;
    }

    public void Interact(Character c)
    {
        if (c != null && c.IsInRange(transform.position))
        {
            c.inventory.TakeItem(Instantiate(item));
            if (destroyOnInteract)
            {
                Destroy(gameObject);
            }
        }
    }
}
