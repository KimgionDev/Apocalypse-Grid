using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTilemap;

    [SerializeField] private TileBase floorTile, wallTopTile, wallBottomTile, wallSideRightTile, wallSideLeftTile, wallFullTile;

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

    // Hàm này nhận vào một tilemap, một tile và một vị trí, và vẽ một ô duy nhất trên tilemap tại vị trí đó.
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

    internal void PaintSingleBasicWall(object position, string binaryType)
    {
        int typAsInt  = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if(WallTypesHelper.wallTop.Contains(typAsInt))
        {
            tile = wallTopTile;
        }
        else if(WallTypesHelper.wallBottm.Contains(typAsInt))
        {
            tile = wallBottomTile;
        }
        else if(WallTypesHelper.wallSideRight.Contains(typAsInt))
        {
            tile = wallSideRightTile;
        }
        else if(WallTypesHelper.wallSideLeft.Contains(typAsInt))
        {
            tile = wallSideLeftTile;
        }
        else if(WallTypesHelper.wallFull.Contains(typAsInt))
        {
            tile = wallFullTile;
        }
        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, (Vector2Int)position);
        }
        else
        {
            Debug.LogError("No tile found for binary type: " + binaryType);
        }
    }

    public void PaintSingleConnerWall(Vector2Int position, string neighboursBinaryType)
    {
        
    }
}