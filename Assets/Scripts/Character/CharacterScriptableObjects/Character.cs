using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject
{
    public enum Type{
        FRIENDLY,
        ENEMY,
        NPC
    }
    
    public Sprite image;
    public string characterName;

    public int level;
    public int curEXP;

    public int moveSpeed = 3; //This is SPEED THE CHARACTER MOVES IN GAME!!! Not the stat -_-;
    public int moveRange;
    public Type type;
    public bool isBoss;
    public bool grounded;
    public int maxStamina;
    public int attackStaminaCost;
    //TODO: Put all unit stats, Inventory, etc also in here
    [Header("Stats Affected by Growths")]
    public int maxHP;
    public int str;
    public int skll;
    public int spd;
    public int lck;
    public int def;
    public int res;

    [Header("Base Stat Growths")]
    public int hpGrowth;
    public int strGrowth;
    public int skllGrowth;
    public int spdGrowth;
    public int lckGrowth;
    public int defGrowth;
    public int resGrowth;
}
