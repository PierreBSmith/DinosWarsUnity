using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile")]
public class Tile : ScriptableObject
{
    public enum Type{
        GRASS,
        FOREST,
        WATER,
        SAND,
        MOUNTAIN,
        FORTRESS
    }

    public Type type;
    public bool walkable;
    public int avoidBonus;
    public int defResBonus;
    public int movementBonus;
}
