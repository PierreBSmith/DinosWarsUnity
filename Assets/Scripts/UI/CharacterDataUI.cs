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

    public void OpenCharacterUI(CharacterMovement character)
    {
        image.sprite = character.character.image;
        charName.text = character.character.characterName;
        curHP.text = character.currHP + " /";
        maxHP.text = character.character.maxHP.ToString();
        curStam.text = character.currentStamina + " /";
        maxStam.text = character.character.maxStamina.ToString();
    }
}
