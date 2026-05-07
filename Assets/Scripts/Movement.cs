using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
  private Rigidbody2D _rb;
  [SerializeField] private float _speed;

  private void Start()
  {
    _rb = GetComponent<Rigidbody2D>();
  }

  private void FixedUpdate()
  {
    _rb.linearVelocityX = _speed;
  }
}
