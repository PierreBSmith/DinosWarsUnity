using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatForcastUI : MonoBehaviour
{
    [Header("Selected Character")]
    [SerializeField]
    private Image displayImage;
    [SerializeField]
    private Text selectedCharacterName;
    [SerializeField]
    private Text selectedCharacterWeapon;
    [SerializeField]
    private Text selectedCharaterAccessory;
    [SerializeField]
    private Text selectedCharacterHP;
    [SerializeField]
    private Text selectedCharacterMt;
    [SerializeField]
    private Text selectedCharacterHit;
    [SerializeField]
    private Text selectedCharacterCrit;

    [Header("Enemy Character")]
    [SerializeField]
    private Text enemyName;
    [SerializeField]
    private Text enemyWeapon;
    [SerializeField]
    private Text enemyAccessory;
    [SerializeField]
    private Text enemyHP;
    [SerializeField]
    private Text enemyMt;
    [SerializeField]
    private Text enemyHit;
    [SerializeField]
    private Text enemyCrit;

    public void OpenMenu(CharacterMovement selectedCharacter, CharacterMovement enemyCharacter)
    {
        displayImage.sprite = selectedCharacter.character.image;
        selectedCharacterName.text = selectedCharacter.character.characterName;
        if(selectedCharacter.inventory.equippedWeapon)
        {
            selectedCharacterWeapon.text = selectedCharacter.inventory.equippedWeapon.itemName;
        }
        else
        {
            selectedCharacterWeapon.text = "None";
        }
        if(selectedCharacter.inventory.equippedAccessory)
        {
            selectedCharaterAccessory.text = selectedCharacter.inventory.equippedAccessory.itemName;
        }
        else
        {
            selectedCharaterAccessory.text = "None";
        }

        enemyName.text = enemyCharacter.character.characterName;
        if(enemyCharacter.inventory.equippedWeapon)
        {
            enemyWeapon.text = enemyCharacter.inventory.equippedWeapon.itemName;
        }
        else
        {
            enemyWeapon.text = "None";
        }
        if(enemyCharacter.inventory.equippedAccessory)
        {
            enemyAccessory.text = enemyCharacter.inventory.equippedAccessory.itemName;
        }
        else
        {
            enemyAccessory.text = "None";
        }
    }
}
