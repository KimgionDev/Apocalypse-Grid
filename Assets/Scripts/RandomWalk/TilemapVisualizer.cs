using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;

    [SerializeField]
    private TileBase floorTile, wallTopTile;

    // Dùng IEnumerable<Vector2Int> để có thể nhận bất kỳ tập hợp nào của Vector2Int, như List, HashSet, v.v. Điều này giúp cho hàm linh hoạt hơn trong việc xử lý các loại dữ liệu khác nhau mà có thể chứa các vị trí sàn.
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions) // Hàm này nhận vào một tập hợp các vị trí sàn (floorPositions) và sử dụng chúng để vẽ các ô sàn trên tilemap. Nó gọi hàm PaintTiles để thực hiện việc vẽ từng ô sàn dựa trên các vị trí đã cho.
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile) // Hàm này nhận vào một tập hợp các vị trí (positions), một tilemap và một tile, và vẽ các ô tương ứng trên tilemap.
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)       // Hàm này nhận vào một tilemap, một tile và một vị trí, và vẽ một ô duy nhất trên tilemap tại vị trí đó.
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(object position)
    {
        PaintSingleTile(wallTilemap, wallTopTile, (Vector2Int)position);
    }
}
