using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public interface IDamageable { void TakeDamage(int dmg); }

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;          // drag child here
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundMask;           // tick "Ground" layer(s)

    [Header("Combat")]
    [SerializeField] private Transform attackPoint;          // drag child here
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyMask;            // tick "Enemy" layer
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.25f;

    [Header("Attack VFX / Debug")]
    [SerializeField] private bool showAttackDebug = true;    // draws a red circle briefly
    [SerializeField] private float debugFlashTime = 0.1f;

    [SerializeField] private bool flashPlayerOnAttack = false;
    [SerializeField] private Color playerFlashColor = Color.yellow;
    [SerializeField] private float playerFlashTime = 0.1f;

    [SerializeField] private GameObject slashEffectPrefab;   // optional small sprite prefab
    [SerializeField] private Vector3 slashOffset = Vector3.zero;
    [SerializeField] private float slashLifetime = 0.25f;

    private Rigidbody2D rb;
    private bool facingRight = true;
    private bool canAttack = true;

    // New Input System
    private InputSystem_Actions input;       // auto-generated class from your .inputactions
    private Vector2 moveInput;               // x = left/right
    private bool requestJump;
    private bool requestAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();

        // subscribe to actions
        input.Player.Move.performed += OnMovePerformed;
        input.Player.Move.canceled += OnMoveCanceled;

        input.Player.Jump.performed += OnJumpPerformed;
        input.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        // unsubscribe to avoid leaks
        input.Player.Move.performed -= OnMovePerformed;
        input.Player.Move.canceled -= OnMoveCanceled;
        input.Player.Jump.performed -= OnJumpPerformed;
        input.Player.Attack.performed -= OnAttackPerformed;

        input.Player.Disable();
        input.Dispose();
    }

    private void Update()
    {
        // horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // face movement direction
        if (moveInput.x > 0 && !facingRight) Flip();
        else if (moveInput.x < 0 && facingRight) Flip();

        // jump (edge-triggered)
        if (requestJump && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        requestJump = false;

        // attack (edge-triggered)
        if (requestAttack && canAttack)
        {
            requestAttack = false;
            StartCoroutine(DoAttack());
        }
        else
        {
            requestAttack = false; // consume even if on cooldown
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
    }

    private IEnumerator DoAttack()
    {
        canAttack = false;

        // --- VFX / DEBUG ---
        if (showAttackDebug) StartCoroutine(FlashAttackArea());
        if (flashPlayerOnAttack) StartCoroutine(PlayerFlash());

        if (slashEffectPrefab)
        {
            // spawn slash at attackPoint (or player if missing)
            Vector3 pos = (attackPoint ? attackPoint.position : transform.position) + slashOffset;
            var slash = Instantiate(slashEffectPrefab, pos, Quaternion.identity);

            // flip the slash to face our current facing direction
            var slashT = slash.transform;
            var s = slashT.localScale;
            s.x = Mathf.Abs(s.x) * (facingRight ? 1f : -1f);
            slashT.localScale = s;

            Destroy(slash, slashLifetime);
        }

        // --- DAMAGE ---
        var hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyMask);
        foreach (var h in hits)
        {
            h.GetComponent<IDamageable>()?.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        var s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    // ---------- New Input System callbacks ----------
    private void OnMovePerformed(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => moveInput = Vector2.zero;
    private void OnJumpPerformed(InputAction.CallbackContext ctx) => requestJump = true;
    private void OnAttackPerformed(InputAction.CallbackContext ctx) => requestAttack = true;

    // ---------- Debug / VFX helpers ----------
    private IEnumerator FlashAttackArea()
    {
        float t = 0f;
        const int segments = 20; // circle segments for debug draw
        while (t < debugFlashTime)
        {
            DrawDebugCircle(attackPoint ? attackPoint.position : transform.position, attackRange, Color.red, segments);
            t += Time.deltaTime;
            yield return null;
        }
    }

    private void DrawDebugCircle(Vector3 center, float radius, Color color, int segments)
    {
        Vector3 prev = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float ang = i * Mathf.PI * 2f / segments;
            Vector3 next = center + new Vector3(Mathf.Cos(ang) * radius, Mathf.Sin(ang) * radius, 0f);
            Debug.DrawLine(prev, next, color, Time.deltaTime, false);
            prev = next;
        }
    }

    private IEnumerator PlayerFlash()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (!sr) yield break;

        Color original = sr.color;
        sr.color = playerFlashColor;
        yield return new WaitForSeconds(playerFlashTime);
        sr.color = original;
    }

    // ---------- Gizmos ----------
    private void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
