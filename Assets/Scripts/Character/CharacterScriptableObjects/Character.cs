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
    
    public int moveSpeed = 3; //This is SPEED THE CHARACTER MOVES IN GAME!!! Not the stat -_-;
    public int moveRange;
    public Type type;
    public int attackRange;
    public int attackDamage;
    public int maxHP;
    public bool grounded;
    public int maxStamina;
    //TODO: Put all unit stats, Inventory, etc also in here
}
