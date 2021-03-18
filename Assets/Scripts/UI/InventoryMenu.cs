﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryMenu : MonoBehaviour
{
    private const int MAX_INVENTORY_SPACE = 5;
    private Button equippedWeapon;
    private Button equippedAccessory;
    private Button[] itemSlots = new Button[MAX_INVENTORY_SPACE];
    private CharacterMovement selectedCharacter; //This is set in UpdateUIMenu() because UpdateUIMenu() will always be called first upon opening the menu!

    private bool selectedMode = false;
    private Item selectedItem;
    private Button firstSelectedButton;
    private Button secondSelectedButton;
    private int firstIndex = 0;
    private int secondIndex = 0;

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
            if(Input.GetMouseButtonDown(1))//Right click! Closes UI menu
            {
                transform.parent.gameObject.SetActive(false);
                selectedMode = false;
                selectedItem = null;
            }

        }
    }

    public void OpenInventoryUIMenu(CharacterMovement character, CharacterInventory inventory)
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
            if(item)
            {
                itemSlots[index].transform.GetChild(0).gameObject.GetComponent<Text>().text = item.itemName;
            }
            else
            {
                itemSlots[index].transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
            }
            index++;
        }
    }

    private void UpdateUIMenu()
    {
        if(selectedCharacter.inventory.equippedWeapon)
        {
            equippedWeapon.transform.GetChild(0).gameObject.GetComponent<Text>().text = selectedCharacter.inventory.equippedWeapon.itemName;
        }
        else
        {
            equippedWeapon.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
        }

        if(selectedCharacter.inventory.equippedAccessory)
        {
            equippedAccessory.transform.GetChild(0).gameObject.GetComponent<Text>().text = selectedCharacter.inventory.equippedAccessory.itemName;
        }
        else
        {
            equippedAccessory.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
        }

        int index = 0;
        foreach(Item item in selectedCharacter.inventory.inventory)
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
    //The following will be the Button Stuff!
    public void OnButtonClick()
    {
        Button selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if(!selectedMode) //If nothing was selected before hand
        {
            selectedMode = true;
            firstSelectedButton = selectedButton;
            if(selectedButton == equippedWeapon)
            {
                if(selectedCharacter.inventory.equippedWeapon) //Checks if the unit has a weapon
                {
                    selectedItem = selectedCharacter.inventory.equippedWeapon;
                    Debug.Log(selectedItem.itemName);
                }
            }
            else if(selectedButton == equippedAccessory)
            {
                if(selectedCharacter.inventory.equippedAccessory)
                {
                    selectedItem = selectedCharacter.inventory.equippedAccessory;
                }
            }
            else
            {
                for(int i = 0; i < itemSlots.Length; i++)
                {
                    if(firstSelectedButton == itemSlots[i])
                    {
                        firstIndex = i;
                    }
                }
                string selectedItemName = selectedButton.transform.GetChild(0).gameObject.GetComponent<Text>().text;
                if(selectedItemName != "") //If there is an item in that slot, find item from inventory
                {
                    selectedItem = selectedCharacter.inventory.inventory[firstIndex];
                    Debug.Log("Found item: " + selectedItem.itemName);
                    if(selectedItem.type == Item.TYPE.CONSUMABLE)
                    {
                        if(selectedItem.consumableType == Item.CONSUMABLE.MEDICINE)
                        {
                            //TODO: Medicine effect
                            Debug.Log("Using medicine");
                            selectedItem.uses--;
                            Debug.Log(selectedItem.uses); //TODO: Edit the number of uses.
                            selectedMode = false; //You use the medicine and that's it really.
                            selectedItem = null;
                            //TODO: check if uses go to 0, cuz then the item should disappear from inventory and actually just from the game file LOL.
                        }
                    }
                }
            } 
        }
        else
        {
            if(selectedItem)
            {
                if(selectedButton == equippedWeapon && selectedItem.type == Item.TYPE.WEAPON)
                {
                    //we assuming that the unit clicked on a weapon. It has to be a weapon otherwise you can't equip it!
                    if(GetButtonText(selectedButton).text == "" && !selectedCharacter.inventory.equippedWeapon)
                    {
                        //If the unit wasn't equipping a weapon
                        GetButtonText(selectedButton).text = GetButtonText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = "";
                        selectedCharacter.inventory.equippedWeapon = selectedItem;
                    }
                    else
                    {
                        //unit is already equipping a weapon
                        Item tempItem = selectedCharacter.inventory.equippedWeapon;
                        //selectedCharacter.inventory.equippedWeapon = selectedItem; //swap out weapon
                        GetButtonText(selectedButton).text = GetButtonText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = tempItem.itemName;
                        Debug.Log(GetButtonText(firstSelectedButton).text);
                        selectedCharacter.inventory.inventory.Insert(selectedCharacter.inventory.inventory.IndexOf(selectedItem), tempItem);
                    }
                    selectedCharacter.inventory.equippedWeapon = selectedItem;
                    selectedCharacter.inventory.inventory.Remove(selectedItem);
                }
                else if(selectedButton == equippedAccessory && selectedItem.type == Item.TYPE.ACCESORY)
                {
                    //we assuming that the unit clicked on a weapon. It has to be a weapon otherwise you can't equip it!
                    if(GetButtonText(selectedButton).text == "")
                    {
                        //If the unit wasn't equipping a weapon
                        GetButtonText(selectedButton).text = GetButtonText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = "";
                    }
                    else
                    {
                        //unit is already equipping a weapon
                        Item tempItem = selectedCharacter.inventory.equippedAccessory;
                        //selectedCharacter.inventory.equippedWeapon = selectedItem; //swap out weapon
                        GetButtonText(selectedButton).text = GetButtonText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = tempItem.itemName;
                        Debug.Log(GetButtonText(firstSelectedButton).text);
                        selectedCharacter.inventory.inventory.Insert(selectedCharacter.inventory.inventory.IndexOf(selectedItem), tempItem);
                    }
                    selectedCharacter.inventory.equippedAccessory = selectedItem;
                    selectedCharacter.inventory.inventory.Remove(selectedItem);
                }
                //Slot to slot transferring
                else
                {
                    for(int i = 0; i < itemSlots.Length; i++)
                    {
                        if(selectedButton == itemSlots[i])
                        {
                            secondIndex = i;
                        }
                    }
                    if(GetButtonText(selectedButton).text == "")
                    {
                        //If nothing in slot
                        GetButtonText(selectedButton).text = GetButtonText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = "";
                        selectedCharacter.inventory.inventory.RemoveAt(firstIndex);
                        selectedCharacter.inventory.inventory.Insert(secondIndex, selectedItem);
                    }
                    else
                    {
                        Item tempItem = selectedCharacter.inventory.inventory[secondIndex];
                        GetButtonText(selectedButton).text = GetButtonText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = tempItem.itemName;
                        selectedCharacter.inventory.inventory.RemoveAt(firstIndex);
                        selectedCharacter.inventory.inventory.Insert(firstIndex, tempItem); //This replaces the first selected slot with the second
                        selectedCharacter.inventory.inventory.RemoveAt(secondIndex); 
                        selectedCharacter.inventory.inventory.Insert(secondIndex, selectedItem); //This replaces the second selected slot with the fist
                    }
                }
            }
            else
            {
                //If empty slot was initially selected
                if(selectedButton == equippedWeapon)
                {
                    //Move weapon into inventory
                    GetButtonText(firstSelectedButton).text = GetButtonText(selectedButton).text;
                    GetButtonText(selectedButton).text = "";
                    selectedCharacter.inventory.inventory.RemoveAt(firstIndex);
                    selectedCharacter.inventory.inventory.Insert(firstIndex, selectedCharacter.inventory.equippedWeapon);
                    selectedCharacter.inventory.equippedWeapon = null; //unequips weapon
                }
                else if (selectedButton == equippedAccessory)
                {
                    GetButtonText(firstSelectedButton).text = GetButtonText(selectedButton).text;
                    GetButtonText(selectedButton).text = "";
                    selectedCharacter.inventory.inventory.RemoveAt(firstIndex);
                    selectedCharacter.inventory.inventory.Insert(firstIndex, selectedCharacter.inventory.equippedAccessory);
                    selectedCharacter.inventory.equippedAccessory = null;
                }
            }
            selectedMode = false;
            selectedItem = null;
        }
    }

    //Get's the text component of the button cuz I am tired of typing that long thing out.
    private Text GetButtonText(Button button)
    {
        return button.transform.GetChild(0).gameObject.GetComponent<Text>();
    }
}
