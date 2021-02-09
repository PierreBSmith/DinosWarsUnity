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

    //The 
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

    private void spawnCharacter(Character character, Vector2Int position)
    {
        character.clicked.AddListener(unitClicked);
        character.doneMoving.AddListener(doneMoving);
        character.transform.position = new Vector3(position.x, position.y, -1);
        character.position = position;
        board.getTerrainTile(position).occupied = character;
    }

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
    private void enemyTurn()
    {
        if (activeList.Count != 0)
        {
            moveCharacter(board.generateEnemyPath(activeList[0]), activeList[0]);
        }
        else
        {
            //Beware of infinite recursion
            toggleTurn();
        }

    }
    private void loadActiveList(List<Character> load)
    {
        foreach (var unit in load)
        {
            activeList.Add(unit);
        }
    }
    private void moveFriendly(PathToTile path)
    {
        if (selected.type == Character.Type.FRIENDLY) {
            moveCharacter(path, selected);
        }
        deselectCharacter();
    }
    private void moveCharacter(PathToTile path, Character character)
    {
        moving = true;
        deoccupyTile(board.getTerrainTile(character.position));
        character.move(path);
        occupyTile(board.getTerrainTile(path.tile), character);
        activeList.Remove(character);
        
    }
    private void occupyTile(Tile tile, Character character)
    {
        tile.occupied = character;
    }

    private void deoccupyTile(Tile tile)
    {
        tile.occupied = null;
    }
    private void doneMoving()
    {

        moving = false;
        if (activeList.Count == 0)
        {
            toggleTurn();
        }
        if (activeTeam == Character.Type.ENEMY)
        {
            enemyTurn();
        }
        
    }
    private void unitClicked(Character character)
    {
        if (activeTeam == Character.Type.FRIENDLY && activeList.Contains(character) && !moving)
        {
            if (selected == null)
            {
                selectCharacter(character);
            }
            else if (selected == character)
            {
                deselectCharacter();
            }
            else
            {
                deselectCharacter();
                selectCharacter(character);
            }
        }
    }

    private void selectCharacter(Character character)
    {
        selected = character;
        board.showMoveRange(character);
    }

    private void deselectCharacter()
    {
        selected = null;
        board.clearMoveRange();
    }
}
