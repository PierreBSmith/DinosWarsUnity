using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Game Manager that is always running in the background and handles transferring information between scenes
    // Start is called before the first frame update
    public List<Character> enemyList;
    public List<Character> friendList;
    public Tile tilePrefab;

    void Start()
    {
        Tile.Type[,] tileSet = new Tile.Type[24,16];
        for(int i = 0; i < tileSet.GetLength(0); i++){
            for(int j = 0; j < tileSet.GetLength(1); j++){
                tileSet[i,j] = Tile.Type.GRASS; 
            }
        }
        Map1 map = new Map1(tileSet, new List<Vector2Int>{new Vector2Int(3,3),new Vector2Int(5,3),new Vector2Int(3,5)},
                    new List<Vector2Int>{new Vector2Int(15,13),new Vector2Int(13,13),new Vector2Int(13,15)});
        List<Character> Enemies = new List<Character>();
       
        foreach(var enemy in enemyList)
        {
            Enemies.Add(Instantiate(enemy));
            if (Enemies.Count >= map.enemySpawnPoints.Count)
                break;
        }
        List<Character> Friends = new List<Character>();
        foreach(var friend in friendList)
        {
            Friends.Add(Instantiate(friend));
            if (Friends.Count >= map.friendlySpawnPoints.Count)
                break;
        }
        var RulesEngine = FindObjectOfType<RulesEngine>();

        RulesEngine.init(Enemies, Friends, null, map, tilePrefab);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
