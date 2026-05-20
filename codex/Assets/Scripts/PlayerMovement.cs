using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Salto")]
    [SerializeField] private float normalJumpForce = 10f;
    [SerializeField] private float specialJumpForce = 14f;
    [SerializeField] private int maxSpecialJumps = 3;

    [Header("Suelo")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    [SerializeField] private float groundCheckOffset = 0.02f;

    [Header("Debug")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private int specialJumpsRemaining;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private float moveInput;
    private bool jumpPressed;
    private bool specialJumpPressed;
    private bool wasGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        specialJumpsRemaining = maxSpecialJumps;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
        specialJumpPressed = Input.GetKeyDown(KeyCode.LeftShift);
    }

    private void FixedUpdate()
    {
        wasGrounded = isGrounded;
        CheckGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            DoJump(normalJumpForce);
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && specialJumpsRemaining > 0)
        {
            DoJump(specialJumpForce);
            specialJumpsRemaining--;
            isGrounded = false;
        }
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (jumpPressed && isGrounded)
        {
            DoJump(normalJumpForce);
        }
        else if (specialJumpPressed && isGrounded && specialJumpsRemaining > 0)
        {
            DoJump(specialJumpForce);
            specialJumpsRemaining--;
        }

        jumpPressed = false;
        specialJumpPressed = false;
    }

    private void CheckGrounded()
    {
        Bounds bounds = boxCollider.bounds;
        Vector2 checkCenter = new Vector2(bounds.center.x, bounds.min.y - groundCheckOffset);
        isGrounded = Physics2D.OverlapBox(checkCenter, groundCheckSize, 0f, groundLayer) != null;
    }

    private void DoJump(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        Bounds bounds = col.bounds;
        Vector2 checkCenter = new Vector2(bounds.center.x, bounds.min.y - groundCheckOffset);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(checkCenter, groundCheckSize);
    }
}