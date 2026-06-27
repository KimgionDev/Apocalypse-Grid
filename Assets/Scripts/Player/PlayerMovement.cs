using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public PlayerStatsSO playerStats;

    [SerializeField] private float moveSpeed = 5f;
    public static Transform InstanceTransform { get; private set; }

    private Rigidbody2D rb;
    private Vector2 inputDirection;
    [SerializeField] private Animator bodyAnimator;
    
    public AudioClip footstepSound;
    public float footstepDelay = 0.35f;
    private float nextFootstepTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        InstanceTransform = this.transform;
    }

    private void Start()
    {
        if (playerStats != null)
        {
            moveSpeed = playerStats.moveSpeed;
        }
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

        if (bodyAnimator != null)
        {
            bodyAnimator.SetFloat("Speed", inputDirection.magnitude);
        }

        if (inputDirection.magnitude > 0f)
        {
            if (Time.time >= nextFootstepTime && footstepSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(footstepSound);
                nextFootstepTime = Time.time + footstepDelay;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = inputDirection * moveSpeed;
    }
}