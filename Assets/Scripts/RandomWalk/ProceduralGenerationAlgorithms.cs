using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    // Hàm này tạo ra một hành lang ngẫu nhiên bắt đầu từ startPosition và có độ dài corridorLength. Kết quả là một danh sách các vị trí sàn mới được tạo ra, đại diện cho hành lang.
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        // path chứa tất cả các vị trí đã đi qua trong quá trình random walk. HashSet được sử dụng để đảm bảo rằng mỗi vị trí chỉ được lưu trữ một lần, tránh trùng lặp.
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

    // Hàm này tạo ra một hành lang ngẫu nhiên bắt đầu từ startPosition và có độ dài corridorLength. Kết quả là một danh sách các vị trí sàn mới được tạo ra, đại diện cho hành lang.
    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }

    /*
        Hàm này thực hiện thuật toán Binary Space Partitioning (BSP) để chia một không gian lớn thành các phòng nhỏ hơn.
        Với đầu vào là một BoundsInt đại diện cho không gian cần chia, cùng với chiều rộng và chiều cao tối thiểu của các phòng
        Hàm sẽ trả về một danh sách các BoundsInt đại diện cho các phòng đã được tạo ra.
    */
    public static List<BoundsInt> BinarySpacePartioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        // roomsList sẽ chứa tất cả các phòng đã được tạo ra sau khi quá trình chia không gian hoàn thành.
        List<BoundsInt> roomsList = new List<BoundsInt>();

        // roomsQueue được sử dụng để quản lý các không gian cần được chia trong quá trình thực hiện thuật toán.
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)
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
                else
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

    private static void SplitHorizontally(int minHeight, List<BoundsInt> rooms, Queue<BoundsInt> roomsQueue, BoundsInt room)
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