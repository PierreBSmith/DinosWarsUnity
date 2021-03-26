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
    private const int BASE_EXP_HEAL = 10;
    private const int EXTRA_FOR_KILL = 30;
    private const int NUM_OF_STATS = 7;//There are 7 stats to calculate if they go up :D
    private const int MINIMUM_STAMINA = 0;

    private const int DOUBLING_MAGIC_NUMBER = 5;
    private const int WEAPON_ADVANTAGE = 10;
    private const int WEAPON_DAMAGE_ADVANTAGE = 1;
    private const int WEAPON_DAMAGE_DISADVANTAGE = 1;
    private const int WEAPON_DISADVANTAGE = 10;

    private const int LOW_STAMINA_PENALTY = 10;

    //We still need to implement critical percentages.
    public bool CombatExchange(CharacterMovement playerUnit, CharacterMovement enemyUnit)
    {
        if(playerUnit.inventory.equippedWeapon.weaponType == Item.WEAPON.SPIRIT)
        {
            enemyUnit.currHP -= playerUnit.inventory.equippedWeapon.might;
            playerUnit.currentStamina -= playerUnit.character.attackStaminaCost;
            if(enemyUnit.currHP > enemyUnit.character.maxHP)
            {
                enemyUnit.currHP = enemyUnit.character.maxHP;
            }
            playerUnit.inventory.equippedWeapon.uses--;
        }
        else
        {
            int playerAccuracy = GetAccuracy(playerUnit, enemyUnit);
            int hitChance = Random.Range(0, 101); //Random number in range 0 - 100
            playerUnit.currentStamina -= playerUnit.character.attackStaminaCost;
            if (hitChance <= playerAccuracy)
            {
                //Hit enemy
                int critChance = Random.Range(0,101);
                if(critChance <= GetCritChance(playerUnit, enemyUnit))
                {
                    enemyUnit.currHP -= GetDamageDealt(playerUnit, enemyUnit) * 3;
                }
                else
                {
                    enemyUnit.currHP -= GetDamageDealt(playerUnit, enemyUnit);
                }
                playerUnit.inventory.equippedWeapon.uses--;
            }//Check if enemy dead
            if(enemyUnit.currHP <= 0)
            {
                return true;
            }

            //Enemy phase of attack
            hitChance = Random.Range(0, 101); //new Random number!
            int enemyAccuracy = GetAccuracy(enemyUnit, playerUnit);
            enemyUnit.currentStamina -= enemyUnit.character.attackStaminaCost;
            if(hitChance <= enemyAccuracy)
            {
                //Get hit
                int critChance = Random.Range(0,101);
                if(critChance <= GetCritChance(enemyUnit, playerUnit))
                {
                    playerUnit.currHP -= GetDamageDealt(enemyUnit, playerUnit) * 3;
                }
                else
                {
                    playerUnit.currHP -= GetDamageDealt(enemyUnit, playerUnit);
                }
                enemyUnit.inventory.equippedWeapon.uses--;
            } //Check if player dead
            if(playerUnit.currHP <= 0)
            {
                return true;
            }

            //Second player hit
            if(GetAttackSpeed(playerUnit) - GetAttackSpeed(enemyUnit) >= DOUBLING_MAGIC_NUMBER)
            {
                //Double yay!
                hitChance = Random.Range(0, 101); //Another new Random;
                if (hitChance <= playerAccuracy)
                {
                    //Hit enemy
                    int critChance = Random.Range(0,101);
                    if(critChance <= GetCritChance(playerUnit, enemyUnit))
                    {
                        enemyUnit.currHP -= GetDamageDealt(playerUnit, enemyUnit) * 3;
                    }
                    else
                    {
                        enemyUnit.currHP -= GetDamageDealt(playerUnit, enemyUnit);
                    }
                    playerUnit.inventory.equippedWeapon.uses--;
                }//Check if enemy dead
                if(enemyUnit.currHP <= 0)
                {
                    return true;
                }
            }
            else if (GetAttackSpeed(enemyUnit) - GetAttackSpeed(playerUnit) >= DOUBLING_MAGIC_NUMBER)
            {
                //Enemy double fricking sadge :(
                hitChance = Random.Range(0, 101); //Another new Random;
                if (hitChance <= enemyAccuracy)
                {
                    //Hit enemy
                    int critChance = Random.Range(0,101);
                    if(critChance <= GetCritChance(enemyUnit, playerUnit))
                    {
                        playerUnit.currHP -= GetDamageDealt(enemyUnit, playerUnit) * 3;
                    }
                    else
                    {
                        playerUnit.currHP -= GetDamageDealt(enemyUnit, playerUnit);
                    }
                    enemyUnit.inventory.equippedWeapon.uses--;
                }
                //Check if enemy dead
                if(playerUnit.currHP <= 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
    //Probably want an IEnumerator for attack pausa and animations and stoof

    public int GetAttackSpeed(CharacterMovement unit)
    {
        int burden = unit.inventory.equippedWeapon.weight - unit.character.str;
        if(burden < 0)
        {
            burden = 0;
        }
        return unit.character.spd - burden;
    }

    public int GetAccuracy(CharacterMovement playerUnit, CharacterMovement enemyUnit)
    {
        int accuracy = GetHitRate(playerUnit) - GetAvoid(enemyUnit);
        switch(playerUnit.inventory.equippedWeapon.weaponType)
        {
            case Item.WEAPON.CLUB:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.SPEAR:
                        accuracy += WEAPON_ADVANTAGE;
                        break;
                    case Item.WEAPON.AXE:
                        accuracy -= WEAPON_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                break;
            case Item.WEAPON.SPEAR:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.AXE:
                        accuracy += WEAPON_ADVANTAGE;
                        break;
                    case Item.WEAPON.CLUB:
                        accuracy -= WEAPON_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                break;
            case Item.WEAPON.AXE:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.CLUB:
                        accuracy += WEAPON_ADVANTAGE;
                        break;
                    case Item.WEAPON.SPEAR:
                        accuracy -= WEAPON_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                break;
            case Item.WEAPON.CURSE:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.ANIMA:
                        accuracy += WEAPON_ADVANTAGE;
                        break;
                    case Item.WEAPON.SPIRIT:
                        accuracy -= WEAPON_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                break;
            case Item.WEAPON.ANIMA:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.SPIRIT:
                        accuracy += WEAPON_ADVANTAGE;
                        break;
                    case Item.WEAPON.CURSE:
                        accuracy -= WEAPON_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                break;
            case Item.WEAPON.SPIRIT:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.CURSE:
                        accuracy += WEAPON_ADVANTAGE;
                        break;
                    case Item.WEAPON.ANIMA:
                        accuracy -= WEAPON_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                break;
        }
        if(playerUnit.currentStamina <= MINIMUM_STAMINA)
        {
            accuracy -= LOW_STAMINA_PENALTY;
        }
        if(accuracy < 0)
        {
            accuracy = 0;
        }
        return accuracy;
    }

    public int GetHitRate(CharacterMovement unit)
    {
        return unit.character.skll + (int)(unit.character.lck * 0.5) + unit.inventory.equippedWeapon.hit;
    }

    private int GetAvoid(CharacterMovement unit)
    {
        int avoid = GetAttackSpeed(unit) * 2 + unit.character.lck + unit.currentTile.tile.avoidBonus;
        if(unit.currentStamina <= MINIMUM_STAMINA)
        {
            avoid -= LOW_STAMINA_PENALTY;
        }
        if (avoid < 0)
        {
            avoid = 0;
        }
        return avoid; //+ any terrain bonus + stamina penalties :(
    }

    public int GetCritChance(CharacterMovement playerUnit, CharacterMovement enemyUnit)
    {
        return (int)(playerUnit.character.skll/2) + playerUnit.inventory.equippedWeapon.crit - enemyUnit.character.lck;
    }

    public int GetDamageDealt(CharacterMovement playerUnit, CharacterMovement enemyUnit)
    {
       int attack = playerUnit.character.str + playerUnit.inventory.equippedWeapon.might;
       int defense = 0; //any terrain bonuses they get
       switch(playerUnit.inventory.equippedWeapon.weaponType)
        {
            case Item.WEAPON.CLUB:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.SPEAR:
                        attack += WEAPON_DAMAGE_ADVANTAGE;
                        break;
                    case Item.WEAPON.AXE:
                        attack -= WEAPON_DAMAGE_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                defense += enemyUnit.character.def;
                break;
            case Item.WEAPON.SPEAR:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.AXE:
                        attack += WEAPON_DAMAGE_ADVANTAGE;
                        break;
                    case Item.WEAPON.CLUB:
                        attack -= WEAPON_DAMAGE_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                defense += enemyUnit.character.def;
                break;
            case Item.WEAPON.AXE:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.CLUB:
                        attack += WEAPON_DAMAGE_ADVANTAGE;
                        break;
                    case Item.WEAPON.SPEAR:
                        attack -= WEAPON_DAMAGE_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                defense += enemyUnit.character.def;
                break;
            case Item.WEAPON.CURSE:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.ANIMA:
                        attack += WEAPON_DAMAGE_ADVANTAGE;
                        break;
                    case Item.WEAPON.SPIRIT:
                        attack -= WEAPON_DAMAGE_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                defense += enemyUnit.character.res;
                break;
            case Item.WEAPON.ANIMA:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.SPIRIT:
                        attack += WEAPON_DAMAGE_ADVANTAGE;
                        break;
                    case Item.WEAPON.CURSE:
                        attack -= WEAPON_DAMAGE_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                defense += enemyUnit.character.res;
                break;
            case Item.WEAPON.SPIRIT:
                switch(enemyUnit.inventory.equippedWeapon.weaponType)
                {
                    case Item.WEAPON.CURSE:
                        attack += WEAPON_ADVANTAGE;
                        break;
                    case Item.WEAPON.ANIMA:
                        attack -= WEAPON_DAMAGE_DISADVANTAGE;
                        break;
                    default:
                        break; //Does nothing :D
                }
                defense += enemyUnit.character.res;
                break;
            case Item.WEAPON.BOW:
                defense += enemyUnit.character.def;
                break;
            case Item.WEAPON.SHIFTER:
                defense += enemyUnit.character.res;
                break;
        }
        return attack - defense;
    }

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
            if(playerUnit.inventory.equippedWeapon.weaponType == Item.WEAPON.SPIRIT)
            {
                playerUnit.character.curEXP += BASE_EXP_HEAL;
            }
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
