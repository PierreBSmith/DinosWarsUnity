using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Item")]

public class Item : ScriptableObject
{
    public enum TYPE
    {
        WEAPON,
        CONSUMABLE,
        ACCESORY
    }

    public enum WEAPON
    {
        CLUB,
        SPEAR,
        AXE,
        BOW,
        CURSE,
        ANIMA,
        SPIRIT,
        SHIFTER
    }

    public enum CONSUMABLE
    {
        MEDICINE,
        STAT_BOOSTER,
        KEY
    }

    public TYPE type;
    public int uses; //number of times you can use this, or durability of weapon

    //The following values are if the item is a weapon
    [Header("Weapon Variables")]
    public WEAPON weaponType;
    public int range;
    public int might;
    public int hit;
    public int avoid;
    public int weight;
    public int crit;
}
