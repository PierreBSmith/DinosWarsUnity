using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    //THIS IS A CLASS THAT CONTAINS ALGORITHMS ONLY.
    //EVERYTHING HERE IS TO BE CALLED IN RULESENGINE!!!!!
    //Thus all methods should be public (unless it's a helper function within Combat)

    private const int MAX_EXP_TO_LEVEL = 100;
    private const int BASE_EXP_GAIN = 20; //For now, we will tweak upon play test
    private const int EXTRA_FOR_KILL = 30;
    private const int NUM_OF_STATS = 7;//There are 7 stats to calculate if they go up :D

    public void GainEXP(CharacterMovement playerUnit, CharacterMovement enemyUnit, int damageDealt, bool killedEnemy)
    {
        if(!killedEnemy) //If the enemy did not die
        {
            if(damageDealt == 0)
            {
                playerUnit.character.curEXP++; //Ya only get 1 EXP if ya don't do anything LUL
            }
            else
            {
                playerUnit.character.curEXP += EXPFromDamageDealt(playerUnit, enemyUnit);
            }
        }
        else
        {
            playerUnit.character.curEXP += EXPFromDamageDealt(playerUnit, enemyUnit) + EXTRA_FOR_KILL;
            //with probably a lot more stuff. Like enemy level, promotions, and other stuff. Probably like debuffs or something?
        }
        LevelUp(playerUnit);
    }

    private int EXPFromDamageDealt(CharacterMovement playerUnit, CharacterMovement enemyUnit)
    {
        return BASE_EXP_GAIN + (enemyUnit.character.level) - (playerUnit.character.level);
        //We changing this probably to balance it out, but I need classes for this. So for now, we have this.
    }

    private void LevelUp(CharacterMovement playerUnit)
    {
        if (playerUnit.character.curEXP >= MAX_EXP_TO_LEVEL)
        {
            //LEVEL UP! With growths and stuff, but for now, just uhhhhh level goes up.
            playerUnit.character.level++;
            playerUnit.character.curEXP -= MAX_EXP_TO_LEVEL;
            for(int i = 0; i < NUM_OF_STATS; i++)
            {
                StatGrowth(playerUnit, i); //Random numbers for every single stat!
            }
        }
    }

    private void StatGrowth(CharacterMovement playerUnit, int index)
    {
        int growthFactor = Random.Range(0, 101); //Random Number 0 - 100
        switch(index)
        {
            case 0: //HP
                if(growthFactor <= playerUnit.character.hpGrowth)
                {
                    playerUnit.character.maxHP++;
                    Debug.Log("HP UP");
                }
                break;
            case 1: //Strength
                if(growthFactor <= playerUnit.character.strGrowth)
                {
                    playerUnit.character.str++;
                    Debug.Log("STR UP");
                }
                break;
            case 2: //Skill
                if(growthFactor <= playerUnit.character.skllGrowth)
                {
                    playerUnit.character.skll++;
                    Debug.Log("SKL UP");
                }
                break;
            case 3: //Speed
                if(growthFactor <= playerUnit.character.spdGrowth)
                {
                    playerUnit.character.spd++;
                    Debug.Log("SPD UP");
                }
                break;
            case 4: //Luck
                if(growthFactor <= playerUnit.character.lckGrowth)
                {
                    playerUnit.character.lck++;
                    Debug.Log("LCK UP");
                }
                break;
            case 5: //Defense
                if(growthFactor <= playerUnit.character.defGrowth)
                {
                    playerUnit.character.def++;
                    Debug.Log("DEF UP");
                }
                break;
            case 6: //Resistance
                if(growthFactor <= playerUnit.character.resGrowth)
                {
                    playerUnit.character.res++;
                    Debug.Log("RES UP");
                }
                break;
        }
    }
}
