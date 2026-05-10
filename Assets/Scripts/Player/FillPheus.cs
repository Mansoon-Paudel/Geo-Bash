using System;
using UnityEngine;

namespace Player
{
    public class FillPheus : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float up;
        private Rigidbody2D rb;
        private bool isDead;
        private SpriteRenderer sr;
        [SerializeField] private ParticleSystem deathParticles;
        [SerializeField] private float restartDelay;
        private bool goingUp;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            GivingInput();
            DieTrigger();
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
                Die();
        }

        private void DieTrigger()
        {
            if (rb.linearVelocity.x < speed - 0.1)
            {
                Die();
            }

            if (goingUp == true)
            {
                if (rb.linearVelocity.y < up - 1)
                {
                    Die();
                }
            }
            else
            {
                if (rb.linearVelocity.y > -up + 1)
                {
                    Die();
                }
            }

        }

        private void GivingInput()
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            {
                rb.rotation = 45f;
                rb.linearVelocity = new Vector2(speed, up);
                goingUp = true;
            }
            else
            {
                rb.rotation = -45f;
                rb.linearVelocity = new Vector2(speed, -up);
                goingUp = false;
            }
        }
    }
}