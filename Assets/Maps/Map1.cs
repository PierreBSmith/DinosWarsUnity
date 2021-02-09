using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1
{
    public Tile.Type[,] map;
    public List<Vector2Int> friendlySpawnPoints;
    public List<Vector2Int> enemySpawnPoints;
    public Map1(Tile.Type[,] map, List<Vector2Int> friendlySpawnPoints, List<Vector2Int> enemySpawnPoints){
        this.map = map;
        this.friendlySpawnPoints = friendlySpawnPoints;
        this.enemySpawnPoints = enemySpawnPoints;
    }

}
