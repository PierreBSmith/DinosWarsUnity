using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
[Serializable]
public class CharacterEvent : UnityEvent<CharacterMovement> { }

public class CharacterMovement : MonoBehaviour, IPointerClickHandler
{
    public Character character;

    [Header("Stamina Implementation")]
    public int currentStamina;
    private const int LINEAR_STAMINA_DEPLETION = 10;
    public readonly int attackStaminaCost = 30;
    private int extraMovementRange;
    [Header("Movement")]
    private List<TileBehaviour> selectableTiles = new List<TileBehaviour>(); //the list of tiles that can be moved to
    private GameObject[] tiles; //stores all tiles of the map
    private Stack<TileBehaviour> path = new Stack<TileBehaviour>(); //the actual path the character wishes to travel along
    public bool hasMoved = false; //variables to know if the unit can still do this action this turn
    public bool hasAttacked = false;
    [HideInInspector]
    public TileBehaviour currentTile; //the tile the unit is currently inhabiting
    private TileBehaviour targetTile;
    private Vector3 velocity = new Vector3(); //the speed the unit is moving
    private Vector3 heading = new Vector3(); //the direction the unit is moving
    private const float HEIGHT_OF_UNIT_ABOVE_TILE = 0.5f;
    public int currHP;
    [HideInInspector]
    public SpriteRenderer _sprite;
    [HideInInspector]
    public Animator _animator;

    public Vector2Int position; //This might not need to be here
    public CharacterEvent clicked; //Event for when Character is clicked. Is handled by RulesEngine
    public CharacterEvent passTurn; //Event for when Character has stopped moving after a movement command. Is handled by RulesEngine
    public UnityEvent doneMoving;
    public CharacterEvent unitAttacking;
    public Canvas canvas;
    public List<CharacterMovement> attackableList = new List<CharacterMovement>();

    void Start()
    {
        currentStamina = character.maxStamina;
        currHP = character.maxHP;
        GetExtraRange();
        //unfortunately would have to call this in Update if we decided to make a map with disappearing tiles LMAO
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        GetCurrentTile();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    //This takes in a path and moves the unit along that path


    //Event handler functions
    //void OnMouseDown()
    //{
    //    clicked.Invoke(this);
    //}
    public void OnPointerClick(PointerEventData eventData)
    {
        clicked.Invoke(this);
        //Debug.Log("Character");
    }
    //Button Panel functions
    public void moveButtonClicked() //called when action panel move button is clicked
    {
        if (!hasMoved)
        {
            DisplayRange(true, false);
            canvas.gameObject.SetActive(false);
        }
    }

    
    public void passActive() //called when action panel pass button is clicked, removes unit from active list without taking any more actions
    {
        passTurn.Invoke(this);
    }

    public void attackButtonClicked()
    {
        if (!hasAttacked && currentStamina >= attackStaminaCost)
        {
            DisplayRange(true, true);
            //Debug.Log(selectableTiles.Count);
            canvas.gameObject.SetActive(false);
            unitAttacking.Invoke(this);
        }
    }
    //CHARACTER RESET FUNCTION. PLEASE CALL BEFORE THE START OF THE PLAYER PHASE!!!!!!!!!!!!!
    public void ResetTurn()
    {
        currentStamina = character.maxStamina;
        hasMoved = false;
        hasAttacked = false;
        _sprite.color = Color.white;
        _animator.enabled = true;
        _animator.Play("Idle", 0, 0f);
    }

    public void ResetTemp()
    {
        _sprite.color = Color.white;
        _animator.enabled = true;
        _animator.Play("Idle", 0, 0f);
    }

    private void GetExtraRange()
    {
        int normalStaminaUsage = character.moveRange * LINEAR_STAMINA_DEPLETION;
        int maxStaminaUsage = normalStaminaUsage;
        extraMovementRange = 0;
        while(maxStaminaUsage < currentStamina)
        {
            extraMovementRange++;
            maxStaminaUsage += (extraMovementRange + 1) * LINEAR_STAMINA_DEPLETION;
            if(maxStaminaUsage > currentStamina)
            {
                extraMovementRange--;
                break;
            }
        }
        //Debug.Log(extraMovementRange);
    }

    private TileBehaviour GetTargetTile(GameObject target)
    {
        RaycastHit2D hit; //It's not in 2D cuz we're working with the Z-Axis here. Has since been changed to 2D because needed to be for 2D colliders on tiles.
        TileBehaviour tile = null;
        hit = Physics2D.Raycast(target.transform.position, Vector2.down, .1f, LayerMask.GetMask("Tile"));
        if (hit)
        {
            tile = hit.collider.GetComponent<TileBehaviour>();
        }

        return tile;
    }

    private void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        //currentTile.unit = gameObject;
    }

    private void ComputeNeighboringTiles()
    {
        foreach(GameObject tile in tiles)
        {
            TileBehaviour t = tile.GetComponent<TileBehaviour>();
            t.FindNeighbors();
        }
    }

    public void FindAttackableTiles()
    {
        ComputeNeighboringTiles();
        GetCurrentTile();

        //This BFS finds tiles. It has nothing to do with Pathfinding
        Queue<TileBehaviour> process = new Queue<TileBehaviour>();
        process.Enqueue(currentTile);
        currentTile.visited = true;

        while (process.Count > 0)
        {
            TileBehaviour tile = process.Dequeue();
            //checks if the distance of that tile is within the movement range.
            //TODO: probably have a sort of checker to see how many extra tiles the unit can move depending on stamina left over :D
            if (tile.distance <= (character.attackRange) && tile != currentTile)//&& !tile.hasUnit
            {
                //These checks are for stamina usage stuff

                //Adds tile to selectable Tile list if it's within movement range and there's nothing else on the tile
                selectableTiles.Add(tile);
                tile.selectable = true;
                if(tile.occupied && tile.occupied.character.type != character.type)
                {
                    //Gets all units in attack range that aren't on their team
                    attackableList.Add(tile.occupied);
                }
            }

            //This looks for more tiles that can be moved to
            if (tile.distance < (character.attackRange))
            {
                foreach (TileBehaviour t in tile.neighbours)
                {
                    if (!t.visited)
                    {
                        t.parent = tile;
                        t.visited = true;
                        t.distance = 1 + tile.distance; //if it's a child of the parent node, then it's on tile farther than the parent tile
                        process.Enqueue(t);
                    }
                }
            }
        }
    }
    private void FindSelectableTiles()
    {
        ComputeNeighboringTiles();
        GetCurrentTile();

        //This BFS finds tiles. It has nothing to do with Pathfinding
        Queue<TileBehaviour> process = new Queue<TileBehaviour>();
        process.Enqueue(currentTile);
        currentTile.visited = true;

        while(process.Count > 0)
        {
            TileBehaviour tile = process.Dequeue();
            //checks if the distance of that tile is within the movement range.
            //TODO: probably have a sort of checker to see how many extra tiles the unit can move depending on stamina left over :D
            
            if(tile.distance <= (character.moveRange + extraMovementRange) && (!tile.occupied || tile == currentTile))
            {
                //These checks are for stamina usage stuff
                if(tile.distance <= character.moveRange && (!tile.occupied || tile == currentTile))
                {
                    tile.withinRange = true;
                }
                else
                {
                    tile.withinRange = false;
                }
                //Adds tile to selectable Tile list if it's within movement range and there's nothing else on the tile
                selectableTiles.Add(tile);
                tile.selectable = true;
            }
            
            //This looks for more tiles that can be moved to
            if(tile.distance < (character.moveRange + extraMovementRange))
            {
                foreach(TileBehaviour t in tile.neighbours)
                {
                    if(!t.visited)
                    {
                        t.parent = tile;
                        t.visited = true;
                        t.distance = 1 + tile.distance; //if it's a child of the parent node, then it's on tile farther than the parent tile
                        //Debug.Log("Distance " + t.distance);
                        process.Enqueue(t);
                    }
                }
            }
        }
    }

    //Pathfinding time
    //Takes in the target tile to move to as a parameter
    //This method is for the player units. An actual A* movement will be implemented later.
    public void FindPath(TileBehaviour tile)
    {
        path.Clear(); //so we don't have any leftover stuff from the previous move
        tile.targetTile = true;

        TileBehaviour nextTile = tile;
       // Debug.Log(path.Count + " path count " + character.type);
        while (nextTile != null)
        { 
            if(nextTile.selectable && nextTile != currentTile)
            {
                path.Push(nextTile);
            }
            nextTile = nextTile.parent;
        }
    }

    //Gets direction the unit needs to move
    private void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    //Sets the velocity to move the unit in that direction
    private void SetVelocity()
    {
        velocity = heading * character.speed;
    }

    public void RemoveSelectableTiles()
    {
        if(currentTile != null)
        {
            currentTile = null;
        }
        foreach (TileBehaviour tile in selectableTiles)
        {
            tile.ResetTile();
        }
        selectableTiles.Clear();
    }

    public void Move()
    {
        //Debug.Log(path.Count);
        StartCoroutine(followPath());
        DisplayRange(false, false);
        GetCurrentTile();
    }

    private IEnumerator followPath()
    {

        TileBehaviour moveTarget;

        while (true)
        {
            if (path.Count != 0)
            {
                moveTarget = path.Peek();

            }
            else {
                RemoveSelectableTiles();
                GetCurrentTile();
                currentTile.occupied = this;
                doneMoving.Invoke();
                break;

            }
            Vector3 targetPosition = moveTarget.transform.position;
            targetPosition.z -= HEIGHT_OF_UNIT_ABOVE_TILE;

            //Calculates Unit's position
            if (Vector2.Distance(transform.position, targetPosition) >= 0.05f)
            {
                CalculateHeading(targetPosition);
                SetVelocity();
                transform.position += velocity * Time.deltaTime;
                //We don't need to attach Rigidbody2D to the unit because they're not acting on Physics, they're just moving their transform places.
                //We should somehow make the movement gradual instead. Coroutine?
            }
            else
            {
                //The unit has reached the center of the tile
                transform.position = targetPosition;
                //Debug.Log(moveTarget.distance + " distance is");
                //Stamina stuff here. For now it's going to be linear since we have no way of getting extra movement range yet.
                if (moveTarget.distance > character.moveRange)
                {
                    int extraDistance = (moveTarget.distance - character.moveRange) + 1;
                    currentStamina -= extraDistance * LINEAR_STAMINA_DEPLETION;
                }
                else
                {
                    currentStamina -= LINEAR_STAMINA_DEPLETION;
                }
                if (path.Count > 0)
                {
                    path.Pop();

                }

            }
            yield return null;
        }
    }

    //private IEnumerator followPath(PathToTile path)
    //{
    //    Queue<Vector2Int> actualPath = new Queue<Vector2Int>(path.path);
    //    actualPath.Enqueue(path.tile);
    //    position = path.tile;
    //    Vector2 target = actualPath.Dequeue(); //We need to figure out how to gradually increase stamina intake depending on distance.
    //    while (true)
    //    {
    //        Vector2 dist = target - (Vector2)transform.position;
    //        print(character.speed);
    //        if (dist.magnitude > 0.05)
    //        {
    //            //I assume this is where the unit records that it has moved one tile?
    //            transform.position += (Vector3)(dist.normalized * Time.deltaTime * character.speed);
    //        }
    //        else if (actualPath.Count != 0)
    //        {
    //            transform.position += (Vector3)dist;
    //            target = actualPath.Dequeue();
    //        }
    //        else
    //        {
    //            doneMoving.Invoke();
    //            break;
    //        }
    //        yield return null;
    //    }
    //}

    //This starts movement along the given path
    /*
    public void move(PathToTile path)
    {
        StartCoroutine(followPath(path));
    }*/

    //Now the fun stuff. Pathfinding for AI :D
    //A* time
    protected TileBehaviour FindLowestTotalCost(List<TileBehaviour> list)
    {
        TileBehaviour lowest = list[0];
        foreach(TileBehaviour tile in list)
        {
            //Debug.Log(tile.name + " in FOR EACH");
            if(tile.totalCost < lowest.totalCost)
            {
                lowest = tile;
            }
        }

        list.Remove(lowest);

        return lowest;
    }

    protected TileBehaviour FindEndTile(TileBehaviour tile)
    {
        Stack<TileBehaviour> tempPath = new Stack<TileBehaviour>();
        TileBehaviour nextTile = tile.parent;

        while(nextTile != null)
        {
            tempPath.Push(nextTile);
            nextTile = nextTile.parent;
        }

        if(tempPath.Count <= character.moveRange)
        {
            return tile.parent;
        }

        TileBehaviour endTile = null;
        for(int i = 0; i <= character.moveRange; i++)
        {
            endTile = tempPath.Pop();
        }

        return endTile;
    }

    //The real A*
    //The previous methods were all just helper functions
    public void EnemyFindPath(TileBehaviour target)
    { 
        //ComputeNeighboringTiles();
        //GetCurrentTile();
        FindSelectableTiles();

        List<TileBehaviour> openList = new List<TileBehaviour>(); //tiles to visit
        List<TileBehaviour> closedList = new List<TileBehaviour>(); //tiles already visited and processed

        openList.Add(currentTile);
        Debug.Log(target);
        Debug.Log(currentTile);
        currentTile.costFromProcessedTileToTargetTile = Vector2.Distance(currentTile.transform.position, target.transform.position);
        currentTile.totalCost = currentTile.costFromProcessedTileToTargetTile;
        //Debug.Log(target.name + " NAME OF THE TARGET");
        while(openList.Count > 0)
        {
            
            TileBehaviour tile = FindLowestTotalCost(openList); //finds next tile with lowest cost
            //Debug.Log(tile.name);
            closedList.Add(tile); //we looking at it now, so it shouldn't be looked at ever again
            if(tile == target)
            {
                //Debug.Log("LET ME IN");
                //We've found our destination and now time to get there
                targetTile = FindEndTile(tile);
                FindPath(targetTile);
                return;
            }
            foreach(TileBehaviour t in tile.neighbours)
            {
                if(closedList.Contains(t))
                {
                    //LOL don't do anything but we need to make it not enter the next if statement
                }
                else if (openList.Contains(t))
                {
                    float tempGValue = t.costFromParentToCurrentTile + Vector2.Distance(t.transform.position, tile.transform.position);
                    if(tempGValue < t.costFromParentToCurrentTile)
                    {
                        t.parent = tile;

                        t.costFromParentToCurrentTile = tempGValue;
                        t.totalCost = t.costFromParentToCurrentTile + t.costFromProcessedTileToTargetTile;
                    }
                }
                else
                {
                    t.parent = tile;
                    t.costFromParentToCurrentTile += Vector2.Distance(t.transform.position, tile.transform.position);
                    t.costFromProcessedTileToTargetTile = Vector2.Distance(t.transform.position, target.transform.position);
                    t.totalCost = t.costFromParentToCurrentTile + t.costFromProcessedTileToTargetTile;

                    openList.Add(t);
                }
            }
        }
    }

    //Finds the nearest target to move to.
    public GameObject FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        //Finding the weakest, most damaged, does special effective damage, etc all goes here
        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach(GameObject unit in targets)
        {
            float d = Vector3.Distance(transform.position, unit.transform.position);
            if(d < distance)
            {
                distance = d;
                nearest = unit;
            }
        }

        return nearest;
    }

    //This is for displaying movement range
    public void DisplayRange(bool toActivate, bool isAttackRange)
    {
        if(toActivate) //If activated, we highlight all the tiles
        {
            if (isAttackRange)
            {
                FindAttackableTiles();
                //Debug.Log("PIERRE IS PEPE");
            }
            else
            {
                FindSelectableTiles();//Finds all them delicious selectable tiles.

            }
            //We just call this to ensure that we get the selectable tiles.
            foreach(TileBehaviour tile in selectableTiles)
            {
                tile.setMask(isAttackRange, character.type);
            }
        }
        else
        {
            //We don't need to call FindSelectableTiles() cuz we've already found them once.
            foreach(TileBehaviour tile in selectableTiles)
            {
                tile.clearMask();
            }
        }

    }

    public void turnOnPanel()
    {
        canvas.gameObject.SetActive(true);
        if (hasAttacked)
        {
            canvas.transform.Find("Panel").transform.Find("attackButton").gameObject.GetComponent<Button>().interactable = false;
            //canvas.transform.GetComponentsInChildren<Button>().Interactable = false;
        }
        if (hasMoved)
        {
            canvas.transform.Find("Panel").transform.Find("moveButton").gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void turnOffPanel()
    {
        canvas.transform.Find("Panel").transform.Find("attackButton").gameObject.GetComponent<Button>().interactable = true;
        canvas.transform.Find("Panel").transform.Find("moveButton").gameObject.GetComponent<Button>().interactable = true;
        canvas.gameObject.SetActive(false);
    }
}

