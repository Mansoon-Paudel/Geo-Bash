using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravityScale = 3f;

    private Rigidbody2D _rb;
    private Animator _anim;
    private bool _isGrounded = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = gravityScale;
        _rb.freezeRotation = true;
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        _anim.SetBool("Jump", _isGrounded);

        bool inputHeld = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
        bool halfJump = _isGrounded && !inputHeld && _rb.linearVelocity.y < 0;
        _anim.SetBool("HalfJump", halfJump);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }
    }
    
    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(speed, _rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        _isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        _isGrounded = false;
    }
}
