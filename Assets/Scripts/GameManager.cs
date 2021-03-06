﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    //Game Manager that is always running in the background and handles transferring information between scenes
    // Start is called before the first frame update
    public bool inLevel = false;

    //[HideInInspector]
    public int currentLevel = 1;
    [HideInInspector]
    public LevelButton.LEVEL choosenLevel;

    //public TileBehaviour tilePrefab;
    private GameObject[] tiles;
    [Header("UI Menus")]
    [SerializeField]
    private GameObject actionMenuUIPrefab;
    private GameObject actionMenuUI;
    [SerializeField]
    private GameObject InventoryUIPrefab;
    private GameObject inventoryUI;
    [SerializeField]
    private GameObject CharacterDataUIPrefab;
    private GameObject characterDataUI;
    [SerializeField]
    private GameObject combatForecastUIPrefab;
    private GameObject combatForecastUI;
    [SerializeField]
    private GameObject tileInfoUIPrefab;
    private GameObject tileInfoUI;
    [SerializeField]
    private GameObject healUIPrefab;
    private GameObject healUI;
    [SerializeField]
    private GameObject combatNumberPrefab;
    private GameObject combatNumber;
    [SerializeField]
    private GameObject levelUpUIPrefab;
    private GameObject levelUpUI;
    [SerializeField]
    private GameObject statusMenuPrefab;
    private GameObject statusMenu;
    [SerializeField]
    private GameObject bottomMenuPrefab;
    private GameObject bottomMenu;

    [Header("Memory Scriptable Values")]
    public List<Item> allItems = new List<Item>();
    public List<CharacterMovement> friendlyUnits;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(inLevel)
        {
            InitializeLevel();
        }
        else
        {
            if(combatNumber)
            {
                combatNumber.SetActive(false);
            }
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeLevel()
    {
       /*
        //Initializes the map, friendly units, and enemy units so it can hand it to RulesEngine. Currently uses prefab references that are given to it in the Unity development client.
        Tile.Type[,] tileSet = new Tile.Type[24,16]; //Placeholder map that is all grass tiles, this can be any map theoretically
        for(int i = 0; i < tileSet.GetLength(0); i++){
            for(int j = 0; j < tileSet.GetLength(1); j++){
                tileSet[i,j] = Tile.Type.GRASS; 
            }
        }
        Map1 map = new Map1(tileSet, new List<Vector2Int>{new Vector2Int(3,3),new Vector2Int(5,3),new Vector2Int(3,5)},
                    new List<Vector2Int>{new Vector2Int(15,13),new Vector2Int(13,13),new Vector2Int(13,15)});
                    */
        GameObject[] locatingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        List<CharacterMovement> Enemies = new List<CharacterMovement>();
       
        foreach(GameObject enemy in locatingEnemies)
        {
            Enemies.Add(enemy.GetComponent<CharacterMovement>());//TODO: Don't forget to add the instantiate back
            /*
            if (Enemies.Count >= map.enemySpawnPoints.Count)
                break;
            */
        }
        GameObject[] locatingPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in locatingPlayer)
        {
            friendlyUnits.Add(player.GetComponent<CharacterMovement>()); //TODO:Don't forget to add the instantiate back
            /*
            if (Friends.Count >= map.friendlySpawnPoints.Count)
                break;
                */
        }
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        actionMenuUI = GameObject.Instantiate(actionMenuUIPrefab);
        actionMenuUI.SetActive(false);

        inventoryUI = GameObject.Instantiate(InventoryUIPrefab);
        inventoryUI.SetActive(false);

        characterDataUI = GameObject.Instantiate(CharacterDataUIPrefab);
        characterDataUI.SetActive(false);
        
        combatForecastUI = GameObject.Instantiate(combatForecastUIPrefab);
        combatForecastUI.SetActive(false);

        tileInfoUI = GameObject.Instantiate(tileInfoUIPrefab);
        tileInfoUI.SetActive(false);

        healUI = GameObject.Instantiate(healUIPrefab);
        healUI.SetActive(false);

        combatNumber = GameObject.Instantiate(combatNumberPrefab); 
        //combatNumber.SetActive(false);
        levelUpUI = GameObject.Instantiate(levelUpUIPrefab);

        statusMenu = GameObject.Instantiate(statusMenuPrefab);
        statusMenu.SetActive(false);

        bottomMenu = GameObject.Instantiate(bottomMenuPrefab);

        var RulesEngine = FindObjectOfType<RulesEngine>();
        RulesEngine.init(Enemies, friendlyUnits, null, tiles, inventoryUI, characterDataUI, combatForecastUI,
            tileInfoUI, healUI, actionMenuUI, statusMenu, bottomMenu);//, map, tilePrefab); //Initializion function that handles the rest of the game. 
    }
}
