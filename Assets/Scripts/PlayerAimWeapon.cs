using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        // Tính toán góc xoay
        Vector3 mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        transform.eulerAngles = new Vector3(0, 0, angle);

        // Lật trục Y của súng nếu chuột sang trái
        Vector3 localScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;
        }

        transform.localScale = localScale;
    }
}