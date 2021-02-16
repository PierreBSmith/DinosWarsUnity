using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class TileEvent : UnityEvent<TileBehaviour> { }
public class TileBehaviour : MonoBehaviour, IPointerClickHandler
{
    public Tile tile;

    private GameObject movementMask;

    public CharacterMovement occupied; //will be set dynamically depending on whether unit is there or not. Might not be necessary here 
    public TileEvent clicked; //Even that is thrown when the tile is clicked on
    public Vector2Int positon; //Logical coords of tile
    
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
    //void OnMouseUp()
    //{
    //    clicked?.Invoke(this);
    //}

    // Start is called before the first frame update
    void Awake()
    {
        movementMask = transform.Find("MovementMask").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clicked.Invoke(this);
    }
}
