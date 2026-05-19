using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Salto")]
    [SerializeField] private float normalJumpForce = 10f;
    [SerializeField] private float specialJumpForce = 14f;
    [SerializeField] private int maxSpecialJumps = 3;
    [SerializeField] private bool allowSpecialJumpInAir = false;

    [Header("Detección de suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Estado (runtime)")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private int specialJumpsRemaining;

    private Rigidbody2D rb;
    private float moveInput;
    private bool wasGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        specialJumpsRemaining = maxSpecialJumps;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        CheckGrounded();

        if (isGrounded && !wasGrounded)
        {
            specialJumpsRemaining = maxSpecialJumps;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump(normalJumpForce);
            isGrounded = false;
        }

        bool canUseSpecialJump = specialJumpsRemaining > 0 && (isGrounded || allowSpecialJumpInAir);
        if (Input.GetKeyDown(KeyCode.LeftShift) && canUseSpecialJump)
        {
            Jump(specialJumpForce);
            specialJumpsRemaining--;
            isGrounded = false;
        }

        wasGrounded = isGrounded;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Jump(float jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
