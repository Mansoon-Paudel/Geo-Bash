using System;
using UnityEngine;

public class FlipPheus : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool Flipped;
    private bool Rotated;
    private bool Grounded;
    private bool isDead;
    private SpriteRenderer sr;
    [SerializeField] private ParticleSystem groundParticles;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private float restartDelay;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Flip();
        DieTrigger();
    }
    void Flip()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Grounded)
        {
            Flipped = !Flipped;
            rb.gravityScale *= -1;
            if (Flipped == true)
            {
                transform.rotation = new Quaternion(
                    transform.rotation.x,
                    transform.rotation.y,
                    180,
                    transform.rotation.w);
            }
            else
            {
                transform.rotation = new Quaternion(
                    transform.rotation.x,
                    transform.rotation.y,
                    0,
                    transform.rotation.w);
            }
        }
    }
    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (sr != null) sr.enabled = false;
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
    private void DieTrigger()
    {
        if (rb.linearVelocity.x < -0.2)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Die();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
       Debug.Log(other.gameObject.tag); 
        if (other.gameObject.CompareTag("Ground"))
        {
            
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Grounded = false;
        }
    }
}
