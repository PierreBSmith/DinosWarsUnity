using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    private GameManager gameManager; //We need this to access the entire Game memory!

    private const int MAX_INVENTORY_SPACE = 5;

    [SerializeField]
    private Item startingWeapon;
    [SerializeField]
    private Item startingAccessory;
    [SerializeField]
    private List<Item> startingInventory = new List<Item>(MAX_INVENTORY_SPACE);

    [HideInInspector]
    public List<Item> inventory = new List<Item>(MAX_INVENTORY_SPACE);
    [HideInInspector]
    public Item equippedWeapon;
    [HideInInspector]
    public Item equippedAccessory;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //We should have a flag in gameManager or something that says THIS IS THE START OF THE GAME so we can give them a starting weapon, otherwise, just give them to weapon they already have.
        if(startingWeapon)
        {
            equippedWeapon = (Item)Instantiate(gameManager.allItems.Find(x => x == startingWeapon));
        }
        else
        {
            equippedWeapon = null;
        }
        if(startingAccessory)
        {
            equippedAccessory = (Item)Instantiate(gameManager.allItems.Find(x => x == startingAccessory));
        }
        else
        {
            equippedAccessory = null;
        }
        
        int index = 0;
        if(startingInventory.Count > 0)
        {
            foreach(Item item in startingInventory)
            {
                inventory.Add((Item)Instantiate(gameManager.allItems.Find(x => x == item)));
                index++;
            }
        }
        if(index < MAX_INVENTORY_SPACE)
        {
            while(index < MAX_INVENTORY_SPACE)
            {
                inventory.Add(null); //place holders
                index++;
            }
        }

    }

    public void EquipWeapon(Item weapon)
    {
        equippedWeapon = weapon;
    }

    //Do we need to distinguish??? Idk.....
    public void EquipWeaponFromInventory(Item weapon)
    {
        equippedWeapon = weapon;
        inventory.Remove(weapon);
    }

    public void UnequipWeapon()
    {
        inventory.Add(equippedWeapon);
        equippedWeapon = null;
    }

    public void EquippAccessory(Item accessory)
    {
        equippedAccessory = accessory;
    }

    public void EquipAccessoryFromInventory(Item accessory)
    {
        equippedAccessory = accessory;
        inventory.Remove(accessory);
    }

    public void UnequipAccessory()
    {
        inventory.Add(equippedAccessory);
        equippedAccessory = null;
    }

    public void AddItemToInventory(Item item)
    {
        if(inventory.Count < MAX_INVENTORY_SPACE)
        {
            inventory.Add(item);
        }
        else
        {
            //TODO: Give prompt to either throw away item or remove an item from inventory
        }
    }
}
