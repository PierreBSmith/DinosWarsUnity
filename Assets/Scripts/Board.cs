using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;

[Serializable]
public class MoveCharacterEvent : UnityEvent<PathToTile> { }

public class Board : MonoBehaviour
{
    // Start is called before the first frame update
    public TileBehaviour[,] terrain;
    private List<PathToTile> activeRange;
    private List<TileBehaviour> activeAttackRange;
    public UnityEvent deselectCharacter;
    public MoveCharacterEvent moveCharacter;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Initialization function that draws the map given a 2D array of Tile.Type's
    public void init(Map1 map, TileBehaviour tilePrefab)
    {
        terrain = new TileBehaviour[map.map.GetLength(0), map.map.GetLength(1)];
        activeRange = new List<PathToTile>();
        activeAttackRange = new List<TileBehaviour>();
        for (int i = 0; i < map.map.GetLength(0); i++)
        {
            for (int j = 0; j < map.map.GetLength(1); j++)
            {
                TileBehaviour newTile = Instantiate(tilePrefab, this.transform, true);
                map.map[i,j] = newTile.tile.type;
                newTile.transform.position = new Vector3(i, j, 0);
                newTile.positon = new Vector2Int(i, j);
                terrain[i, j] = newTile;
                newTile.clicked.AddListener(tileClicked);
            }
        }
    }
    //Tile click event handler. If there is a friendly unit selected at the time of the click and the click is in the range of the unit, move the unit to the location of the click. Otherwise deselect unit.
    private void tileClicked(TileBehaviour tile)
    {
        if (activeRange.Any((x)=>x.tile == tile.positon))
        {
            moveCharacter.Invoke(activeRange.First((x)=>x.tile == tile.positon));
        }
        else
        {
            deselectCharacter.Invoke();
        }
    }
    //Function to find potential tiles that CharacterMovement can move to so that paths can be made out of these tiles
    public List<Vector2Int> GeneratePotentialRange(CharacterMovement CharacterMovement)
    {
        List<Vector2Int> potentialTiles = new List<Vector2Int>();
        
        for( int i = -CharacterMovement.character.moveRange; i <= CharacterMovement.character.moveRange; i++)
        {
            for( int j = -(CharacterMovement.character.moveRange - Mathf.Abs(i)); j <= CharacterMovement.character.moveRange - Mathf.Abs(i); j++)
            {
                Vector2Int currTilePos = CharacterMovement.position + new Vector2Int(i, j);
                if(currTilePos.x >= 0 && currTilePos.x < terrain.GetLength(0) && currTilePos.y >= 0 && currTilePos.y < terrain.GetLength(1))
                {
                    //This might need to get changed
                    if(CharacterMovement.position != currTilePos && (!CharacterMovement.character.grounded || getTerrainTile(currTilePos).tile.walkable) && canPassThrough(getTerrainTile(currTilePos), CharacterMovement.character.type))
                        potentialTiles.Add(currTilePos);
                }
            }
        }

        return potentialTiles;
    }
    //Returns a list of all walkable tiles if unit is grounded or just all the tiles otherwise
    private List<Vector2Int> allLegalTiles(CharacterMovement CharacterMovement)
    {
        List<Vector2Int> tileSet = new List<Vector2Int>();
        foreach (var tile in terrain)
        {
            if (CharacterMovement.position != tile.positon && (!CharacterMovement.character.grounded || getTerrainTile(tile.positon).tile.walkable))
            {
                tileSet.Add(tile.positon);
            }
        }
        return tileSet;
    }
    
    //helper function for if the unit can pass through the tile that is handed to it
    private bool canPassThrough(TileBehaviour tile, Character.Type CharacterMovementType)
    {
        if(tile.occupied == null)
        {
            return true;
        }
        else
        {
            return !(tile.occupied.character.type == Character.Type.ENEMY ^ CharacterMovementType == Character.Type.ENEMY);
        }
    }
    //Wrapper function for generatePaths() that generates possible friendly unit paths
    public List<PathToTile> generateFriendlyPaths(CharacterMovement CharacterMovement)
    {
        List<Vector2Int> potentialRanges = GeneratePotentialRange(CharacterMovement);
        List<PathToTile> sptSet = generatePaths(potentialRanges, CharacterMovement.position);

        return sptSet.Where((x) => x.weightedDistance <= CharacterMovement.character.moveRange && getTerrainTile(x.tile).occupied == null).ToList();
    }
    //Wrapper function for generate paths that gets a the quickest path to a friendly unit
    public PathToTile generateEnemyPath(CharacterMovement CharacterMovement)
    {
        List<Vector2Int> searchSpace = allLegalTiles(CharacterMovement);
        List<PathToTile> path = generatePaths(searchSpace, CharacterMovement.position, Character.Type.FRIENDLY);
        PathToTile finalPath = new PathToTile(new Vector2Int(0,0));
        finalPath.weightedDistance = 0;
        int nextDist = 0;
        foreach(var tile in path[0].path)
        {
           
            if(nextDist + finalPath.weightedDistance > CharacterMovement.character.moveRange)
            {
                break;    
            }
            else
            {
                finalPath.path.Add(tile);
                finalPath.weightedDistance += nextDist;
            }
            nextDist = 1 - getTerrainTile(tile).tile.movementBonus;
        }
        finalPath.tile = finalPath.path.Last();
        finalPath.path.Remove(finalPath.path.Last());
        return finalPath;
    }


    //Called by generateFriendlyPaths() and generateEnemyPath(). 
    //If called by generateFriendlyPaths() returns a list of shortest paths to every single possible tile within the CharacterMovements move range.
    //If called by generateEnemyPath() returns shortest path to a friendlyUnit()
    //This is an implementation of Dijkstra algorithm
    private List<PathToTile> generatePaths(List<Vector2Int> searchSpace, Vector2Int startingSpace, Character.Type? targetType = null)
    {
        List<PathToTile> notYetIncluded = new List<PathToTile>();
        List<PathToTile> sptSet = new List<PathToTile>();
        foreach (var obj in searchSpace)
        {
            notYetIncluded.Add(new PathToTile(obj));
        }
        notYetIncluded.Add(new PathToTile(startingSpace, 0, new List<Vector2Int>()));
        PathToTile shortestDist = notYetIncluded.Min();
        while (notYetIncluded.Count != 0 && shortestDist.weightedDistance < int.MaxValue)
        {
            if (targetType != null && getTerrainTile(shortestDist.tile).occupied?.character.type == targetType)
            {
                return new List<PathToTile>{shortestDist};
            }
            if (shortestDist.tile != startingSpace)
                sptSet.Add(shortestDist);
            notYetIncluded.Remove(shortestDist);
            notYetIncluded = updateAdjacents(shortestDist, notYetIncluded);
            shortestDist = notYetIncluded.Min();
        }
        return sptSet;
    }
    //Helper function for generatePaths()
    private List<PathToTile> updateAdjacents(PathToTile tile, List<PathToTile> list)
    {
        foreach(var obj in list)
        {
            if((tile.tile - obj.tile).magnitude == 1)
            {
                obj.updatePath(tile, 1 - getTerrainTile(tile.tile).tile.movementBonus);
            }
        }
        return list;
    }
    //helper function to return a tile at specific coords using a vector2
    public TileBehaviour getTerrainTile(Vector2Int coords)
    {
        return terrain[coords.x, coords.y];
    }
    //Function to draw moveRange for a selected unit
    public void showMoveRange(CharacterMovement CharacterMovement)
    {
        activeRange = generateFriendlyPaths(CharacterMovement);
        foreach(var path in activeRange)
        {
            getTerrainTile(path.tile).setMask(false, CharacterMovement.character.type);
        }

    }
    public void showAttackRange(Vector2Int startingSpace, int range)
    {
        for (int i = - range; i <= range; i++)
        {
            for (int j = - (range - Mathf.Abs(i)); j <= range - Mathf.Abs(i); j++)
            {

                if (!(i == 0 && j == 0))
                {
                    terrain[i + startingSpace.x, j + startingSpace.y].setMask(true, Character.Type.ENEMY);
                    activeAttackRange.Add(terrain[i + startingSpace.x, j + startingSpace.y]);
                }
            }
        }
    }
    public void clearAttackRange()
    {
        foreach (var tile in activeAttackRange)
        {
           tile.clearMask();
        }
        activeAttackRange.Clear();
    }
    //Function that clears drawn move range when a CharacterMovement is deselected
    public void clearMoveRange()
    {
        foreach (var path in activeRange)
        {
            getTerrainTile(path.tile).clearMask();
        }
        activeRange.Clear();
    }
}
//Utility class that holds Path, Tile, and Distance information for a units path
public class PathToTile : IComparable<PathToTile>
{
    public int weightedDistance;
    public Vector2Int tile;
    public List<Vector2Int> path;

    public void updatePath(PathToTile newPath, int dist)
    {
        if(weightedDistance > newPath.weightedDistance + dist)
        {
            path = new List<Vector2Int>(newPath.path);
            path.Add(newPath.tile);
            weightedDistance = newPath.weightedDistance + dist;
        }
    }
    public PathToTile(Vector2Int tile)
    {
        this.tile = tile;
        weightedDistance = int.MaxValue;
        path = new List<Vector2Int>();
    }
    public PathToTile(Vector2Int tile, int weightedDistance, List<Vector2Int> path)
    {
        this.tile = tile;
        this.weightedDistance = weightedDistance;
        this.path = new List<Vector2Int>(path);
    }
    
    public int CompareTo(PathToTile other)
    {
        return weightedDistance.CompareTo(other.weightedDistance);
    }
}
