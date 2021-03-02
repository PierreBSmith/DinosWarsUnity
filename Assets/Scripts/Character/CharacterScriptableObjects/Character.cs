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
    
    public int speed = 3;
    public int moveRange;
    public Type type;
    public int attackRange;
    public int attackDamage;
    public int maxHP;
    public bool grounded;
    public int maxStamina;
    public int attackStaminaCost;
    //TODO: Put all unit stats, Inventory, etc also in here
}
