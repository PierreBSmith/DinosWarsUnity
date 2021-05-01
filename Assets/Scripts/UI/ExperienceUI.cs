using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceUI : MonoBehaviour
{
    public Slider experienceBar;
    public Text experienceNum;
    public Image characterImage;

    public void OpenExperienceUI(CharacterMovement character, int startEXP)
    {
        characterImage.sprite = character.character.image;
        experienceBar.value = startEXP;
        experienceNum.text = startEXP.ToString();
    }
}
