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
    private bool moving = false;
    private bool attacking = false;
    private List<CharacterMovement> activeList;

    private GameObject inventoryUI;
    private GameObject characterData;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This is called from GameManager and sets up all the units and where they go calls board to draw the map. 
    public void init(List<CharacterMovement> enemies, List<CharacterMovement> friendlies, List<CharacterMovement> NPCs, GameObject[] map, GameObject inventoryUI,
        GameObject characterData)//, Map1 map, TileBehaviour tilePrefab)
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

        this.inventoryUI = inventoryUI;
        this.characterData = characterData;
    }

    //Helper function to spawn in characters and setup event listeners for those characters given a character object and a position
    private void spawnCharacter(CharacterMovement character)//, Vector2Int position)
    {
        //TODO: Spawning Character needs to be fixed slightly. Through set spawn points on the map :D
        character.clicked.AddListener(unitClicked);
        character.passTurn.AddListener(passTurn);
        character.doneMoving.AddListener(doneMoving);
        character.unitAttacking.AddListener(unitAttacking);
        character.openInventory.AddListener(OpenInventoryMenu);
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
            foreach(CharacterMovement friend in friendlyList)
            {
                //This resets the temporary animation stuff
                friend.ResetTemp();
            }
            loadActiveList(enemyList);
            foreach(CharacterMovement enemy in enemyList)
            {
                //This resets all values of the current team's turn!
                enemy.ResetTurn();
            }
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
                foreach(CharacterMovement enemy in enemyList)
                {
                    enemy.ResetTemp();
                }
                loadActiveList(friendlyList);
                foreach(CharacterMovement friend in friendlyList)
                {
                    //This resets the temporary animation stuff
                    friend.ResetTurn();
                }
            }
        }
        else {
            activeTeam = Character.Type.FRIENDLY;
            loadActiveList(friendlyList);
        }
        //Debug.Log(activeTeam);
    }
    //Handles enemy ai turn movement. Wrapper function for moveCharacter
    //This will function might cause infinite recursion if enemylist is empty atm. This shouldnt be a problem in the future because the game should end if there are no enemys remaining so I am not fixing this.
    private void enemyTurn()
    {
        if (activeList.Count != 0)
        {
            selectCharacter(activeList[0]);
            TileBehaviour target = activeList[0].FindNearestTarget().GetComponent<CharacterMovement>().currentTile;
            activeList[0].FindAttackableTiles();
            if(activeList[0].attackableList.Count > 0 && activeList[0].attackableList.Contains(target.occupied))
            {
                Debug.Log(target.occupied);
                attackCharacter(target.occupied);
            }
            else
            {
                activeList[0].EnemyFindPath(target);
                //Debug.Log(activeList[0].name);
                moveCharacter(activeList[0]); //I assume this always gets the first character since 
            }
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
            if (!attacking && selected.character.type == Character.Type.FRIENDLY && target.selectable) 
            {
                //moveCharacter(path, selected);
                selected.FindPath(target);
                moveCharacter(selected);
            }
            //deselectCharacter();
        }
    }
    //Is called by moveFriendly() and enemyTurn() 
    private void moveCharacter(CharacterMovement character)//PathToTile path, 
    {
        moving = true;
        deoccupyTile(character.currentTile);
        //character.move(path);
        //Move function is called here.
        activeList.Remove(character); //This is in this order to make sure that the character is removed from active list before doneMoving() is called
        character.Move();
        //Debug.Log("Activelist count is " + activeList.Count);
        //TODO: The character should be able to make an action after moving.
        
        
        //Debug.Log("Number of active units remaining: " + activeList.Count);
    }

    //Helper function that sets what unit is occupying a tile called by moveCharacter()
    public void occupyTile(TileBehaviour tile, CharacterMovement character)
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
        selected._sprite.color = Color.gray;
        selected._sprite.flipX = selected.baseFlipState;
        //This stays in "Selected" state for some god awful reason because EVERYTHING HAPPENS IN A SINGLE FRAME. Thus there is no time for the animation to change back. -____-
        //This should be fixed upon a true end turn, but elsewise, it's going to be sadge for now.
        selected._animator.enabled = false;
        deselectCharacter();
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
            else if (attacking && selected.attackableList.Contains(character))
            {
                Debug.Log(character.name + " is being attacked");
                attackCharacter(character);
            }
            else //if unit clicked is not selected and there is already a unit selected. Deselect old unit and select new unit
            {
                deselectCharacter();
                selectCharacter(character);
            }
        }
    }

    private void passTurn(CharacterMovement character)
    {
        activeList.Remove(character);
        //deselectCharacter();
        doneMoving();
        /*
        if (activeList.Count == 0)
        {
            doneMoving();
        }
        */
    }

    private void unitAttacking(CharacterMovement character)
    {
        attacking = true;

    }

    private void attackCharacter(CharacterMovement character)
    {
        character.currHP -= selected.inventory.equippedWeapon.might;
        Debug.Log(character.name + " has " + character.currHP + " HP left");
        if(character.currHP <= 0)
        {
            KillUnit(character);
        }
        selected.inventory.equippedWeapon.uses--;//This should go down everytime the unit attacks
        Debug.Log(selected.inventory.equippedWeapon.uses); 
        activeList.Remove(selected);
        doneMoving();
    }

    private void KillUnit(CharacterMovement character)
    {
        if(character.character.type == Character.Type.FRIENDLY)
        {
            friendlyList.Remove(character);
        }
        else if (character.character.type == Character.Type.ENEMY)
        {
            enemyList.Remove(character);
        }
        else
        {
            NPCList.Remove(character);
        }
        deoccupyTile(character.currentTile);
        character.gameObject.SetActive(false);
    }

    private void OpenInventoryMenu(CharacterMovement character)
    {
        inventoryUI.SetActive(true);
        inventoryUI.transform.GetChild(0).gameObject.GetComponent<InventoryMenu>().OpenInventoryUIMenu(selected, selected.inventory);
    }

    private void OpenCharacterData(CharacterMovement character)
    {
        characterData.SetActive(true);
        characterData.transform.GetChild(0).gameObject.GetComponent<CharacterDataUI>().OpenCharacterUI(character);
    }

    private void CloseCharacterData()
    {
        characterData.SetActive(false);
    }

    //Helper function called from unitClicked()
    private void selectCharacter(CharacterMovement character)
    {
        selected = character;
        if (character.character.type == Character.Type.FRIENDLY && activeList.Contains(character)) //if friendly character with actions left show action panel
        {
            OpenCharacterData(selected);
            selected._animator.SetBool("selected", true);
            character.canvas.gameObject.SetActive(true);
        }
        else if (character.character.type == Character.Type.ENEMY && activeTeam != Character.Type.ENEMY) //else if enemy show move range
        {
            OpenCharacterData(selected);
            selected.DisplayRange(true, false);
        }
        //board.showMoveRange(character);
    }

    //helper function called from unitClicked(), and doneMoving()
    private void deselectCharacter()
    {
        CloseCharacterData();
        selected.canvas.gameObject.SetActive(false);
        selected.DisplayRange(false, false);
        selected._animator.SetBool("selected", false);
        selected._animator.Play("Idle", 0, 0f);
        attacking = false;
        selected = null;
        //board.clearMoveRange();
    }
}
