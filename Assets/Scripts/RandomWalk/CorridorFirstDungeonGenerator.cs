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
        CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
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

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
    }
}
