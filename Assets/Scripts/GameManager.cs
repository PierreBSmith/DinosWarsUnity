using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Game Manager that is always running in the background and handles transferring information between scenes
    // Start is called before the first frame update
    public List<CharacterMovement> enemyList;
    public List<CharacterMovement> friendList;
    //public TileBehaviour tilePrefab;
    private GameObject[] tiles;
    [Header("UI Menus")]
    [SerializeField]
    private GameObject InventoryUIPrefab;
    private GameObject inventoryUI;
    [SerializeField]
    private GameObject CharacterDataUIPrefab;
    private GameObject characterDataUI;
    [SerializeField]
    private GameObject combatForecastUIPrefab;
    private GameObject combatForecastUI;

    [Header("Memory Scriptable Values")]
    public List<Item> allItems = new List<Item>();

    void Start()
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
        List<CharacterMovement> Enemies = new List<CharacterMovement>();
       
        foreach(var enemy in enemyList)
        {
            Enemies.Add(enemy);//TODO: Don't forget to add the instantiate back
            /*
            if (Enemies.Count >= map.enemySpawnPoints.Count)
                break;
            */
        }
        List<CharacterMovement> Friends = new List<CharacterMovement>();
        foreach(var friend in friendList)
        {
            Friends.Add(friend); //TODO:Don't forget to add the instantiate back
            /*
            if (Friends.Count >= map.friendlySpawnPoints.Count)
                break;
                */
        }
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        inventoryUI = GameObject.Instantiate(InventoryUIPrefab);
        inventoryUI.SetActive(false);

        characterDataUI = GameObject.Instantiate(CharacterDataUIPrefab);
        characterDataUI.SetActive(false);
        
        combatForecastUI = GameObject.Instantiate(combatForecastUIPrefab);
        combatForecastUI.SetActive(false);

        var RulesEngine = FindObjectOfType<RulesEngine>();
        RulesEngine.init(Enemies, Friends, null, tiles, inventoryUI, characterDataUI, combatForecastUI);//, map, tilePrefab); //Initializion function that handles the rest of the game.

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
