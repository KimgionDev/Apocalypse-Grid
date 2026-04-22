using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float roomPercent = 0.8f;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();
        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
            //corridors[i] = IncreaseCorridorBrush3by3(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    // Tăng kích thước hành lang lên 3x3 bằng cách thêm tất cả các ô gạch nằm xung quanh mỗi ô gạch của hành lang cũ
    public List<Vector2Int> IncreaseCorridorBrush3by3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 0; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    // Tăng kích thước hành lang lên 3x3 bằng cách thêm các ô gạch nằm xung quanh mỗi ô gạch của hành lang cũ
    public List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int previousDirection = Vector2Int.zero;

        for (int i = 1; i < corridor.Count; i++)
        {
            // Lưu hướng di chuyển từ ô gạch trước đó đến ô gạch hiện tại
            Vector2Int directionFromCell = corridor[i] - corridor[i - 1];

            // Nếu không phải là ô gạch đầu tiên và hướng di chuyển thay đổi so với hướng trước đó, thêm tất cả các ô gạch xung quanh ô gạch trước đó để tạo ra một góc rộng hơn
            if (previousDirection != Vector2Int.zero && directionFromCell != previousDirection)
            {
                for (int x = -1; x < 2; x++) // từ -1 vì muốn thêm cả ô gạch nằm ở bên trái và bên phải của ô gạch hiện tại, tương tự với y
                {
                    for (int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                    }
                }
            }
            else
            {
                Vector2Int offset = GetDirection90From(directionFromCell);
                newCorridor.Add(corridor[i - 1] + offset);
                newCorridor.Add(corridor[i - 1] - offset);
            }

            newCorridor.Add(corridor[i - 1]);

            previousDirection = directionFromCell;
        }

        if (corridor.Count > 0)
        {
            Vector2Int lastTile = corridor[corridor.Count - 1];
            Vector2Int offset = GetDirection90From(previousDirection);
            newCorridor.Add(lastTile);
            newCorridor.Add(lastTile + offset);
            newCorridor.Add(lastTile - offset);
        }

        return newCorridor;
    }

    // Hàm này có tác dụng xác định hướng 2 bên
    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return Vector2Int.right;
        if (direction == Vector2Int.right) return Vector2Int.down;
        if (direction == Vector2Int.down) return Vector2Int.left;
        if (direction == Vector2Int.left) return Vector2Int.up;
        return Vector2Int.zero;
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            // Kiểm tra xem điểm ngõ cụt này đã được biến thành phòng từ trước chưa?
            // (Điều này có thể xảy ra nếu phòng bên cạnh to ra và nuốt chửng điểm này)
            if (roomFloors.Contains(position) == false)
            {
                // Nếu chưa, tạo một phòng mới tại điểm ngõ cụt này
                var roomFloor = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(roomFloor);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (var position in floorPositions)
        {
            int neighboursCount = 0;

            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                // Nếu hướng đó cũng là một ô nền, tăng biến đếm lên 1
                if (floorPositions.Contains(position + direction))
                {
                    neighboursCount++;
                }
            }

            // Nếu ô này chỉ kết nối với ĐÚNG 1 ô nền khác -> nó là điểm tận cùng
            if (neighboursCount == 1)
            {
                deadEnds.Add(position);
            }
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);   // Tính toán số lượng phòng cần tạo dựa trên phần trăm đã định sẵn và số lượng vị trí tiềm năng.

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();   // Lấy ngẫu nhiên một số lượng vị trí từ potentialRoomPositions để tạo phòng. Sử dụng OrderBy với Guid.NewGuid() để xáo trộn danh sách và Take để lấy số lượng cần thiết.

        foreach (var roomPosision in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosision);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            corridors.Add(corridor);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
        return corridors;
    }
}
