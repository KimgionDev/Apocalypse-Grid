using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 inputDirection;

    [SerializeField] private Animator bodyAnimator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Giảm khả năng xuyên tường khi di chuyển nhanh
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Update()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;
        if (Input.GetKey(KeyCode.S)) moveY -= 1f;
        if (Input.GetKey(KeyCode.W)) moveY += 1f;

        inputDirection = new Vector2(moveX, moveY).normalized;

        if(bodyAnimator != null)
        {
            bodyAnimator.SetFloat("Speed", inputDirection.magnitude);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = inputDirection * moveSpeed;
    }
}
