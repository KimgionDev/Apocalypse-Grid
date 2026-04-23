using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    /*
    Hàm này thực hiện thuật toán Random Walk để tạo ra một đường đi ngẫu nhiên bắt đầu từ startPosition và có độ dài walkLength. 
    Kết quả là một tập hợp các vị trí sàn mới được tạo ra.
     */
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();   // path chứa tất cả các vị trí đã đi qua trong quá trình random walk. HashSet được sử dụng để đảm bảo rằng mỗi vị trí chỉ được lưu trữ một lần, tránh trùng lặp.

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

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)   // Hàm này tạo ra một hành lang ngẫu nhiên bắt đầu từ startPosition và có độ dài corridorLength. Kết quả là một danh sách các vị trí sàn mới được tạo ra, đại diện cho hành lang.
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
        // BoundsInt là một cấu trúc dữ liệu trong Unity đại diện cho một khối hình chữ nhật trong không gian 3D, được xác định bởi một điểm gốc (origin) và kích thước (size).
        // Trong trường hợp này, BoundsInt được sử dụng để đại diện cho các phòng trong quá trình chia không gian.
        List<BoundsInt> roomsList = new List<BoundsInt>();  // roomsList sẽ chứa tất cả các phòng đã được tạo ra sau khi quá trình chia không gian hoàn thành.
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();   // roomsQueue được sử dụng để quản lý các không gian cần được chia trong quá trình thực hiện thuật toán.
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2)       // Nếu chiều cao của phòng đủ lớn để chia đôi theo chiều ngang, thực hiện chia theo chiều ngang
                    {
                        SplitHorizontally(minHeight, roomsList, roomsQueue, room);
                    }
                    else if(room.size.x >= minWidth * 2)    // Nếu chiều rộng của phòng đủ lớn để chia đôi theo chiều dọc, thực hiện chia theo chiều dọc
                    {
                         SplitVertically(minWidth, roomsList, roomsQueue, room);
                    }
                    else                                    // Nếu cả chiều cao và chiều rộng của phòng đều không đủ lớn để chia đôi, thêm phòng vào danh sách kết quả
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

    private static void SplitVertically(int minWidth, List<BoundsInt> rooms, Queue<BoundsInt> queue, BoundsInt room)
    {
        throw new System.NotImplementedException();
    }

    private static void SplitHorizontally(int minHeight, List<BoundsInt> rooms, Queue<BoundsInt> queue, BoundsInt room)
    {
        throw new System.NotImplementedException();
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

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}
}
