using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private Camera mainCam;
    private Transform playerTransform;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        mainCam = Camera.main;
        playerTransform = transform.parent;
    }

    private void Update()
    {
        // Lấy vị trí chuột
        Vector3 mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // Tính hướng từ nhân vật đến con trỏ chuột
        Vector3 aimDirection = (mousePosition - playerTransform.position).normalized;

        // Gửi thông số hướng (X, Y) vào Animator
        animator.SetFloat("AimX", aimDirection.x);
        animator.SetFloat("AimY", aimDirection.y);
    }
}