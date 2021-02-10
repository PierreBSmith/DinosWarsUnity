using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1
{
    //So we erasing this yeah.
    //Data Structure to store the tileset for a map and the spawn points for units so that things can be loaded in.
    public Tile.Type[,] map;
    public List<Vector2Int> friendlySpawnPoints;
    public List<Vector2Int> enemySpawnPoints;
    public Map1(Tile.Type[,] map, List<Vector2Int> friendlySpawnPoints, List<Vector2Int> enemySpawnPoints){
        this.map = map;
        this.friendlySpawnPoints = friendlySpawnPoints;
        this.enemySpawnPoints = enemySpawnPoints;
    }

}
