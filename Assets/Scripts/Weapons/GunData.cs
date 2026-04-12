using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Weapons/Gun Data")]
public class GunData : ScriptableObject
{
    public string gunName;
    public GameObject bulletPrefab;
    public float fireRate;
    public int bulletCount;
    public float spreadAngle;
    public int magSize;
    public float reloadTime;
}