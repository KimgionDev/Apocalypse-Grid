using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Dungeon/Item Data")]
public class ItemData : ScriptableObject
{
    public GameObject prefab;
    public Vector2Int size = new Vector2Int(1, 1);
    public int minQuantity; 
    public int maxQuantity; 
    public PlacementType placementType;
    public ItemType itemType; 
}

public enum PlacementType
{
    OpenSpace, NearWall 
}

public enum ItemType
{
    DestructibleObstacle,
    ConsumableLoot,
    StaticEnvironment
}