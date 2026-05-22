using System;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsDirections(floorPositions, Direction2D.cardinalDirectionsList);
        var connerWallPositions = FindWallsDirections(floorPositions, Direction2D.cardinalDirectionsList);
        
        CreateBasicWall(tilemapVisualizer, basicWallPositions, floorPositions);
        CreateConnerWalls(tilemapVisualizer, connerWallPositions, floorPositions);
    }

    private static void CreateConnerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> connerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in connerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += 1;
                }
                else
                {
                    neighboursBinaryType += 0;
                }
            }
            tilemapVisualizer.PaintSingleConnerWall(position, neighboursBinaryType);
        }
    }

    private static void CreateBasicWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += 1;
                }
                else
                {
                    neighboursBinaryType += 0;
                }
            }
            tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType);
        }
    }

    private static HashSet<Vector2Int> FindWallsDirections(HashSet<Vector2Int> floorPositions,
        List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition) == false)
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }

        return wallPositions;
    }
}