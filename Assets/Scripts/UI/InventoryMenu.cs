using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    private const int MAX_INVENTORY_SPACE = 5;
    private Button equippedWeapon;
    private Button equippedAccessory;
    private Button[] itemSlots = new Button[MAX_INVENTORY_SPACE];
    private CharacterMovement selectedCharacter;

    void Awake()
    {
        equippedWeapon = transform.GetChild(0).gameObject.GetComponent<Button>(); //First Button will always be the weapon!
        equippedAccessory = transform.GetChild(1).gameObject.GetComponent<Button>(); //Second Button will always be the accessory!

        for(int i = 0; i < MAX_INVENTORY_SPACE; i++)
        {
            itemSlots[i] = transform.GetChild(i + 2).gameObject.GetComponent<Button>();
        }
    }

    void Update()
    {
        if(gameObject.activeSelf)
        {
            if(Input.GetMouseButtonDown(1))//Right click!
            {
                transform.parent.gameObject.SetActive(false);
                selectedCharacter.canvas.gameObject.SetActive(true);
            }
        }
    }

    public void UpdateUIMenu(CharacterMovement character, CharacterInventory inventory)
    {
        selectedCharacter = character;
        if(inventory.equippedWeapon)
        {
            equippedWeapon.transform.GetChild(0).gameObject.GetComponent<Text>().text = inventory.equippedWeapon.itemName;
        }
        else
        {
            equippedWeapon.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
        }

        if(inventory.equippedAccessory)
        {
            equippedAccessory.transform.GetChild(0).gameObject.GetComponent<Text>().text = inventory.equippedAccessory.itemName;
        }
        else
        {
            equippedAccessory.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
        }

        int index = 0;
        foreach(Item item in inventory.inventory)
        {
            itemSlots[index].transform.GetChild(0).gameObject.GetComponent<Text>().text = item.itemName;
            index++;
        }
        if(index < MAX_INVENTORY_SPACE)//This makes the rest of the inventory spaces blank.
        {
            for(int i = index; i < MAX_INVENTORY_SPACE; i++)
            {
                itemSlots[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
            }
        }
    }

    //TODO: Item switching and using items etc etc etc!
}
