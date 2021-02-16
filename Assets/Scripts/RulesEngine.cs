using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesEngine : MonoBehaviour
{
    //This will handle gameplay and rules while to player is in a level
    // Start is called before the first frame update
    private CharacterMovement selected;
    private List<CharacterMovement> enemyList;
    private List<CharacterMovement> friendlyList;
    private List<CharacterMovement> NPCList;
    //private Board board;
    private Character.Type activeTeam;
    private bool moving;
    private List<CharacterMovement> activeList;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This is called from GameManager and sets up all the units and where they go calls board to draw the map. 
    public void init(List<CharacterMovement> enemies, List<CharacterMovement> friendlies, List<CharacterMovement> NPCs, GameObject[] map)//, Map1 map, TileBehaviour tilePrefab)
    {
        friendlyList = friendlies;
        enemyList = enemies;
        NPCList = NPCs;
        /*
        board = FindObjectOfType<Board>();
        board.init(map, tilePrefab);
        board.deselectCharacter.AddListener(deselectCharacter);
        board.moveCharacter.AddListener(moveFriendly);
        */
        activeList = new List<CharacterMovement>();
        for ( int i = 0; i < friendlies.Count ;i++) // && i < map.friendlySpawnPoints.Count
        {
            spawnCharacter(friendlies[i]);//, map.friendlySpawnPoints[i]);
            activeList.Add(friendlies[i]);
        }
        for( int i = 0; i < enemies.Count; i++)// && i < map.enemySpawnPoints.Count
        {
            spawnCharacter(enemies[i]);//, map.enemySpawnPoints[i]);
        }
        foreach(GameObject tile in map)
        {
            tile.GetComponent<TileBehaviour>().clicked.AddListener(moveFriendly);
        }
       
    }

    //Helper function to spawn in characters and setup event listeners for those characters given a character object and a position
    private void spawnCharacter(CharacterMovement character)//, Vector2Int position)
    {
        //TODO: Spawning Character needs to be fixed slightly. Through set spawn points on the map :D
        character.clicked.AddListener(unitClicked);
        //character.doneMoving.AddListener(doneMoving);
        /*
        character.transform.position = new Vector3(position.x, position.y, -1);
        character.position = position;
        */
        //TODO: Change this to RaycastHit2D
        //board.getTerrainTile(position).occupied = character;
    }

    //Handles which turn it is. Is called by DoneMoving. Turn cycle is Friendly>Enemy>NPC>Friendly atm. This should probably be Friendly>NPC>Enemy>Friendly.
    private void toggleTurn()
    {
        if(activeTeam == Character.Type.FRIENDLY)
        {
            activeTeam = Character.Type.ENEMY;
            loadActiveList(enemyList);
        }else if(activeTeam == Character.Type.ENEMY)
        {
            if(NPCList != null && NPCList.Count != 0)
            {
                activeTeam = Character.Type.NPC;
                loadActiveList(NPCList);
            }
            else
            {
                activeTeam = Character.Type.FRIENDLY;
                loadActiveList(friendlyList);
            }
        }
        else {
            activeTeam = Character.Type.FRIENDLY;
            loadActiveList(friendlyList);
        }
    }
    //Handles enemy ai turn movement. Wrapper function for moveCharacter
    //This will function might cause infinite recursion if enemylist is empty atm. This shouldnt be a problem in the future because the game should end if there are no enemys remaining so I am not fixing this.
    private void enemyTurn()
    {
        if (activeList.Count != 0)
        {
            activeList[0].EnemyFindPath(activeList[0].FindNearestTarget().GetComponent<CharacterMovement>().currentTile);
            moveCharacter(activeList[0]); //I assume this always gets the first character since 
        }
        else
        {

            toggleTurn(); //This should never be called but is a fail safe
        }

    }

    //Helper function that is called from toggleTurn() that loads whichever teams units are going to be active next
    private void loadActiveList(List<CharacterMovement> load)
    {
        foreach (var unit in load)
        {
            activeList.Add(unit);
        }
    }

    //Wrapper function for moveCharacter that handles friendly unit movement from player input is called by board event
    private void moveFriendly(TileBehaviour target)//PathToTile path)
    {
        if(selected)
        {
            if (selected.character.type == Character.Type.FRIENDLY && target.selectable) 
            {
                //moveCharacter(path, selected);
                selected.FindPath(target);
                moveCharacter(selected);
            }
            deselectCharacter();
        }
    }
    //Is called by moveFriendly() and enemyTurn() 
    private void moveCharacter(CharacterMovement character)//PathToTile path, 
    {
        moving = true;
        deoccupyTile(character.currentTile);
        //character.move(path);
        //Move function is called here.
        character.Move();
        occupyTile(character.currentTile, character);
        //TODO: The character should be able to make an action after moving.
        activeList.Remove(character);
        doneMoving();
        //Debug.Log("Number of active units remaining: " + activeList.Count);
    }

    //Helper function that sets what unit is occupying a tile called by moveCharacter()
    private void occupyTile(TileBehaviour tile, CharacterMovement character)
    {
        tile.occupied = character;
    }

    //helper function to deoccupy a tile called by moveCharacter()
    private void deoccupyTile(TileBehaviour tile)
    {
        tile.occupied = null;
    }

    //Event handler that is called by character when it stops moving.
    private void doneMoving()
    {
        moving = false;
        if (activeList.Count == 0)
        {
            toggleTurn();
        }
        if (activeTeam == Character.Type.ENEMY) //If active team is enemy calls for next enemy movement
        {
            enemyTurn();
        }
        
    }
    //Event handler that selects and deselects units calls select character and/or deselect character depending on the scenario
    private void unitClicked(CharacterMovement character)
    {
        //Debug.Log(character + " has been clicked");
        if (activeTeam == Character.Type.FRIENDLY && !moving)
        {
            if (selected == null) //if not unit is selected select unit clicked
            {
                selectCharacter(character);
            } 
            else if (selected == character) //if unit clicked is currently selected deselect it
            {
                deselectCharacter();
            }
            else //if unit clicked is not selected and there is already a unit selected. Deselect old unit and select new unit
            {
                deselectCharacter();
                selectCharacter(character);
            }
        }
    }

    //Helper function called from unitClicked()
    private void selectCharacter(CharacterMovement character)
    {
        selected = character;
        selected.DisplayMovementRange(true);
        //board.showMoveRange(character);
    }

    //helper function called from unitClicked(), moveFriendly(), and board.tileClicked()
    private void deselectCharacter()
    {
        selected.DisplayMovementRange(false);
        selected = null;
        //board.clearMoveRange();
    }
}
