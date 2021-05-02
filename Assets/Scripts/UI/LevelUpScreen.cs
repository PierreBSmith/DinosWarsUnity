using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpScreen : MonoBehaviour
{
    public Image characterImage;
    [HideInInspector]
    public List<GameObject> _statLine = new List<GameObject>();

    private const int NUM_OF_STATS = 7;//There are 7 stats to calculate if they go up :D

    public void OpenLevelUpScreen(CharacterMovement character)
    {
        characterImage.sprite = character.character.image;
        Transform stats = transform.Find("Stat_Line");
        foreach(Transform child in stats)
        {
            _statLine.Add(child.gameObject);
        }
        for(int i = 0; i < NUM_OF_STATS; i++)
        {
            SetStatText(character, i);
        }
    }
    
    private void SetStatText(CharacterMovement playerUnit, int statIndex)
    {
        _statLine[statIndex].transform.GetChild(2).gameObject.SetActive(false);
        switch(statIndex)
        {
            case 0: //HP
                _statLine[statIndex].transform.GetChild(1).GetComponent<Text>().text = playerUnit.character.maxHP.ToString();
                break;
            case 1: //Strength
                _statLine[statIndex].transform.GetChild(1).gameObject.GetComponent<Text>().text = playerUnit.character.str.ToString();
                break;
            case 2: //Skill
                _statLine[statIndex].transform.GetChild(1).gameObject.GetComponent<Text>().text = playerUnit.character.skll.ToString();
                break;
            case 3: //Speed
                _statLine[statIndex].transform.GetChild(1).gameObject.GetComponent<Text>().text = playerUnit.character.spd.ToString();
                break;
            case 4: //Luck
                _statLine[statIndex].transform.GetChild(1).gameObject.GetComponent<Text>().text = playerUnit.character.lck.ToString();
                break;
            case 5: //Defense
                _statLine[statIndex].transform.GetChild(1).gameObject.GetComponent<Text>().text = playerUnit.character.def.ToString();
                break;
            case 6: //Resistance
                _statLine[statIndex].transform.GetChild(1).gameObject.GetComponent<Text>().text = playerUnit.character.res.ToString();
                break;
        }
    }
}
