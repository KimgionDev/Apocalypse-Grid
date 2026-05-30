using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Dungeon/Item Data")]
public class ItemData : ScriptableObject
{
    public GameObject prefab;
    public Vector2Int size = new Vector2Int(1, 1);
    public int minQuantity; 
    public int maxQuantity; 
    public PlacementType placementType;
}

public enum PlacementType
{
    OpenSpace, NearWall 
}