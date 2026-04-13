using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Weapons/Gun Data")]
public class GunData : ScriptableObject
{
    public string gunName;
    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;
    public float fireRate;
    public int bulletCount;     // Số viên đạn bắn ra mỗi lần nhấn chuột (ví dụ: 1 cho súng trường, 6 cho shotgun)
    public float spreadAngle;   // Góc ngẫu nhiên tối đa mà viên đạn có thể lệch khỏi hướng bắn chính xác
    public int magSize;         // Số viên đạn tối đa trong một băng đạn
    public float reloadTime;
    public bool reloadOneByOne; // Nếu true, mỗi viên đạn sẽ được nạp lại sau reloadTime; nếu false, toàn bộ băng đạn sẽ được nạp lại cùng lúc
    public float bulletDamage;
    public float bulletLifeTime; // Tầm bắn
    public float flashScale = 1f;// Tỷ lệ kích thước của hiệu ứng muzzle flash
}