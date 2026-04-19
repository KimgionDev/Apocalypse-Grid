using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] private SimpleRandomWalkSO randomWalkParameters;   // Biến này sẽ lưu trữ một ScriptableObject chứa các tham số cho thuật toán Random Walk, như số lần lặp, độ dài của mỗi bước đi, và liệu có bắt đầu từ vị trí ngẫu nhiên mỗi lần hay không.

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters);   // Gọi hàm RunRandomWalk để thực hiện thuật toán Random Walk và nhận về một tập hợp các vị trí sàn đã được tạo ra.
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);   // Gọi hàm CreateWalls để tạo các bức tường xung quanh các vị trí sàn đã được tạo ra. Hàm này sẽ xác định các vị trí cần có tường dựa trên các vị trí sàn và vẽ chúng trên tilemap.
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters)   // Hàm này thực hiện thuật toán Random Walk để tạo ra các vị trí sàn trong dungeon.
    {
        var currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>(); // Tạo một tập hợp để lưu trữ các vị trí sàn đã được tạo ra.
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
            /*
                Dùng hàm UnionWith để gộp tập hợp path mới vào tập hợp floorPositions tổng. 
                Hàm này hợp nhất hai tập hợp và bỏ qua các điểm đã tồn tại.
            */
            floorPositions.UnionWith(path);

            if (parameters.startRandomlyEachIteration) // Nếu biến startRandomlyEachIteration được đặt thành true, thì vòng lặp tiếp theo sẽ bắt đầu từ một vị trí ngẫu nhiên đã được tạo ra trước đó trong floorPositions.
            {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));

            }
        }
        return floorPositions;
    }
}
