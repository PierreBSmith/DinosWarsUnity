﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Events;
using System;

[Serializable]
public class TileEvent : UnityEvent<TileBehaviour> { }
public class TileBehaviour : MonoBehaviour
{
    public Tile tile;
    [HideInInspector]
    public GameObject movementMask;
    private SpriteRenderer _sprite;

    public CharacterMovement occupied; //will be set dynamically depending on whether unit is there or not. Might not be necessary here 
    public TileEvent clicked; //Even that is thrown when the tile is clicked on
    public Vector2Int positon; //Logical coords of tile

    //Most of these variables are actually never used here and are set elsewhere.
    [HideInInspector]
    public bool targetTile = false;
    [HideInInspector]
    public bool selectable = false;
    [HideInInspector]
    public bool hasUnit = false;
    [HideInInspector]
    public List<TileBehaviour> neighbours = new List<TileBehaviour>();
    //BFS variables WHEEEEEEEEE
    [HideInInspector]
    public bool visited = false;
    [HideInInspector]
    public TileBehaviour parent = null;
    [HideInInspector]
    public int distance = 0; //how far this tile is from the start tile
    //A* variables whoop whoop
    [HideInInspector]
    public float totalCost = 0;
    [HideInInspector]
    public float costFromParentToCurrentTile = 0;
    [HideInInspector]
    public float costFromProcessedTileToTargetTile = 0;

    [HideInInspector]
    public GameObject unit;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }


    //This is called when a unit is clicked to show its range of movement
    public void setMask(bool isAttack, Character.Type type){
        movementMask.SetActive(true);
        _sprite.color = Color.green;

        //movementMask.GetComponent<SpriteShapeRenderer>().color = new Color(0, 1, 0, .5f);
        /*
        if (isAttack || type == Character.Type.ENEMY)
        {
            movementMask.GetComponent<SpriteShapeRenderer>().color = new Color(1, 0, 0, .5f);
        }
        else
        {
            movementMask.GetComponent<SpriteShapeRenderer>().color = new Color(0, 1, 0, .5f);
        }
        */
    }
    //This is called to undo display of a movement range
    public void clearMask()
    {
        _sprite.color = Color.white;
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
        //Get's movementMask
        movementMask = transform.GetChild(0).gameObject;
    }

    //Min's Tile Movement Stuff Down Here
    public void ResetTile()
    {
        neighbours.Clear();

        targetTile = false;
        selectable = false;

        visited = false;
        parent = null;
        distance = 0;

        totalCost = costFromParentToCurrentTile = costFromProcessedTileToTargetTile = 0;
    }

    //Checks if there are tiles in the given direction.
    //direction given as a Vector3 instead of a Vector2 for addition to position purposes since transform.position is a Vector3
    private void CheckTile(Vector3 direction)
    {
        Vector2 tileChecker = new Vector2(0.25f, 0.25f); //this is just a Vector2 range
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, tileChecker); //draws a box that overlaps with all corners of the neighboring tiles
        foreach(Collider collider in colliders)
        {
            TileBehaviour tile = collider.GetComponent<TileBehaviour>();
            if(tile != null && tile.tile.walkable)
            {
                neighbours.Add(tile);
            }
        }
    }

    public void FindNeighbors()
    {
        ResetTile();

        CheckTile(Vector3.up); //checks if tile above it
        CheckTile(-Vector3.up); //checks if tile below it
        CheckTile(Vector3.right); //checks if tile to the right
        CheckTile(-Vector3.right); //checks if tile to the left
    }
}
