using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealUI : MonoBehaviour
{
    [SerializeField]
    private Text targetName;
    [SerializeField]
    private Text targetCurrentHP;
    [SerializeField]
    private Text[] targetMaxHP;
    [SerializeField]
    private Text healedHP;

    public void OpenHealUI(CharacterMovement user, CharacterMovement target)
    {
        targetName.text = target.character.characterName;
        targetCurrentHP.text = target.currHP.ToString();
        int heal = target.currHP + (int)(user.character.str * 1.5f);
        if(heal > target.character.maxHP)
        {
            heal = target.character.maxHP;
        }
        healedHP.text = heal.ToString();
        foreach (Text targetMax in targetMaxHP)
        {
            targetMax.text = "/ " + target.character.maxHP.ToString();
        }
    }
}
