using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class CharacterEvent : UnityEvent<Character> { }

public class Character : MonoBehaviour
{
    public enum Type{
        FRIENDLY,
        ENEMY,
        NPC
    }
    
    public int speed = 3;
    //moveError = speed/100 PROBABLY UNNECESSARY IN UNITY
    public int moveRange;
    public Type type;
    public int attackRange;
    public int attackDamage;
    public int HP;
    public int currHP;
    public bool grounded;
    public Vector2Int position; //This might not need to be here
    public CharacterEvent clicked;
    public UnityEvent doneMoving;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void move(PathToTile path)
    {
        StartCoroutine(followPath(path));
    }

    private IEnumerator followPath(PathToTile path)
    {
        Queue<Vector2Int> actualPath = new Queue<Vector2Int>(path.path);
        actualPath.Enqueue(path.tile);
        position = path.tile;
        Vector2 target = actualPath.Dequeue();
        while (true)
        { 
            Vector2 dist = target - (Vector2)transform.position;
            print(speed);
            if (dist.magnitude > 0.05)
            {
                transform.position += (Vector3)(dist.normalized * Time.deltaTime * speed);
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
    void OnMouseUp()
    {
        clicked.Invoke(this);
    }
    // public Character(int speed, int moveRange, Type type, int attackRange, int attackDamage, int HP){
    //     this.speed = speed;
    //     this.moveRange = moveRange;
    //     this.type = type;
    //     this.attackRange = attackRange;
    //     this.attackDamage = attackDamage;
    //     this.HP = HP;
    //     this.currHP = HP;
    // }
}

