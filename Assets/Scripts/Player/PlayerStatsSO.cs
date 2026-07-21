using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Data/Player Stats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Tiến trình Game")]
    public int currentLevel = 1; 
    public int totalGold = 0;  

    [Header("Chỉ số Người chơi cơ bản")]
    public float maxHealth = 100f;
    public float baseDamage = 10f;
    public float moveSpeed = 5f;
    public int shotgunAmmo = 20;
    public int rifleAmmo = 120;

    public int GetAmmo(AmmoType type)
    {
        switch (type)
        {
            case AmmoType.Shotgun: return shotgunAmmo;
            case AmmoType.Rifle: return rifleAmmo;
            default: return 999;
        }
    }

    public void ConsumeAmmo(AmmoType type, int amount)
    {
        switch (type)
        {
            case AmmoType.Shotgun: shotgunAmmo = Mathf.Max(0, shotgunAmmo - amount); break;
            case AmmoType.Rifle: rifleAmmo = Mathf.Max(0, rifleAmmo - amount); break;
        }
    }

    public void AddAmmo(AmmoType type, int amount)
    {
        switch (type)
        {
            case AmmoType.Shotgun: shotgunAmmo += amount; break;
            case AmmoType.Rifle: rifleAmmo += amount; break;
        }
    }
}