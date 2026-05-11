using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
  private Rigidbody2D _rb;
  [SerializeField] private float _speed;

  private void CheckPlayer()
  {
    if (GameObject.FindGameObjectWithTag("Player") == null)
    {
      Time.timeScale = 0f;
      FindObjectOfType<RestartRunner>().Init(2f);
    }
  }

  private void Start()
  {
    _rb = GetComponent<Rigidbody2D>();
  }

  private void FixedUpdate()
  {
    _rb.linearVelocity = new Vector2(_speed, _rb.linearVelocity.y);
  }
}