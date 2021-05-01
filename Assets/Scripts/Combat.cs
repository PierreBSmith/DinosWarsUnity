using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private const float COMBAT_ANIMATION_LENGTH = 0.5f;

    private bool unitKilled = false;

    private GameObject combatNumber;
    private Text combatNumberText;
    private RulesEngine _rulesEngine;

    private ExperienceUI experiencePanel;

    void Start()
    {
        combatNumber = GameObject.FindWithTag("CombatNumber");
        combatNumber.SetActive(false);
        combatNumberText = combatNumber.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>();

        _rulesEngine = GetComponent<RulesEngine>();

        GameObject LevelUpUI = GameObject.FindWithTag("LevelUpUI");
        experiencePanel = LevelUpUI.transform.GetChild(0).gameObject.GetComponent<ExperienceUI>();
        experiencePanel.gameObject.SetActive(false);
    }

    //We still need to implement critical percentages.
    public void CombatExchange(CharacterMovement playerUnit, CharacterMovement enemyUnit, int range)
    {
        //HEALINGUUUUUU.
        if(playerUnit.inventory.equippedWeapon.weaponType == Item.WEAPON.SPIRIT)
        {
            enemyUnit.currHP += (int)(playerUnit.character.str * 1.5f);
            playerUnit.currentStamina -= playerUnit.character.attackStaminaCost;
            if(enemyUnit.currHP > enemyUnit.character.maxHP)
            {
                enemyUnit.currHP = enemyUnit.character.maxHP;
            }
            playerUnit.inventory.equippedWeapon.uses--;
            GainEXP(playerUnit, enemyUnit, playerUnit.inventory.equippedWeapon.might, false);
        }
        else
        {
            //Gotta calculate the heading of the two first.
            StartCoroutine(CheckIfCombatDone(playerUnit, enemyUnit, range));
        }
    }
    //Probably want an IEnumerator for attack pausa and animations and stoof

    public int GetAttackSpeed(CharacterMovement unit)
    {
        if(unit.inventory.equippedWeapon)
        {
            int burden = unit.inventory.equippedWeapon.weight - unit.character.str;
            if(burden < 0)
            {
                burden = 0;
            }
            return unit.character.spd - burden;
        }
        return 0;
    }

    public int GetAccuracy(CharacterMovement playerUnit, CharacterMovement enemyUnit)
    {
        int accuracy = GetHitRate(playerUnit) - GetAvoid(enemyUnit);
        if(playerUnit.inventory.equippedWeapon && enemyUnit.inventory.equippedWeapon)
        {
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
        int weaponBonus = 0;
        if(unit.inventory.equippedWeapon)
        {
            weaponBonus = unit.inventory.equippedWeapon.hit;
        }
        return unit.character.skll + (int)(unit.character.lck * 0.5) + weaponBonus;
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
        int weaponBonus = 0;
        if(playerUnit.inventory.equippedWeapon)
        {
            weaponBonus = playerUnit.inventory.equippedWeapon.crit;
        }
        return (int)(playerUnit.character.skll/2) + weaponBonus - enemyUnit.character.lck;
    }

    public int GetDamageDealt(CharacterMovement playerUnit, CharacterMovement enemyUnit)
    {
        int attack = playerUnit.character.str; 
        if(playerUnit.inventory.equippedWeapon)
        { 
            attack += playerUnit.inventory.equippedWeapon.might;
        }
        int defense = enemyUnit.currentTile.tile.defResBonus; //any terrain bonuses they get
        if(playerUnit.inventory.equippedWeapon && enemyUnit.inventory.equippedWeapon)
        {
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
        }
        return attack - defense;
    }

    private void GainEXP(CharacterMovement playerUnit, CharacterMovement enemyUnit, int damageDealt, bool killedEnemy)
    {
        int startEXP = playerUnit.character.curEXP;
        if(!killedEnemy) //If the enemy did not die
        {
            if(playerUnit.inventory.equippedWeapon.weaponType == Item.WEAPON.SPIRIT)
            {
                playerUnit.character.curEXP += BASE_EXP_HEAL;
            }
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
        int newEXP = playerUnit.character.curEXP;
        StartCoroutine(GainEXPCoroutine(playerUnit, startEXP, newEXP));
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

    private void KillUnit(CharacterMovement character)
    {
        if(character.character.type == Character.Type.FRIENDLY)
        {
            _rulesEngine.friendlyList.Remove(character);
            StartCoroutine(endOfGame(character));
            
        }
        else if (character.character.type == Character.Type.ENEMY)
        {
            Debug.Log("Enemy dying");
            _rulesEngine.enemyList.Remove(character);
            StartCoroutine(endOfGame(character));
        }
        else
        {
            _rulesEngine.NPCList.Remove(character);
        }
        character.currentTile.occupied = null;
        character.currentTile = null;
        character.gameObject.SetActive(false);
    }

    private IEnumerator endOfGame(CharacterMovement character)
    {
        yield return new WaitForSeconds(1);
        GameObject gameManager = GameObject.Find("GameManager");
        if (character.character.characterName == "Asku" || character.character.characterName == "Tatam")
        {
            Time.timeScale = 0f;
            _rulesEngine.endGame.gameObject.SetActive(true);
            _rulesEngine.endGame.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "You Lose";
            yield return new WaitForSecondsRealtime(5);
            Time.timeScale = 1f;
            gameManager.GetComponent<GameManager>().inLevel = false;
            SceneManager.LoadScene("WorldMap");
            //Application.Quit();
        }
        if (_rulesEngine.enemyList.Count == 0)
        {
            Time.timeScale = 0f;
            _rulesEngine.endGame.gameObject.SetActive(true);
            _rulesEngine.endGame.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "You Win";
            yield return new WaitForSecondsRealtime(5);
            //Application.Quit();
            Time.timeScale = 1f;
            gameManager.GetComponent<GameManager>().currentLevel++;
            gameManager.GetComponent<GameManager>().inLevel = false;
            SceneManager.LoadScene("WorldMap");
        }
    }

    private IEnumerator CheckIfCombatDone(CharacterMovement playerUnit, CharacterMovement enemyUnit, int range)
    {
        yield return StartCoroutine(CombatRoutine(playerUnit, enemyUnit, range));
    }

    private IEnumerator CombatRoutine(CharacterMovement playerUnit, CharacterMovement enemyUnit, int range)
    {
        int headingX = (int)enemyUnit.currentTile.gameObject.transform.position.x - (int)playerUnit.currentTile.gameObject.transform.position.x;
        int headingY = (int)enemyUnit.currentTile.gameObject.transform.position.y - (int)playerUnit.currentTile.gameObject.transform.position.y;
        bool hit = false; //This is if the unit hit or not. Is used for combat animation
        int damage = 0;

        //Combat
        int playerAccuracy = GetAccuracy(playerUnit, enemyUnit);
        int hitChance = Random.Range(0, 101); //Random number in range 0 - 100
        playerUnit.currentStamina -= playerUnit.character.attackStaminaCost;
        if (hitChance <= playerAccuracy)
        {
            //Hit enemy
            hit = true;
            int critChance = Random.Range(0,101);
            if(critChance <= GetCritChance(playerUnit, enemyUnit))
            {
                damage = GetDamageDealt(playerUnit, enemyUnit) * 3;
                enemyUnit.currHP -= damage;
            }
            else
            {
                damage = GetDamageDealt(playerUnit, enemyUnit);
                enemyUnit.currHP -= damage;
            }
            playerUnit.inventory.equippedWeapon.uses--;
        }
        else
        {
            hit = false;
        }
        Debug.Log(playerUnit.name + " attacked " + enemyUnit.name + " has " + enemyUnit.currHP);
        yield return StartCoroutine(CombatAnimation(playerUnit, headingX, headingY, hit, damage));
        //Check if enemy dead
        if(enemyUnit.currHP <= 0)
        {
            if(playerUnit.character.type == Character.Type.FRIENDLY)
            {
                GainEXP(playerUnit, enemyUnit, GetDamageDealt(playerUnit, enemyUnit), true);
            }
            Debug.Log(playerUnit.name + " killed " + enemyUnit.name);
            KillUnit(enemyUnit);
            yield break;
        }
        //Enemy phase of attack
        //First must check if enemy can counter attack
        int enemyAccuracy = 0;
        if(enemyUnit.inventory.equippedWeapon &&
            range >= enemyUnit.inventory.equippedWeapon.minRange && range <= enemyUnit.inventory.equippedWeapon.maxRange)
        {
            hitChance = Random.Range(0, 101); //new Random number!
            enemyAccuracy = GetAccuracy(enemyUnit, playerUnit);
            enemyUnit.currentStamina -= enemyUnit.character.attackStaminaCost;
            if(hitChance <= enemyAccuracy)
            {
                //Get hit
                hit = true;
                int critChance = Random.Range(0,101);
                if(critChance <= GetCritChance(enemyUnit, playerUnit))
                {
                    damage = GetDamageDealt(enemyUnit, playerUnit) * 3;
                    playerUnit.currHP -= damage;
                }
                else
                {
                    damage = GetDamageDealt(enemyUnit, playerUnit);
                    playerUnit.currHP -= damage;
                }
                enemyUnit.inventory.equippedWeapon.uses--;
            }
            else
            {
                hit = false;
            }
            Debug.Log(enemyUnit.name + " attacked " + playerUnit.name + " has " + playerUnit.currHP);
            yield return StartCoroutine(CombatAnimation(enemyUnit, -headingX, -headingY, hit, damage)); //pass in opposite since the enemy acts in the opposite direction
            //Check if player dead
            if(playerUnit.currHP <= 0)
            {
                Debug.Log(enemyUnit.name + " killed " + playerUnit.name);
                if(enemyUnit.character.type == Character.Type.FRIENDLY)
                {
                    GainEXP(enemyUnit, playerUnit, GetDamageDealt(enemyUnit, playerUnit), true);
                }
                KillUnit(playerUnit);
                yield break;
            }
        }

        //Second player hit
        if(GetAttackSpeed(playerUnit) - GetAttackSpeed(enemyUnit) >= DOUBLING_MAGIC_NUMBER)
        {
            //Double yay!
            hitChance = Random.Range(0, 101); //Another new Random;
            if (hitChance <= playerAccuracy)
            {
                //Hit enemy
                hit = true;
                int critChance = Random.Range(0,101);
                if(critChance <= GetCritChance(playerUnit, enemyUnit))
                {
                    damage = GetDamageDealt(playerUnit, enemyUnit) * 3;
                    enemyUnit.currHP -= damage;
                }
                else
                {
                    damage = GetDamageDealt(playerUnit, enemyUnit);
                    enemyUnit.currHP -= damage;
                }
                playerUnit.inventory.equippedWeapon.uses--;
            }
            else
            {
                hit = false;
            }
            Debug.Log(playerUnit.name + " attacked " + enemyUnit.name + " has " + enemyUnit.currHP);
            yield return StartCoroutine(CombatAnimation(playerUnit, headingX, headingY, hit, damage));
            //Check if enemy dead
            if(enemyUnit.currHP <= 0)
            {
                if(playerUnit.character.type == Character.Type.FRIENDLY)
                {
                    GainEXP(playerUnit, enemyUnit, GetDamageDealt(playerUnit, enemyUnit), true);
                }
                Debug.Log(playerUnit.name + " killed " + enemyUnit.name);
                KillUnit(enemyUnit);
                yield break;
            }
        }
        //Enemy double fricking sadge :(
        if(enemyUnit.inventory.equippedWeapon &&
            range >= enemyUnit.inventory.equippedWeapon.minRange && range <= enemyUnit.inventory.equippedWeapon.maxRange)
        {
            if (GetAttackSpeed(enemyUnit) - GetAttackSpeed(playerUnit) >= DOUBLING_MAGIC_NUMBER)
            {
                hitChance = Random.Range(0, 101); //Another new Random;
                if (hitChance <= enemyAccuracy)
                {
                    //Hit enemy
                    hit = true;
                    int critChance = Random.Range(0,101);
                    if(critChance <= GetCritChance(enemyUnit, playerUnit))
                    {
                        damage = GetDamageDealt(enemyUnit, playerUnit) * 3;
                        playerUnit.currHP -= damage;
                    }
                    else
                    {
                        damage = GetDamageDealt(enemyUnit, playerUnit);
                        playerUnit.currHP -= damage;
                    }
                    enemyUnit.inventory.equippedWeapon.uses--;
                }
                else
                {
                    hit = false;
                }  
                Debug.Log(enemyUnit.name + " attacked " + playerUnit.name + " has " + playerUnit.currHP);              
                yield return StartCoroutine(CombatAnimation(enemyUnit, -headingX, -headingY, hit, damage));
                //Check if enemy dead
                if(playerUnit.currHP <= 0)
                {
                    Debug.Log(enemyUnit.name + " killed " + playerUnit.name);
                    if(enemyUnit.character.type == Character.Type.FRIENDLY)
                    {
                        GainEXP(enemyUnit, playerUnit, GetDamageDealt(enemyUnit, playerUnit), true);
                    }
                    KillUnit(playerUnit);
                    yield break;
                }
            }
        }   
    }

    private IEnumerator CombatAnimation(CharacterMovement character, int headingX, int headingY, bool hit, int damage)
    {
        character._animator.SetBool("attacking", true);
        if(hit)
        {
            combatNumberText.text = damage.ToString();
        }
        else
        {
            combatNumberText.text = "Miss";
        }
        if(Mathf.Abs(headingX) >= Mathf.Abs(headingY)) //If character should jiggle in the X direction
        {
            character._animator.SetInteger("Xvalue", headingX);
        }
        else //If character should jiggle in the Y direction
        {
            character._animator.SetInteger("Yvalue", headingY);
        }
        combatNumber.SetActive(true);
        combatNumber.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(character.gameObject.transform.position);
        yield return new WaitForSeconds(COMBAT_ANIMATION_LENGTH);
        character._animator.SetBool("attacking", false);
        combatNumber.SetActive(false);
    }

    private IEnumerator GainEXPCoroutine(CharacterMovement character, int startEXP, int newEXP)
    {
        experiencePanel.gameObject.SetActive(true);
        experiencePanel.OpenExperienceUI(character, startEXP);
        Time.timeScale = 0f;
        yield return StartCoroutine(ExperiencePointUp(startEXP, newEXP));
        experiencePanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private IEnumerator ExperiencePointUp(int startEXP, int newEXP)
    {
        int tempEXP = startEXP;
        while(tempEXP < newEXP)
        {
            tempEXP++;
            if(tempEXP >= 100)
            {
                tempEXP = 0;
                newEXP = newEXP - 100;
            }
            yield return StartCoroutine(IncrementEXP(tempEXP));
        }
        yield return new WaitForSecondsRealtime(1.5f);
    }

    private IEnumerator IncrementEXP(int currentEXP)
    {
        experiencePanel.experienceBar.value = currentEXP;
        experiencePanel.experienceNum.text = currentEXP.ToString();
        yield return new WaitForSecondsRealtime(0.05f);
    }
}
