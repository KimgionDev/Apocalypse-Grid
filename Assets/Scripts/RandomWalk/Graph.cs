using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    private HashSet<Vector2Int> roomTiles;

    public Graph(HashSet<Vector2Int> tiles)
    {
        this.roomTiles = tiles;
    }

    // Hàm đếm số ô lân cận trong 8 hướng
    public int CountEightNeighbors(Vector2Int position)
    {
        int count = 0;
        
        foreach (var direction in Direction2D.eightDirectionsList)
        {
            if (roomTiles.Contains(position + direction))
            {
                count++;
            }
        }
        
        return count;
    }
}