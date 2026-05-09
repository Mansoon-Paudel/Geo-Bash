using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 0f;
    [SerializeField] private float jumpForce = 25f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float rotationSpeed = 180f;

    private Rigidbody2D _rb;
    private Movement[] _movements;

    private bool _isGrounded;
    private bool _isDead;
    private bool _shouldJump;
    private float _currentAngle;
    private bool _isSnapping;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = gravityScale;
        _rb.freezeRotation = true;
        _movements = FindObjectsOfType<Movement>();
    }

    void Update()
    {
        if (_isDead) return;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && _isGrounded)
            _shouldJump = true;

        if (!_isGrounded)
        { 
            _currentAngle -= rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle);
        }
    }

    void FixedUpdate()
    {
        if (_isDead) return;

        if (_shouldJump && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(speed, jumpForce);
            _shouldJump = false;
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
            if (contact.normal.y > 0.5f)
            {
                _isGrounded = true;
                if (!_isSnapping)
                    StartCoroutine(SnapRotation());

                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        _isGrounded = false;
    }

    private IEnumerator SnapRotation()
    {
        _isSnapping = true;

        float targetAngle = Mathf.Round(_currentAngle / 90f) * 90f;
        float snapDuration = 0.08f; 
        float elapsed = 0f;
        float startAngle = _currentAngle;

        while (elapsed < snapDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / snapDuration);
            _currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
            transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle);
            yield return null;
        }
        _currentAngle = targetAngle;
        transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle);

        _isSnapping = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) Die();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        foreach (var movement in _movements)
            if (movement != null) movement.enabled = false;

        _rb.linearVelocity = Vector2.zero;
        _rb.simulated = false;
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
        Destroy(gameObject);
    }
}