using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField] [Range(0, 10)] private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;
    
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private GameObject hiddenPortalPrefab;
    [SerializeField] private GameObject enemySpawnerPrefab;
    [SerializeField] private List<ItemData> dungeonDecorations;
    
    private void Start()
    {
        GenerateDungeon();
    }
    
    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        tilemapVisualizer.Clear();

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
        PopulateDungeon(roomList, roomsDictionary, corridors);
    }

    private void PopulateDungeon(List<BoundsInt> roomList, Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary, HashSet<Vector2Int> corridors)
    {
        GameObject oldEntities = GameObject.Find("Entities");
        if (oldEntities != null) Destroy(oldEntities);

        GameObject oldProps = GameObject.Find("Props");
        if (oldProps != null) Destroy(oldProps);

        Vector2Int startRoomCenter = (Vector2Int)Vector3Int.RoundToInt(roomList[0].center);
        Vector2Int finalRoomCenter = (Vector2Int)Vector3Int.RoundToInt(roomList[roomList.Count - 1].center);

        Transform entityGroup = new GameObject("Entities").transform;
        Transform propGroup = new GameObject("Props").transform;

        foreach (var room in roomsDictionary)
        {
            Vector2Int currentRoomCenter = room.Key;
            Vector3 centerWorldPos = new Vector3(currentRoomCenter.x + 0.5f, currentRoomCenter.y + 0.5f, 0);

            if (currentRoomCenter == startRoomCenter)
            {
                GameObject playerInstance = Instantiate(playerPrefab, centerWorldPos, Quaternion.identity, entityGroup);                
                if (vcam != null)
                {
                    vcam.Follow = playerInstance.transform; 
                }
            }
            else if (currentRoomCenter == finalRoomCenter)
            {
                GameObject portal = Instantiate(hiddenPortalPrefab, centerWorldPos, Quaternion.identity, entityGroup);
                MissionManager.Instance.RegisterPortal(portal);
                Instantiate(enemySpawnerPrefab, centerWorldPos, Quaternion.identity, entityGroup);
            }
            else
            {
                Instantiate(enemySpawnerPrefab, centerWorldPos, Quaternion.identity, entityGroup);
            }
            
            ItemPlacementHelper placementHelper = new ItemPlacementHelper(room.Value, corridors);
            SpawnStaticDecorations(dungeonDecorations, placementHelper, propGroup);
        }
    }
    
    private void SpawnStaticDecorations(List<ItemData> items, ItemPlacementHelper helper, Transform parent)
    {
        foreach (var itemData in items)
        {
            int quantityToPlace = Random.Range(itemData.minQuantity, itemData.maxQuantity + 1);

            for (int i = 0; i < quantityToPlace; i++)
            {
                if (helper.TryPlaceItem(itemData, out Vector2Int placedPos))
                {
                    Vector3 spawnPosition = new Vector3(placedPos.x + 0.5f, placedPos.y + 0.5f, 0); 
                    Instantiate(itemData.prefab, spawnPosition, Quaternion.identity, parent);
                }
                else
                {
                    break;
                }
            }
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

            HashSet<Vector2Int> individualRoomFloor = new HashSet<Vector2Int>();

            foreach (var position in roomFloor)
            {
                if (position.x >= roomBounds.xMin + offset && position.x <= roomBounds.xMax - offset &&
                    position.y >= roomBounds.yMin + offset && position.y <= roomBounds.yMax - offset)
                {
                    floor.Add(position);
                    individualRoomFloor.Add(position);
                }
            }

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
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(); 
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