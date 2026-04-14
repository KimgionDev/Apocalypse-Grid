using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int value; // Giá tiền hoặc số lượng nhiệm vụ
    public bool isGold; // Đánh dấu nếu là tiền để cộng dồn
}