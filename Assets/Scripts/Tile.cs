using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Events;
using System;

[Serializable]
public class TileEvent : UnityEvent<Tile> { }
public class Tile : MonoBehaviour
{
    public enum Type{
        GRASS,
        FOREST,
        WATER,
        SAND,
        MOUNTAIN,
        FORTRESS
    }

    //tile properties will be set on creation of map
    public Type type{get; private set;} //tile type that decides the rest of the fields
    public bool walkable{ get; private set;} //Whether grounded units can move through the terrain or not
    public int avoidBonus{ get; private set;} //Effect unit evasion rate. Represents a percentage
    public int defResBonus{ get; private set;} //Effects unit's defense and resistance stat. These represent integer values
    public int movementBonus{ get; private set;} //Certain terrains will be harder to move through
    private GameObject movementMask;

    public Character occupied; //will be set dynamically depending on whether unit is there or not. Might not be necessary here 
    public TileEvent clicked; //Even that is thrown when the tile is clicked on
    public Vector2Int positon; //Logical coords of tile

    public void setTile(Type type){
        this.type = type;
        switch (type)
        {
            case(Type.GRASS):
                walkable = true;
                avoidBonus = 0;
                defResBonus = 0;
                movementBonus = 0;
                break;
            case(Type.FOREST):
                walkable = true;
                avoidBonus = 10;
                defResBonus = 0;
                movementBonus = -1;
                break;
            case(Type.MOUNTAIN):
                walkable = false;
                avoidBonus = 30;
                defResBonus = 5;
                movementBonus = -3;
                break;
            case(Type.WATER):
                walkable = false;
                avoidBonus = 0;
                defResBonus = 0;
                movementBonus = -3;
                break;
            case(Type.SAND):
                walkable = true;
                avoidBonus = -10;
                defResBonus = 0;
                movementBonus = -2;
                break;
            case(Type.FORTRESS):
                walkable = true;
                avoidBonus = 0;
                defResBonus = 10;
                movementBonus = 0;
                break;
            default:
                Debug.LogError("This tile type isnt valid");
                break;
        }
    }
    
    //This is called when a unit is clicked to show its range of movement
    public void setMask(bool isAttack, Character.Type type){
        movementMask.SetActive(true);
        if (isAttack || type == Character.Type.ENEMY)
        {
            movementMask.GetComponent<SpriteShapeRenderer>().color = new Color(1, 0, 0, .5f);
        }
        else
        {
            movementMask.GetComponent<SpriteShapeRenderer>().color = new Color(0, 1, 0, .5f);
        }
    }
    //This is called to undo display of a movement range
    public void clearMask()
    {
        movementMask.SetActive(false);
    }
    //Event handler
    void OnMouseUp()
    {
        clicked.Invoke(this);
    }

    // Start is called before the first frame update
    void Awake()
    {
        movementMask = transform.Find("MovementMask").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
