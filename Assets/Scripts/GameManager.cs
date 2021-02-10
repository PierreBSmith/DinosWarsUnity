using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Game Manager that is always running in the background and handles transferring information between scenes
    // Start is called before the first frame update
    public List<CharacterMovement> enemyList;
    public List<CharacterMovement> friendList;
    public TileBehaviour tilePrefab;

    void Start()
    {
        //Initializes the map, friendly units, and enemy units so it can hand it to RulesEngine. Currently uses prefab references that are given to it in the Unity development client.
        Tile.Type[,] tileSet = new Tile.Type[24,16]; //Placeholder map that is all grass tiles, this can be any map theoretically
        for(int i = 0; i < tileSet.GetLength(0); i++){
            for(int j = 0; j < tileSet.GetLength(1); j++){
                tileSet[i,j] = Tile.Type.GRASS; 
            }
        }
        Map1 map = new Map1(tileSet, new List<Vector2Int>{new Vector2Int(3,3),new Vector2Int(5,3),new Vector2Int(3,5)},
                    new List<Vector2Int>{new Vector2Int(15,13),new Vector2Int(13,13),new Vector2Int(13,15)});
        List<CharacterMovement> Enemies = new List<CharacterMovement>();
       
        foreach(var enemy in enemyList)
        {
            Enemies.Add(Instantiate(enemy));
            if (Enemies.Count >= map.enemySpawnPoints.Count)
                break;
        }
        List<CharacterMovement> Friends = new List<CharacterMovement>();
        foreach(var friend in friendList)
        {
            Friends.Add(Instantiate(friend));
            if (Friends.Count >= map.friendlySpawnPoints.Count)
                break;
        }
        var RulesEngine = FindObjectOfType<RulesEngine>();

        RulesEngine.init(Enemies, Friends, null, map, tilePrefab); //Initializion function that handles the rest of the game.

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
