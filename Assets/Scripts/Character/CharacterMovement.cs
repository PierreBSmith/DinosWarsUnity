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
    private int currentStamina;
    private const int LINEAR_STAMINA_DEPLETION = 10;
    private int extraMovementRange;
    [Header("Movement")]
    private List<TileBehaviour> selectableTiles = new List<TileBehaviour>(); //the list of tiles that can be moved to
    private GameObject[] tiles; //stores all tiles of the map
    private Stack<TileBehaviour> path = new Stack<TileBehaviour>(); //the actual path the character wishes to travel along
    [HideInInspector]
    public TileBehaviour currentTile; //the tile the unit is currently inhabiting
    private TileBehaviour targetTile;
    private Vector3 velocity = new Vector3(); //the speed the unit is moving
    private Vector3 heading = new Vector3(); //the direction the unit is moving
    private const float HEIGHT_OF_UNIT_ABOVE_TILE = 0.5f;
    private int currHP;

    public Vector2Int position; //This might not need to be here
    public CharacterEvent clicked; //Event for when Character is clicked. Is handled by RulesEngine
    public UnityEvent doneMoving; //Event for when Character has stopped moving after a movement command. Is handled by RulesEngine

    void Start()
    {
        currentStamina = character.maxStamina;
        GetExtraRange();
        //unfortunately would have to call this in Update if we decided to make a map with disappearing tiles LMAO
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        GetCurrentTile();
    }

    //This starts movement along the given path
    /*
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
        Vector2 target = actualPath.Dequeue(); //We need to figure out how to gradually increase stamina intake depending on distance.
        while (true)
        { 
            Vector2 dist = target - (Vector2)transform.position;
            print(character.speed);
            if (dist.magnitude > 0.05)
            {
                //I assume this is where the unit records that it has moved one tile?
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
    */
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

    //CHARACTER RESET FUNCTION. PLEASE CALL BEFORE THE START OF THE PLAYER PHASE!!!!!!!!!!!!!
    public void ResetCharacter()
    {
        currentStamina = character.maxStamina;
    }

    private void GetExtraRange()
    {
        int normalStaminaUsage = character.moveRange * LINEAR_STAMINA_DEPLETION;
        int maxStaminaUsage = normalStaminaUsage;
        extraMovementRange = 0;
        while(maxStaminaUsage < character.maxStamina)
        {
            extraMovementRange++;
            maxStaminaUsage += (extraMovementRange + 1) * LINEAR_STAMINA_DEPLETION;
            if(maxStaminaUsage > character.maxStamina)
            {
                extraMovementRange--;
                break;
            }
        }
        Debug.Log(extraMovementRange);
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
            if(tile.distance <= (character.moveRange + extraMovementRange) && !tile.hasUnit)
            {
                //These checks are for stamina usage stuff
                if(tile.distance <= character.moveRange)
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
        while(nextTile != null)
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

    protected void RemoveSelectableTiles()
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
        while(path.Count > 0)
        {
            TileBehaviour moveTarget = path.Peek();
            Vector3 targetPosition = moveTarget.transform.position;
            targetPosition.z -= HEIGHT_OF_UNIT_ABOVE_TILE;
        
            //Calculates Unit's position
            if(Vector2.Distance(transform.position, targetPosition) >= 0.05f)
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
                //Stamina stuff here. For now it's going to be linear since we have no way of getting extra movement range yet.
                if(moveTarget.distance > character.moveRange)
                {
                    int extraDistance = (moveTarget.distance - character.moveRange) + 1;
                    currentStamina -= extraDistance * LINEAR_STAMINA_DEPLETION;
                }
                else
                {
                    currentStamina -= LINEAR_STAMINA_DEPLETION;
                }
                path.Pop();
            }
        }
        DisplayMovementRange(false);
        RemoveSelectableTiles();
        GetCurrentTile();
        //doneMoving.Invoke();
    }

    //Now the fun stuff. Pathfinding for AI :D
    //A* time
    protected TileBehaviour FindLowestTotalCost(List<TileBehaviour> list)
    {
        TileBehaviour lowest = list[0];
        foreach(TileBehaviour tile in list)
        {
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
        ComputeNeighboringTiles();
        GetCurrentTile();
        FindSelectableTiles();

        List<TileBehaviour> openList = new List<TileBehaviour>(); //tiles to visit
        List<TileBehaviour> closedList = new List<TileBehaviour>(); //tiles already visited and processed

        openList.Add(currentTile);
        currentTile.costFromProcessedTileToTargetTile = Vector2.Distance(currentTile.transform.position, target.transform.position);
        currentTile.totalCost = currentTile.costFromProcessedTileToTargetTile;

        while(openList.Count > 0)
        {
            TileBehaviour tile = FindLowestTotalCost(openList); //finds next tile with lowest cost
            closedList.Add(tile); //we looking at it now, so it shouldn't be looked at ever again
            if(tile == target)
            {
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
    public void DisplayMovementRange(bool toActivate)
    {
        if(toActivate) //If activated, we highlight all the tiles
        {
            FindSelectableTiles();//Finds all them delicious selectable tiles.
            //We just call this to ensure that we get the selectable tiles.
            foreach(TileBehaviour tile in selectableTiles)
            {
                tile.setMask(false, Character.Type.FRIENDLY);
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
}

