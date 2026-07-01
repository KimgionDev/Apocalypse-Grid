using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class DropItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int value;
    public bool isGold;
    public bool isHealItem; 
    public float healAmount;
    
    public AudioClip pickupSound;
}