using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] protected SimpleRandomWalkSO randomWalkParameters;

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);   // Gọi hàm CreateWalls để tạo các bức tường xung quanh các vị trí sàn đã được tạo ra. Hàm này sẽ xác định các vị trí cần có tường dựa trên các vị trí sàn và vẽ chúng trên tilemap.
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position)   // Hàm này thực hiện thuật toán Tạo ra các vị trí sàn trong dungeon.
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions.UnionWith(path);

            if (parameters.startRandomlyEachIteration) // Nếu biến startRandomlyEachIteration được đặt thành true, thì vòng lặp tiếp theo sẽ bắt đầu từ một vị trí ngẫu nhiên đã được tạo ra trước đó trong floorPositions.
            {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));

            }
        }
        return floorPositions;
    }
}
