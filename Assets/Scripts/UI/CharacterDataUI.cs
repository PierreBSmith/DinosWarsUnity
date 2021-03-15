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

    public void OpenCharacterUI(CharacterMovement character)
    {
        image.sprite = character.character.image;
        charName.text = character.character.characterName;
        curHP.text = character.currHP + " /";
        maxHP.text = character.character.maxHP.ToString();
        curStam.text = character.currentStamina + " /";
        maxStam.text = character.character.maxStamina.ToString();
        level.text = character.character.level.ToString();
        exp.text = character.character.curEXP.ToString();
    }
}
