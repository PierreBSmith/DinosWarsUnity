using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

[Serializable]
public class CharacterEvent : UnityEvent<CharacterMovement> { }

public class CharacterMovement : MonoBehaviour, IPointerClickHandler
{
    public Character character;
    public Vector2Int position; //This might not need to be here
    public CharacterEvent clicked; //Event for when Character is clicked. Is handled by RulesEngine
    public UnityEvent doneMoving; //Event for when Character has stopped moving after a movement command. Is handled by RulesEngine
    public Button attackButton;
    public Button moveButton;
    public Canvas canvas;
    public CharacterEvent moveClicked;
    public CharacterEvent attackClicked;
    public CharacterEvent passClicked;
    public int currHP;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //This starts movement along the given path
    public void move(PathToTile path)
    {
        StartCoroutine(followPath(path));
    }
    //This takes in a path and moves the unit along that path
    private IEnumerator followPath(PathToTile path)
    {
        Queue<Vector2Int> actualPath = new Queue<Vector2Int>(path.path);
        actualPath.Enqueue(path.tile);
        position = path.tile;
        Vector2 target = actualPath.Dequeue();
        while (true)
        { 
            Vector2 dist = target - (Vector2)transform.position;
            if (dist.magnitude > 0.05)
            {
                transform.position += (Vector3)(dist.normalized * Time.deltaTime * character.speed);
            }
            else if(actualPath.Count != 0)
            {
                transform.position += (Vector3)dist;
                target = actualPath.Dequeue();
            }
            else
            {
                doneMoving.Invoke();
                break;
            }
            yield return null;
        }
    }
    //Event handler function
    //void OnMouseUp()
    //{
    //    clicked.Invoke(this);
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        clicked.Invoke(this);
    }

    public void moveClickedEvent()
    {
        moveClicked.Invoke(this);
    }

    public void attackClickedEvent()
    {
        attackClicked.Invoke(this);
    }
    
    public void passClickedEvent()
    {
        passClicked.Invoke(this);
    }
}

