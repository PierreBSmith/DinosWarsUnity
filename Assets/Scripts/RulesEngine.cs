using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesEngine : MonoBehaviour
{
    //This will handle gameplay and rules while to player is in a level
    private CharacterMovement selected;
    private List<CharacterMovement> enemyList;
    private List<CharacterMovement> friendlyList;
    private List<CharacterMovement> NPCList;
    //private Board board;
    private Character.Type activeTeam;
    private bool moving = false;
    private bool attacking = false;
    private List<CharacterMovement> activeList;

    private GameObject actionMenuUI;
    private GameObject inventoryUI;
    private GameObject characterData;
    private GameObject combatForecastUI;
    private GameObject tileInfoUI;
    private GameObject healUI;
    public GameObject endGame;
    private RaycastHit2D hover;
    [SerializeField]
    private Camera playerCamera;

    private Combat _combatManager;

    void Start()
    {
        _combatManager = GetComponent<Combat>();
    }

    // Update is called once per frame
    void Update()
    {
        hover = Physics2D.Raycast(new Vector2(playerCamera.ScreenToWorldPoint(Input.mousePosition).x, playerCamera.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);
        if(hover)
        {
            if(attacking)
            {
                if(selected.attackableList.Contains(hover.collider.gameObject.GetComponent<CharacterMovement>()))
                {
                    if(selected.inventory.equippedWeapon.weaponType == Item.WEAPON.SPIRIT)
                    {
                        OpenHealUI(selected, hover.collider.gameObject.GetComponent<CharacterMovement>());
                    }
                    else
                    {
                        OpenCombatForecast(selected, hover.collider.gameObject.GetComponent<CharacterMovement>());
                    }
                }
                else
                {
                    CloseCombatForecast();
                    CloseHealUI();
                }
            }
            else
            {
                CloseCombatForecast();
                CloseHealUI();
                if(hover.collider.gameObject.tag == "Player" || hover.collider.gameObject.tag == "Enemy")
                {
                    //if the mouse hovers over a unit
                    OpenCharacterData(hover.collider.gameObject.GetComponent<CharacterMovement>());
                    OpenTileUI(hover.collider.gameObject.GetComponent<CharacterMovement>().currentTile.tile);
                }
                else if (hover.collider.gameObject.tag == "Tile")
                {
                    CloseCharacterData();
                    OpenTileUI(hover.collider.gameObject.GetComponent<TileBehaviour>().tile);
                }
            }
        }
        else
        {
            //This is absolutely making sure everything closes out!
            CloseCharacterData();
            CloseTileUI();
            CloseCombatForecast();
            CloseHealUI();
        }
    }

    //This is called from GameManager and sets up all the units and where they go calls board to draw the map. 
    public void init(List<CharacterMovement> enemies, List<CharacterMovement> friendlies, List<CharacterMovement> NPCs, GameObject[] map, GameObject inventoryUI,
        GameObject characterData, GameObject combatForecastUI, GameObject tileInfoUI, GameObject healUI, GameObject actionMenuUI)//, Map1 map, TileBehaviour tilePrefab)
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

        this.actionMenuUI = actionMenuUI;
        this.inventoryUI = inventoryUI;
        this.characterData = characterData;
        this.combatForecastUI = combatForecastUI;
        this.tileInfoUI = tileInfoUI;
        this.healUI = healUI;
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
        if(friendlyList.Count == 0)
        {

        }else if(enemyList.Count == 0)
        {

        }
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
            //Debug.Log(target);
            activeList[0].FindAttackableTiles();
            //Debug.Log("lolol " + activeList[0].hasAttacked + " " + activeList[0].hasMoved);
            if(!activeList[0].hasAttacked && activeList[0].attackableList.Count > 0 && activeList[0].attackableList.Contains(target.occupied))
            {
                //Debug.Log(target.occupied);
                attackCharacter(target.occupied, target.distance);
            }
            else if(!activeList[0].hasMoved && !activeList[0].character.isBoss)
            {
                activeList[0].EnemyFindPath(target);
                //Debug.Log(activeList[0].name);
                moveCharacter(activeList[0]); //I assume this always gets the first character since 
            }
            else
            {
                characterDone(activeList[0]);
                doneMoving();
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
            if (!selected.hasMoved && !attacking && selected.character.type == Character.Type.FRIENDLY && target.selectable) 
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
        character.hasMoved = true;
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

    private void characterDone(CharacterMovement character)
    {
        //Debug.Log(character.gameObject.name);
        selected = character;
        activeList.Remove(character);
        if(character._sprite)
        {
            character._sprite.color = Color.gray;
        }
        else
        {
            foreach(SpriteRenderer sprite in character._theRealSprites)
            {
                sprite.color = Color.gray;
            }
        }
        character._animator.enabled = false;
    }
    //Event handler that is called by character when it stops moving.
    private void doneMoving()
    {
        moving = false;

        if(selected._sprite)
        {
            selected._sprite.flipX = selected.baseFlipState;
        }
        else
        {
            foreach(SpriteRenderer sprite in selected._theRealSprites)
            {
                sprite.flipX = selected.baseFlipState;
            }
        }
        if (selected && selected.currentStamina <= 0)
        {
            characterDone(selected);
        }
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
        //Debug.Log("STAMINA " + character.currentStamina);
        //Debug.Log(character.gameObject.transform.position + " has been clicked");
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
                attackCharacter(character, character.currentTile.distance);
            }
            else //if unit clicked is not selected and there is already a unit selected. Deselect old unit and select new unit
            {
                deselectCharacter();
                selectCharacter(character);
            }
        }
    }

    public void passTurn(CharacterMovement character)
    {
        characterDone(character);
        doneMoving();
    }

    public void unitAttacking(CharacterMovement character)
    {
        attacking = true;
        characterData.SetActive(false);
    }

    private void attackCharacter(CharacterMovement character, int range)
    {
        if(_combatManager.CombatExchange(selected, character, range))
        {
            if(selected.currHP <= 0)
            {
                KillUnit(selected);
            }
            else
            {
                KillUnit(character);
            }
        }
        attacking = false;
        selected.hasAttacked = true;
        //activeList.Remove(selected);
        doneMoving();
    }


    private void KillUnit(CharacterMovement character)
    {
        if(character.character.type == Character.Type.FRIENDLY)
        {
            friendlyList.Remove(character);
            StartCoroutine(endOfGame(character));
            
        }
        else if (character.character.type == Character.Type.ENEMY)
        {
            enemyList.Remove(character);
            StartCoroutine(endOfGame(character));
        }
        else
        {
            NPCList.Remove(character);
        }
        deoccupyTile(character.currentTile);
        character.gameObject.SetActive(false);
    }
    private IEnumerator endOfGame(CharacterMovement character)
    {
        if (character.character.characterName == "Asku" || character.character.characterName == "Tatam")
        {
            endGame.gameObject.SetActive(true);
            endGame.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "You Lose";
            yield return new WaitForSeconds(seconds: 5);
            Application.Quit();
        }
        if (enemyList.Count == 0)
        {
            endGame.gameObject.SetActive(true);
            endGame.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "You Win";
            yield return new WaitForSeconds(seconds: 5);
            Application.Quit();
        }

    }
    public void OpenInventoryMenu(CharacterMovement character)
    {
        CloseActionUI();
        inventoryUI.SetActive(true);
        inventoryUI.transform.GetChild(0).gameObject.GetComponent<InventoryMenu>().OpenInventoryUIMenu(selected, selected.inventory);
    }

    private void CloseInventoryMenu()
    {
        inventoryUI.transform.GetChild(0).gameObject.GetComponent<InventoryMenu>().CloseInventoryUI();
        selected.usedInventory = CheckIfInventoryWasUsed();
    }

    private bool CheckIfInventoryWasUsed()
    {
        return inventoryUI.transform.GetChild(0).gameObject.GetComponent<InventoryMenu>().used;
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

    private void OpenTileUI(Tile tile)
    {
        tileInfoUI.SetActive(true);
        tileInfoUI.transform.GetChild(0).gameObject.GetComponent<TileInfoUI>().OpenTileInfoUI(tile);
    }

    private void CloseTileUI()
    {
        tileInfoUI.SetActive(false);
    }

    private void OpenCombatForecast(CharacterMovement player, CharacterMovement enemy)
    {
        combatForecastUI.SetActive(true);
        combatForecastUI.transform.GetChild(0).GetComponent<CombatForcastUI>().OpenMenu(selected, enemy,
                                                                                                        _combatManager.GetDamageDealt(selected, enemy),
                                                                                                        _combatManager.GetAttackSpeed(selected) - _combatManager.GetAttackSpeed(enemy) >= 5 ? true : false,
                                                                                                        _combatManager.GetHitRate(selected), _combatManager.GetCritChance(selected, enemy),
                                                                                                        _combatManager.GetDamageDealt(enemy, selected),
                                                                                                        _combatManager.GetAttackSpeed(enemy) - _combatManager.GetAttackSpeed(selected) >= 5 ? true : false,
                                                                                                        _combatManager.GetHitRate(enemy),
                                                                                                        _combatManager.GetCritChance(enemy, selected));
    }

    private void CloseCombatForecast()
    {
        combatForecastUI.SetActive(false);
    }

    private void OpenHealUI(CharacterMovement user, CharacterMovement target)
    {
        healUI.SetActive(true);
        healUI.transform.GetChild(0).GetComponent<HealUI>().OpenHealUI(user, target);
    }

    private void CloseHealUI()
    {
        healUI.SetActive(false);
    }

    private void OpenActionUI(CharacterMovement character)
    {
        actionMenuUI.SetActive(true);
        actionMenuUI.transform.GetChild(0).GetComponent<ActionMenuUI>().OpenActionMenu(character);
    }

    private void CloseActionUI()
    {
        actionMenuUI.transform.GetChild(0).GetComponent<ActionMenuUI>().CloseActionMenu();
        //actionMenuUI.SetActive(false);
    }

    //Helper function called from unitClicked()
    private void selectCharacter(CharacterMovement character)
    {
        selected = character;
        if (character.character.type == Character.Type.FRIENDLY && activeList.Contains(character)) //if friendly character with actions left show action panel
        {
            character.RemoveSelectableTiles();
            selected._animator.SetBool("selected", true);
            //Vector3 screenPos = Camera.main.WorldToScreenPoint(character.transform.position) / 64;
            //selected.turnOnPanel(screenPos);
            OpenActionUI(selected);
        }
        else if (character.character.type == Character.Type.ENEMY && activeTeam != Character.Type.ENEMY) //else if enemy show move range
        {
            selected._animator.SetBool("selected", true);
            selected.DisplayRange(true, false, false);
        }
        //board.showMoveRange(character);
    }

    //helper function called from unitClicked(), and doneMoving()
    private void deselectCharacter()
    {
        //selected.turnOffPanel();
        if(selected.character.type == Character.Type.FRIENDLY && actionMenuUI.active)
        {
            CloseActionUI();
            CloseInventoryMenu();
        }
        selected.DisplayRange(false, false, false);
        selected._animator.SetBool("selected", false);
        selected._animator.Play("Idle", 0, 0f);
        attacking = false;
        selected = null;
        
        //board.clearMoveRange();
    }
}
