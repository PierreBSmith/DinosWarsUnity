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
    private GameObject selectedCharacterx2Text;
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
    private GameObject enemyx2Text;
    [SerializeField]
    private Text enemyHit;
    [SerializeField]
    private Text enemyCrit;

    public void OpenMenu(CharacterMovement selectedCharacter, CharacterMovement enemyCharacter, int selectedMt, bool selectedDouble,
        int selectedHit, int selectedCrit, int enemyMight, bool enemyDouble, int enemyHt, int enemyCritical)
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
        selectedCharacterHP.text = selectedCharacter.currHP.ToString();
        if(selectedCharacter.inventory.equippedWeapon)
        {
            selectedCharacterMt.text = selectedMt.ToString();
            if(selectedMt < 0)
            {
                selectedCharacterMt.text = "0";
            }
            selectedCharacterMt.text = selectedMt.ToString();
            if(selectedDouble)
            {
                selectedCharacterx2Text.SetActive(true);
            }
            else
            {
                selectedCharacterx2Text.SetActive(false);
            }
            selectedCharacterHit.text = selectedHit.ToString();
            if(selectedHit < 0)
            {
                selectedCharacterHit.text = "0";
            }
            selectedCharacterCrit.text = selectedCrit.ToString();
            if(selectedCrit < 0)
            {
                selectedCharacterCrit.text = "0";
            }
        }
        else
        {
            selectedCharacterMt.text = "-";
            selectedCharacterx2Text.SetActive(false);
            selectedCharacterHit.text = "-";
            selectedCharacterCrit.text = "-";
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
        enemyHP.text = enemyCharacter.currHP.ToString();
        if (!enemyCharacter.inventory.equippedWeapon 
            || enemyCharacter.inventory.equippedWeapon.maxRange < selectedCharacter.inventory.equippedWeapon.minRange
            || enemyCharacter.inventory.equippedWeapon.minRange > selectedCharacter.inventory.equippedWeapon.maxRange)
        {
            enemyMt.text = "-";
            enemyx2Text.SetActive(false);
            enemyHit.text = "-";
            enemyCrit.text = "-";
        }
        else if(enemyCharacter.inventory.equippedWeapon) 
        {
            enemyMt.text = enemyMight.ToString();
            if(enemyMight < 0)
            {
                enemyMt.text = "0";
            }
            if(enemyDouble)
            {
                enemyx2Text.SetActive(true);
            }
            else
            {
                enemyx2Text.SetActive(false);
            }
            enemyHit.text = enemyHt.ToString();
            if(enemyHt < 0)
            {
                enemyHit.text = "0";
            }
            enemyCrit.text = enemyCritical.ToString();
            if(enemyCritical < 0)
            {
                enemyCrit.text = "0";
            }
        }
    }
}
