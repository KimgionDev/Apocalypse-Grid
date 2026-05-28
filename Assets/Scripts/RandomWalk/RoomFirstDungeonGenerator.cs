using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField] [Range(0, 10)] private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomList = ProceduralGenerationAlgorithms.BinarySpacePartioning(new BoundsInt((Vector3Int)startPosition,
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

        if (randomWalkRooms)
        {
            floor = CreateRandomRooms(roomList, roomsDictionary);
        }
        else
        {
            floor = CreateSimpleRooms(roomList, roomsDictionary);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
        PlaceItemsInRooms(roomsDictionary, corridors);
    }

    private void PlaceItemsInRooms(Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary,
        HashSet<Vector2Int> corridors)
    {
        // Duyệt qua từng phòng đã được tạo
        foreach (var room in roomsDictionary)
        {
            // Khởi tạo Helper kiểm tra không gian 8 hướng cho phòng hiện tại, đồng thời loại trừ hành lang
            ItemPlacementHelper placementHelper = new ItemPlacementHelper(room.Value, corridors);

            // Bắt đầu logic TryPlaceItem ở đây (ví dụ minh họa)
            // Debug.Log($"Đang xử lý phòng tại {room.Key} có {room.Value.Count} ô gạch sàn.");

            // Mã giả:
            // if (placementHelper.TryPlaceItem(tableData, out Vector2Int placedPos))
            // {
            //      Instantiate(tableData.prefab, (Vector3Int)placedPos, Quaternion.identity);
            // }
        }
    }

    private HashSet<Vector2Int> CreateRandomRooms(List<BoundsInt> roomList,
        Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomList.Count; i++)
        {
            var roomBounds = roomList[i];
            var roomCenter =
                new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);

            HashSet<Vector2Int> individualRoomFloor = new HashSet<Vector2Int>(); // HashSet riêng cho phòng này

            foreach (var position in roomFloor)
            {
                if (position.x >= roomBounds.xMin + offset && position.x <= roomBounds.xMax - offset &&
                    position.y >= roomBounds.yMin + offset && position.y <= roomBounds.yMax - offset)
                {
                    floor.Add(position);
                    individualRoomFloor.Add(position); // Lưu ô gạch vào phòng
                }
            }

            // Thêm dữ liệu phòng vào Dictionary
            roomsDictionary.Add(roomCenter, individualRoomFloor);
        }

        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;

        // Hàm phụ để đắp thêm gạch vào xung quanh vị trí hiện tại (làm hành lang rộng 3x3)
        void AddWideCorridorPosition(Vector2Int pos)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    corridor.Add(pos + new Vector2Int(x, y));
                }
            }
        }

        AddWideCorridorPosition(position);

        // Đi dọc theo trục Y
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }

            AddWideCorridorPosition(position);
        }

        // Đi ngang theo trục X
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }

            AddWideCorridorPosition(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2Int.Distance(currentRoomCenter, position);
            if (currentDistance < distance)
            {
                closest = position;
                distance = currentDistance;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomList,
        Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach (var room in roomList)
        {
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(); // HashSet riêng cho phòng này
            Vector2Int roomCenter = (Vector2Int)Vector3Int.RoundToInt(room.center);
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                    roomFloor.Add(position);
                }
            }

            roomsDictionary.Add(roomCenter, roomFloor);
        }

        return floor;
    }
}