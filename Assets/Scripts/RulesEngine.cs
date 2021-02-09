using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesEngine : MonoBehaviour
{
    //This will handle gameplay and rules while to player is in a level
    // Start is called before the first frame update
    private Character selected;
    private List<Character> enemyList;
    private List<Character> friendlyList;
    private List<Character> NPCList;
    private Board board;
    private Character.Type activeTeam;
    private bool moving;
    private List<Character> activeList;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This is called from GameManager and sets up all the units and where they go calls board to draw the map. 
    public void init(List<Character> enemies, List<Character> friendlies, List<Character> NPCs, Map1 map, Tile tilePrefab){
        friendlyList = friendlies;
        enemyList = enemies;
        NPCList = NPCs;
        board = FindObjectOfType<Board>();
        board.init(map, tilePrefab);
        board.deselectCharacter.AddListener(deselectCharacter);
        board.moveCharacter.AddListener(moveFriendly);
        activeList = new List<Character>();
        for ( int i = 0; i < friendlies.Count && i < map.friendlySpawnPoints.Count;i++)
        {
            spawnCharacter(friendlies[i], map.friendlySpawnPoints[i]);
            activeList.Add(friendlies[i]);
        }
        for( int i = 0; i < enemies.Count && i < map.enemySpawnPoints.Count; i++)
        {
            spawnCharacter(enemies[i], map.enemySpawnPoints[i]);
        }
       
    }

    //Helper function to spawn in characters and setup event listeners for those characters given a character object and a position
    private void spawnCharacter(Character character, Vector2Int position)
    {
        character.clicked.AddListener(unitClicked);
        character.doneMoving.AddListener(doneMoving);
        character.transform.position = new Vector3(position.x, position.y, -1);
        character.position = position;
        board.getTerrainTile(position).occupied = character;
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
            moveCharacter(board.generateEnemyPath(activeList[0]), activeList[0]);
        }
        else
        {

            toggleTurn(); //This should never be called but is a fail safe
        }

    }

    //Helper function that is called from toggleTurn() that loads whichever teams units are going to be active next
    private void loadActiveList(List<Character> load)
    {
        foreach (var unit in load)
        {
            activeList.Add(unit);
        }
    }

    //Wrapper function for moveCharacter that handles friendly unit movement from player input is called by board event
    private void moveFriendly(PathToTile path)
    {
        if (selected.type == Character.Type.FRIENDLY) {
            moveCharacter(path, selected);
        }
        deselectCharacter();
    }
    //Is called by moveFriendly() and enemyTurn() 
    private void moveCharacter(PathToTile path, Character character)
    {
        moving = true;
        deoccupyTile(board.getTerrainTile(character.position));
        character.move(path);
        occupyTile(board.getTerrainTile(path.tile), character);
        activeList.Remove(character);
        
    }

    //Helper function that sets what unit is occupying a tile called by moveCharacter()
    private void occupyTile(Tile tile, Character character)
    {
        tile.occupied = character;
    }

    //helper function to deoccupy a tile called by moveCharacter()
    private void deoccupyTile(Tile tile)
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
    private void unitClicked(Character character)
    {
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
    private void selectCharacter(Character character)
    {
        selected = character;
        board.showMoveRange(character);
    }

    //helper function called from unitClicked(), moveFriendly(), and board.tileClicked()
    private void deselectCharacter()
    {
        selected = null;
        board.clearMoveRange();
    }
}
