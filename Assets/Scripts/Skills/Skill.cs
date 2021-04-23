using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    
    [Header("Base Stat modifiers")]
    public int maxHP;
    public int skll;
    public int spd;
    public int lck;
    public int def;
    public int res;

    [Header("Base Stat Growth Modifiers")]
    public int hpGrowth;
    public int strGrowth;
    public int skllGrowth;
    public int spdGrowth;
    public int lckGrowth;
    public int defGrowth;
    public int resGrowth;

    [Header("Other Stuff Skills can modify")]
    public int critDamage;
}
