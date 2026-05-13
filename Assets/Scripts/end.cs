using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class end : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("FliPheus"))
        {
            SceneManager.LoadScene(0);
        }
    }
}