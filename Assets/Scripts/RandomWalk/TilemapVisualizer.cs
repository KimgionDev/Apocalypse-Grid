using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTilemap;

    [SerializeField] private TileBase floorTile,
        wallTopTile,
        wallBottomTile,
        wallSideRightTile,
        wallSideLeftTile,
        wallFullTile,
        wallInnerCornerDownLeftTile,
        wallInnerCornerDownRightTile,
        wallInnerCornerUpLeftTile,
        wallInnerCornerUpRightTile,
        wallDiagonalDownLeftTile,
        wallDiagonalDownRightTile,
        wallDiagonalUpLeftTile,
        wallDiagonalUpRightTile;

    // Hàm này nhận vào một tập hợp các vị trí sàn (floorPositions) và sử dụng chúng để vẽ các ô sàn trên tilemap. Nó gọi hàm PaintTiles để thực hiện việc vẽ từng ô sàn dựa trên các vị trí đã cho.
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    // Hàm này nhận vào một tập hợp các vị trí (positions), một tilemap và một tile, và vẽ các ô tương ứng trên tilemap.
    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypesHelper.wallTop.Contains(typAsInt))
        {
            tile = wallTopTile;
        }
        else if (WallTypesHelper.wallBottm.Contains(typAsInt))
        {
            tile = wallBottomTile;
        }
        else if (WallTypesHelper.wallSideRight.Contains(typAsInt))
        {
            tile = wallSideRightTile;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typAsInt))
        {
            tile = wallSideLeftTile;
        }
        else if (WallTypesHelper.wallFull.Contains(typAsInt))
        {
            tile = wallFullTile;
        }

        Debug.LogWarning(position + binaryType);
        
        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }
        else
        {
            Debug.LogError("No tile found for binary type: " + binaryType);
        }
    }

    public void PaintSingleConnerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLeftTile;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRightTile;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalDownLeftTile;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallDiagonalDownRightTile;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalUpLeftTile;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallDiagonalUpRightTile;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFullTile;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = wallBottomTile;
        }
        else if (WallTypesHelper.wallInnerCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerUpLeftTile;
        }
        else if (WallTypesHelper.wallInnerCornerUpRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerUpRightTile;
        }

        Debug.Log(position + binaryType);
        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
    }
}