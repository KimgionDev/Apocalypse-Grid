using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPlacementHelper
{
    private Dictionary<PlacementType, List<Vector2Int>> freeSpaces;

    public ItemPlacementHelper(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> corridorPositions)
    {
        freeSpaces = new Dictionary<PlacementType, List<Vector2Int>>();
        freeSpaces.Add(PlacementType.OpenSpace, new List<Vector2Int>());
        freeSpaces.Add(PlacementType.NearWall, new List<Vector2Int>());

        Graph roomGraph = new Graph(roomFloor);

        // Lặp qua từng ô sàn của phòng 
        foreach (var pos in roomFloor)
        {
            // Trừ đi các vị trí hành lang để không block lối đi 
            if (corridorPositions.Contains(pos)) continue;

            // Phân loại ô 
            if (roomGraph.CountEightNeighbors(pos) < 8)
                freeSpaces[PlacementType.NearWall].Add(pos);
            else
                freeSpaces[PlacementType.OpenSpace].Add(pos);
        }
    }

    public bool TryPlaceItem(ItemData item, out Vector2Int placedPosition)
    {
        placedPosition = Vector2Int.zero;
        int maxIterations = 100; // Tránh lặp vô hạn
        int iterations = 0;

        List<Vector2Int> availableSpaces = freeSpaces[item.placementType];

        while (iterations < maxIterations && availableSpaces.Count > 0)
        {
            // Lấy ngẫu nhiên một vị trí
            int randomIndex = Random.Range(0, availableSpaces.Count);
            Vector2Int potentialPos = availableSpaces[randomIndex];

            // Nếu vật phẩm size 1x1, chỉ cần đặt luôn
            if (item.size.x == 1 && item.size.y == 1)
            {
                placedPosition = potentialPos;
                availableSpaces.RemoveAt(randomIndex);
                return true;
            }

            // Nếu vật phẩm lớn hơn, chạy vòng lặp kiểm tra không gian
            bool canPlace = CheckIfSpaceIsFree(potentialPos, item.size, availableSpaces);
            if (canPlace)
            {
                placedPosition = potentialPos;
                MarkSpaceAsTaken(potentialPos, item.size, availableSpaces);
                return true;
            }

            iterations++;
        }

        return false;
    }

    // Hàm kiểm tra không gian cho vật phẩm lớn
    private bool CheckIfSpaceIsFree(Vector2Int startPos, Vector2Int size, List<Vector2Int> availableSpaces)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int posToCheck = startPos + new Vector2Int(x, y);
                if (!availableSpaces.Contains(posToCheck))
                {
                    return false; // Có vật cản hoặc tường
                }
            }
        }

        return true;
    }

    // Hàm xóa các ô đã bị chiếm dụng
    private void MarkSpaceAsTaken(Vector2Int startPos, Vector2Int size, List<Vector2Int> availableSpaces)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                availableSpaces.Remove(startPos + new Vector2Int(x, y));
            }
        }
    }
}