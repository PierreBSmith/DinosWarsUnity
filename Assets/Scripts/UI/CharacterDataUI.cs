using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDataUI : MonoBehaviour
{
    public Image image;
    public Text charName;
    public Text curHP;
    public Text maxHP;
    public Text curStam;
    public Text maxStam;
    public Text level;
    public Text exp;
    public Text equippedWeapon;
    public Slider hpBar;
    public Slider staminaBar;

    public void OpenCharacterUI(CharacterMovement character)
    {
        image.sprite = character.character.image;
        charName.text = character.character.characterName;

        curHP.text = character.currHP + " /";
        maxHP.text = character.character.maxHP.ToString();
        hpBar.maxValue = character.character.maxHP;
        hpBar.value = character.currHP;

        curStam.text = character.currentStamina + " /";
        maxStam.text = character.character.maxStamina.ToString();
        staminaBar.maxValue = character.character.maxStamina;
        staminaBar.value = character.currentStamina;

        level.text = character.character.level.ToString();
        exp.text = character.character.curEXP.ToString();
        if(character.inventory.equippedWeapon)
        {
            equippedWeapon.text = character.inventory.equippedWeapon.itemName;
        }
        else
        {
            equippedWeapon.text = "None";
        }
    }
}
