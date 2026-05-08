using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpCutMultiplier = 0.85f;
    [SerializeField] private float groundNormalThreshold = 0.5f;

    private GameObject _walk;
    private Rigidbody2D _rb;
    private Movement[] _movements;

    private bool _isGrounded;
    private float _coyoteTimer = 0f;
    private bool IsGrounded => _isGrounded || _coyoteTimer > 0f;

    private bool _isDead;
    private bool _shouldJump;
    private bool _inputHeld;
    private float _currentAngle;
    private bool _wasGrounded;

    void Start()
    {
        _walk = transform.Find("Particles")?.gameObject;
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = gravityScale;
        _rb.freezeRotation = true;
        _movements = FindObjectsOfType<Movement>();
    }

    void Update()
    {
        if (_isDead) return;

        _inputHeld = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && IsGrounded)
            _shouldJump = true;

        if (_isGrounded)
            _coyoteTimer = coyoteTime;
        else
            _coyoteTimer -= Time.deltaTime;

        bool grounded = IsGrounded;

        if (!grounded)
        {
            _currentAngle -= speed * Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Euler(0, 0, _currentAngle);
        }
        else if (!_wasGrounded && grounded)
        {
            _currentAngle = Mathf.Round(_currentAngle / 90f) * 90f;
            transform.rotation = Quaternion.Euler(0, 0, _currentAngle);
        }

        _wasGrounded = grounded;

        _walk?.SetActive(grounded);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void FixedUpdate()
    {
        if (_isDead) return;

        if (!_inputHeld && _rb.linearVelocity.y > 0)
            _rb.linearVelocity = new Vector2(speed, _rb.linearVelocity.y * jumpCutMultiplier);

        if (_shouldJump && IsGrounded)
        {
            _rb.linearVelocity = new Vector2(speed, jumpForce);
            _shouldJump = false;
            _coyoteTimer = 0f;
        }
        else
        {
            _rb.linearVelocity = new Vector2(speed, _rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > groundNormalThreshold)
            {
                _isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        _isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) Die();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        _walk?.SetActive(false);
        foreach (var movement in _movements)
            if (movement != null) movement.enabled = false;
        _rb.linearVelocity = Vector2.zero;
        _rb.simulated = false;
        StartCoroutine(DieRoutine());
    }

    private System.Collections.IEnumerator DieRoutine()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
        Destroy(gameObject);
    }
}
