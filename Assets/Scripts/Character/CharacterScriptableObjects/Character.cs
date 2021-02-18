﻿using System.Collections;
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
    
    public int speed = 3;
    public int moveRange;
    public Type type;
    public int attackRange;
    public int attackDamage;
    public int HP;
    public int currHP; 
    public bool grounded;

    public int maxStamina;
    //TODO: Put all unit stats, Inventory, etc also in here
}