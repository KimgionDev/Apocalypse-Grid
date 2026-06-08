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

    public void ResetRunProgress()
    {
        currentLevel = 1;
    }
}