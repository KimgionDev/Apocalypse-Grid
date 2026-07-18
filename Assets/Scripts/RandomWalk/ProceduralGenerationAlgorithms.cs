using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }

    public static List<BoundsInt> BinarySpacePartioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        List<BoundsInt> roomsList = new List<BoundsInt>();
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)    // bé hơn < 0.5f thì cắt ngang trước rồi mới cắt dọc
                {
                    // Nếu chiều cao của phòng đủ lớn để chia đôi theo chiều ngang, thực hiện chia theo chiều ngang
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsList, roomsQueue, room);
                    }
                    // Nếu chiều rộng của phòng đủ lớn để chia đôi theo chiều dọc, thực hiện chia theo chiều dọc
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsList, roomsQueue, room);
                    }
                    // Nếu cả chiều cao và chiều rộng của phòng đều không đủ lớn để chia đôi, thêm phòng vào danh sách kết quả
                    else
                    {
                        roomsList.Add(room);
                    }
                }
                else                        // lớn hơn >= 0.5f thì cắt dọc trước rồi cắt ngang
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsList, roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsList, roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }

        return roomsList;
    }

    private static void SplitVertically(int minWidth, List<BoundsInt> rooms, Queue<BoundsInt> roomsQueue,
        BoundsInt room)
    {
        var xSplit = Random.Range(minWidth, (room.size.x - minWidth) + 1);

        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, List<BoundsInt> rooms, Queue<BoundsInt> roomsQueue,
        BoundsInt room)
    {
        var ySplit = Random.Range(minHeight, (room.size.y - minHeight) + 1);

        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // Up
        new Vector2Int(1, 0), // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, 0) // Left
    };

    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1, 1), // UP-RIGHT
        new Vector2Int(1, -1), // RIGHT-DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 1) // LEFT-UP
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // Up
        new Vector2Int(1, 1), // UP-RIGHT
        new Vector2Int(1, 0), // Right
        new Vector2Int(1, -1), // RIGHT-DOWN
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 0), // Left
        new Vector2Int(-1, 1) // LEFT-UP
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}