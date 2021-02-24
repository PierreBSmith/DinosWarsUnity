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
    private CharacterMovement selectedCharacter; //This is set in UpdateUIMenu() because UpdateUIMenu() will always be called first upon opening the menu!

    private bool selectedMode = false;
    private Item selectedItem;
    private Button firstSelectedButton;
    private Button secondSelectedButton;

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
                selectedCharacter.canvas.gameObject.SetActive(true);
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
                string selectedItemName = selectedButton.transform.GetChild(0).gameObject.GetComponent<Text>().text;
                if(selectedItemName != "") //If there is an item in that slot, find item from inventory
                {
                    selectedItem = selectedCharacter.inventory.inventory.Find(x => x.itemName == selectedItemName);
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
                        }
                    }
                }
            } 
        }
        else
        {
            Debug.Log("Entering Swap Mode :D");
            if(selectedItem)
            {
                if(selectedButton == equippedWeapon && selectedItem.type == Item.TYPE.WEAPON)
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
                        Item tempWeapon = selectedCharacter.inventory.equippedWeapon;
                        //selectedCharacter.inventory.equippedWeapon = selectedItem; //swap out weapon
                        GetButtonText(selectedButton).text = GetButtonText(firstSelectedButton).text;
                        GetButtonText(firstSelectedButton).text = tempWeapon.itemName;
                        Debug.Log(GetButtonText(firstSelectedButton).text);
                        //Time for MONKA list manipulation
                        selectedCharacter.inventory.inventory.Insert(selectedCharacter.inventory.inventory.IndexOf(selectedItem), tempWeapon);
                        //selectedCharacter.inventory.inventory.Remove(selectedItem);
                    }
                    selectedCharacter.inventory.equippedWeapon = selectedItem;
                    selectedCharacter.inventory.inventory.Remove(selectedItem);
                }
            }

        }
    }

    //Get's the text component of the button cuz I am tired of typing that long thing out.
    private Text GetButtonText(Button button)
    {
        return button.transform.GetChild(0).gameObject.GetComponent<Text>();
    }
}
