using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 15f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private float groundCheckDistance = 0.05f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Feel Settings")]
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private float jumpCooldown = 0.15f;

    [Header("Death Settings")]
    [SerializeField] private float xVelocityDeathThreshold = -0.52f;
    [SerializeField] private float restartDelay = 2.5f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem groundParticles;
    [SerializeField] private ParticleSystem deathParticles;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private ParticleSystem.EmissionModule particleEmission;

    private float targetRotation = 0f;
    private bool isGrounded = false;
    private bool isOnBlock = false;
    private bool wasGrounded = false;
    private bool wasOnBlock = false;
    private bool jumpedThisFrame = false;
    private bool isDead = false;

    private float coyoteTimer = 0f;
    private float jumpBufferTimer = 0f;
    private float lastJumpTime = -999f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        if (groundParticles != null)
        {
            var main = groundParticles.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            particleEmission = groundParticles.emission;
        }

        if (deathParticles != null)
        {
            var main = deathParticles.main;
            main.useUnscaledTime = true;
            deathParticles.Simulate(0f, true, true);
            deathParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void Update()
    {
        if (isDead) return;

        wasGrounded = isGrounded;
        wasOnBlock = isOnBlock;
        jumpedThisFrame = false;

        CheckGrounded();
        UpdateTimers();
        HandleJumpInput();
        HandleFallRotation();
        ApplySmoothRotation();
        UpdateParticles();
    }

    private void UpdateParticles()
    {
        if (groundParticles == null) return;

        Bounds b = col.bounds;
        Vector3 offset = new Vector3(-b.extents.x, -b.extents.y, 0f);
        groundParticles.transform.position = transform.position + offset;
        groundParticles.transform.rotation = Quaternion.identity;

        bool isOnAnything = isGrounded || isOnBlock;
        particleEmission.enabled = isOnAnything;
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (rb.linearVelocity.x < xVelocityDeathThreshold)
            Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            Die();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (sr != null) sr.enabled = false;

        if (groundParticles != null)
            particleEmission.enabled = false;

        if (deathParticles != null)
        {
            deathParticles.transform.position = transform.position;
            deathParticles.Play();
        }
   Time.timeScale = 0f;

        GameObject runner = new GameObject("RestartRunner");
        DontDestroyOnLoad(runner);
        runner.AddComponent<RestartRunner>().Init(restartDelay);
        Destroy(gameObject);
    }

    private void CheckGrounded()
    {
        Bounds b = col.bounds;
        float castWidth = b.size.x * 0.8f;
        Vector2 origin = new Vector2(b.center.x, b.min.y);
        Vector2 size = new Vector2(castWidth, 0.02f);

        RaycastHit2D groundHit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D blockHit  = Physics2D.BoxCast(origin, size, 0f, Vector2.down, groundCheckDistance, blockLayer);

        isGrounded = groundHit.collider != null;
        isOnBlock  = blockHit.collider != null;

        bool wasOnAnything = wasGrounded || wasOnBlock;
        bool isOnAnything  = isGrounded  || isOnBlock;

        if (!wasOnAnything && isOnAnything)
        {
            targetRotation = SnapToNearest90(targetRotation);
            transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
        }
    }

    private void UpdateTimers()
    {
        bool wasOnAnything = wasGrounded || wasOnBlock;
        bool isOnAnything  = isGrounded  || isOnBlock;

        if (wasOnAnything && !isOnAnything && !jumpedThisFrame)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer = Mathf.Max(0f, coyoteTimer - Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - Time.deltaTime);
    }

    private void HandleJumpInput()
    {
        bool canJump    = isGrounded || isOnBlock || coyoteTimer > 0f;
        bool shouldJump = jumpBufferTimer > 0f && Time.time - lastJumpTime > jumpCooldown;

        if (canJump && shouldJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            targetRotation -= 180f;
            jumpedThisFrame = true;
            lastJumpTime = Time.time;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }
    }

    private void HandleFallRotation()
    {
        bool justLeftBlock = wasOnBlock && !isOnBlock;

        if (justLeftBlock && !jumpedThisFrame)
            targetRotation -= 90f;
    }

    private void ApplySmoothRotation()
    {
        targetRotation = NormalizeAngle(targetRotation);
        float current = NormalizeAngle(transform.eulerAngles.z);
        float newAngle = Mathf.LerpAngle(current, targetRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f)  angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    private float SnapToNearest90(float angle)
    {
        return Mathf.Round(angle / 90f) * 90f;
    }

    private void OnDrawGizmosSelected()
    {
        if (col == null) return;
        Bounds b = col.bounds;
        Gizmos.color = (isGrounded || isOnBlock) ? Color.green : Color.red;
        Gizmos.DrawWireCube(
            new Vector3(b.center.x, b.min.y - groundCheckDistance, 0f),
            new Vector3(b.size.x * 0.8f, 0.02f, 0f)
        );
    }
}