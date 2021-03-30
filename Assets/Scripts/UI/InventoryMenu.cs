using System.Collections;
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
    private CharacterMovement selectedCharacter; //This is set in OpenUIMenu() because OpenUIMenu() will always be called first upon opening the menu!

    private bool selectedMode = false;
    private Item selectedItem;
    private Button firstSelectedButton;
    private Button secondSelectedButton;
    private int firstIndex = 0;
    private int secondIndex = 0;

    public bool used = false;

    void Awake()
    {
        equippedWeapon = transform.GetChild(0).gameObject.GetComponent<Button>(); //First Button will always be the weapon!
        equippedAccessory = transform.GetChild(1).gameObject.GetComponent<Button>(); //Second Button will always be the accessory!

        for(int i = 0; i < MAX_INVENTORY_SPACE; i++)
        {
            itemSlots[i] = transform.GetChild(i + 2).gameObject.GetComponent<Button>();
        }
    }

    public void OpenInventoryUIMenu(CharacterMovement character, CharacterInventory inventory)
    {
        selectedCharacter = character;
        if(inventory.equippedWeapon)
        {
            equippedWeapon.transform.GetChild(0).gameObject.GetComponent<Text>().text = inventory.equippedWeapon.itemName;
            equippedWeapon.transform.GetChild(1).gameObject.GetComponent<Text>().text = inventory.equippedWeapon.uses.ToString();
        }
        else
        {
            equippedWeapon.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
            equippedWeapon.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
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
                itemSlots[index].transform.GetChild(1).gameObject.GetComponent<Text>().text = item.uses.ToString();
            }
            else
            {
                itemSlots[index].transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
                itemSlots[index].transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
            }
            index++;
        }
        if(index < itemSlots.Length - 1)
        {
            for(int i = index; i < itemSlots.Length; i++)
            {
                itemSlots[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
                itemSlots[i].transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
            }
        }
    }

    public void CloseInventoryUI()
    {
        transform.parent.gameObject.SetActive(false);
        selectedMode = false;
        selectedItem = null;
    }

    //TODO: Item switching and using items etc etc etc!
    //The following will be the Button Stuff!
    public void OnButtonClick()
    {
        Button selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        Debug.Log(selectedButton.name);
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
                            selectedCharacter.currHP += selectedItem.amountToHeal;
                            if(selectedCharacter.currHP > selectedCharacter.character.maxHP)
                            {
                                selectedCharacter.currHP = selectedCharacter.character.maxHP;
                            }
                            selectedItem.uses--;
                            selectedMode = false; //You use the medicine and that's it really.
                            selectedItem = null;
                            used = true;
                            CloseInventoryUI();
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
                        GetUsesText(selectedButton).text = GetUsesText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = "";
                        GetUsesText(firstSelectedButton).text = "";
                    }
                    else
                    {
                        //unit is already equipping a weapon
                        Item tempItem = selectedCharacter.inventory.equippedWeapon;
                        GetButtonText(selectedButton).text = GetButtonText(firstSelectedButton).text;
                        GetUsesText(selectedButton).text = GetUsesText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = tempItem.itemName;
                        GetUsesText(firstSelectedButton).text = tempItem.uses.ToString();

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
                        GetUsesText(selectedButton).text = GetUsesText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = "";
                        GetUsesText(firstSelectedButton).text = "";
                    }
                    else
                    {
                        //unit is already equipping a weapon
                        Item tempItem = selectedCharacter.inventory.equippedAccessory;
                        GetButtonText(selectedButton).text = GetButtonText(firstSelectedButton).text;
                        GetUsesText(selectedButton).text = GetUsesText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = tempItem.itemName;
                        GetUsesText(firstSelectedButton).text = tempItem.uses.ToString();

                        selectedCharacter.inventory.inventory.Insert(selectedCharacter.inventory.inventory.IndexOf(selectedItem), tempItem);
                    }
                    selectedCharacter.inventory.equippedAccessory = selectedItem;
                    selectedCharacter.inventory.inventory.Remove(selectedItem);
                }
                //Slot to slot transferring
                else
                {
                    //This for loop gets the index of the second button
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
                        GetUsesText(selectedButton).text = GetUsesText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = "";
                        GetUsesText(firstSelectedButton).text = "";
                        selectedCharacter.inventory.inventory.RemoveAt(firstIndex);
                        selectedCharacter.inventory.inventory.Insert(secondIndex, selectedItem);
                    }
                    else
                    {
                        Item tempItem = selectedCharacter.inventory.inventory[secondIndex];
                        GetButtonText(selectedButton).text = GetButtonText(firstSelectedButton).text;
                        GetUsesText(selectedButton).text = GetUsesText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = tempItem.itemName;
                        GetUsesText(firstSelectedButton).text = tempItem.uses.ToString();

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
                    GetUsesText(firstSelectedButton).text = GetUsesText(selectedButton).text;
                    GetButtonText(selectedButton).text = "";
                    GetUsesText(selectedButton).text = "";
                    selectedCharacter.inventory.inventory.RemoveAt(firstIndex);
                    selectedCharacter.inventory.inventory.Insert(firstIndex, selectedCharacter.inventory.equippedWeapon);
                    selectedCharacter.inventory.equippedWeapon = null; //unequips weapon
                }
                else if (selectedButton == equippedAccessory)
                {
                    GetButtonText(firstSelectedButton).text = GetButtonText(selectedButton).text;
                    GetUsesText(firstSelectedButton).text = GetUsesText(selectedButton).text;
                    GetButtonText(selectedButton).text = "";
                    GetUsesText(selectedButton).text = "";
                    selectedCharacter.inventory.inventory.RemoveAt(firstIndex);
                    selectedCharacter.inventory.inventory.Insert(firstIndex, selectedCharacter.inventory.equippedAccessory);
                    selectedCharacter.inventory.equippedAccessory = null;
                }
            }
            selectedMode = false;
            selectedItem = null;
            used = true;
        }
    }

    //Get's the text component of the button cuz I am tired of typing that long thing out.
    private Text GetButtonText(Button button)
    {
        return button.transform.GetChild(0).gameObject.GetComponent<Text>();
    }
    private Text GetUsesText(Button button)
    {
        return button.transform.GetChild(1).gameObject.GetComponent<Text>();
    }
}
