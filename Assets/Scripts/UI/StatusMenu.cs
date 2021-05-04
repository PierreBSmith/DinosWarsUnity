using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusMenu : MonoBehaviour
{
    public Image characterImage;

    [Header("Character Information")]
    public Text characterName;
    public Slider HP_Bar;
    public Text currentHP;
    public Text maxHP;
    public Slider Stamina_Bar;
    public Text currentStamina;
    public Text maxStamina;
    public Text levelNum;

    [Header("Stat Information")]
    public Text STR_Stat;
    public Text SKL_Stat;
    public Text SPD_Stat;
    public Text LCK_Stat;
    public Text DEF_Stat;
    public Text RES_Stat;

    [Header("Inventory Information")]
    public Text equippedWeaponName;
    public Text equippedWeaponUses;
    public Text equippedAccessoryName;
    public GameObject inventory;
    private List<GameObject> inventorySlots = new List<GameObject>();
    private const int MAX_INVENTORY_SPACE = 5;

    public void OpenStatusMenu(CharacterMovement character)
    {
        GetInventorySlots();
        characterImage.sprite = character.character.image;
        characterName.text = character.character.characterName;

        HP_Bar.maxValue = character.character.maxHP;
        HP_Bar.value = character.currHP;
        currentHP.text = character.currHP.ToString();
        maxHP.text = "/" + character.character.maxHP.ToString();

        Stamina_Bar.maxValue = character.character.maxStamina;
        Stamina_Bar.value = character.currentStamina;
        currentStamina.text = character.currentStamina.ToString();
        maxStamina.text = "/" + character.character.maxStamina.ToString();

        levelNum.text = character.character.level.ToString();


        STR_Stat.text = character.character.str.ToString();
        SKL_Stat.text = character.character.skll.ToString();
        SPD_Stat.text = character.character.spd.ToString();
        LCK_Stat.text = character.character.lck.ToString();
        DEF_Stat.text = character.character.def.ToString();
        RES_Stat.text = character.character.res.ToString();

        if(character.inventory.equippedWeapon)
        {
            equippedWeaponName.text = character.inventory.equippedWeapon.itemName;
            equippedWeaponUses.text = character.inventory.equippedWeapon.uses.ToString();
        }
        else
        {
            equippedWeaponName.text = "";
            equippedWeaponUses.text = "";
        }

        if(character.inventory.equippedAccessory)
        {
            equippedAccessoryName.text = character.inventory.equippedAccessory.itemName;
        }
        else
        {
            equippedAccessoryName.text = "";
        }

        for(int i = 0; i < MAX_INVENTORY_SPACE; i++)
        {
            if(character.inventory.inventory[i])
            {
                inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = character.inventory.inventory[i].itemName;
                inventorySlots[i].transform.GetChild(1).GetComponent<Text>().text = character.inventory.inventory[i].uses.ToString();
            }
            else
            {
                inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = "";
                inventorySlots[i].transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }

    }

    private void GetInventorySlots()
    {
        foreach(Transform child in inventory.transform)
        {
            inventorySlots.Add(child.GetChild(1).gameObject);
        }
    }
}
