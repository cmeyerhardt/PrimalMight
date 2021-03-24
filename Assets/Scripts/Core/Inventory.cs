using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Weapon heldWeapon = null;
    [SerializeField] Food heldFood = null;
    [SerializeField] Transform heldItemTransform;
    [SerializeField] Transform weaponTransform;
    
    public void ConsumeFood()
    {
        if(CheckFood())
        {
            Item i = heldFood;
            heldFood = null;
            Destroy(i.gameObject);
        }
    }

    public float GetRange()
    {
        if(heldWeapon != null)
        {
            return heldWeapon.GetRange();
        }
        //if(HasItem())
        //{
        //    Weapon w = heldItem as Weapon;
        //    if(w != null)
        //    {
        //        return w.GetRange();
        //    }
        //}
        return 3f;
    }

    public bool HasFood()
    {
        return heldFood != null;
        //if(HasItem())
        //{
        //    if (heldFood is Food)
        //    {
        //        return true;
        //    }
        //}
        //return false;
    }


    public bool HasWeapon()
    {
        return heldWeapon != null;

        //if (HasItem())
        //{
        //    if (heldItem is Weapon)
        //    {
        //        return true;
        //    }
        //}
        //return false;
    }

    public Weapon CheckWeapon()
    {
        return heldWeapon;
    }

    public Food CheckFood()
    {
        return heldFood;
    }

    public void DropItem(string type = "")
    {
        //Debug.Log("Dropping item: " + type);
        switch(type)
        {
            case "food":
                if (heldFood != null)
                {
                    heldFood.BeDropped();
                    heldFood = null;
                }
                break;
            case "weapon":
                if (heldWeapon != null)
                {
                    heldWeapon.BeDropped();
                    heldWeapon = null;
                }
                break;
            case "all":
                DropItems();
                break;
            default:
                if (heldFood != null)
                {
                    heldFood.BeDropped();
                    heldFood = null;
                }
                else if (heldWeapon != null)
                {
                    heldWeapon.BeDropped();
                    heldWeapon = null;
                }
                break;
        }
    }

    public void DropItems()
    {
        if (heldFood != null)
        {
            heldFood.BeDropped();
            heldFood = null;
        }

        if (heldWeapon != null)
        {
            heldWeapon.BeDropped();
            heldWeapon = null;
        }
    }

    public void TakeItem(Item item)
    {
        if(item != null)
        {
            Food f = item as Food;
            if (f != null)
            {
                if (heldFood != null)
                {
                    DropItem("food");
                }

                item.BePickedUp();
                heldFood = f;
                item.transform.position = heldItemTransform.position;
                item.transform.parent = heldItemTransform;
                item.transform.rotation = heldItemTransform.rotation;
            }
            else
            {
                Weapon w = item as Weapon;
                if (w != null)
                {
                    if (heldWeapon != null)
                    {
                        DropItem("weapon");
                    }

                    item.BePickedUp();
                    heldWeapon = w;
                    item.transform.position = weaponTransform.position;
                    item.transform.parent = weaponTransform;
                    item.transform.rotation = weaponTransform.rotation;
                }
            }
        }
        else
        {
            Debug.Log("Item is null");
        }
        
        //heldFood = item as Food;
        //if(heldFood != null)
        //{
        //    heldFood.BePickedUp();

        //    Weapon w = item as Weapon;
        //    if (w != null)
        //    {
        //        heldWeapon.transform.position = weaponTransform.position;
        //        heldFood.transform.parent = weaponTransform;
        //        //heldItem.transform.forward = weaponTransform.forward;
        //    }
        //    else
        //    {
        //        heldFood.transform.position = heldItemTransform.position;
        //        heldFood.transform.parent = heldItemTransform;
        //        //heldItem.transform.forward = heldItemTransform.forward;
        //    }
        //}



        //Debug.Log(name + "taking item: " + item);
        
    }
}
