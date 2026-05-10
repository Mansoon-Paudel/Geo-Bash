using UnityEngine;

public class ColorTrigger : MonoBehaviour
{
    [SerializeField] private int colorIndex = 1;
    private bool _triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (other.CompareTag("Player")) return;
        if (other.CompareTag("Enemy")) return;

        _triggered = true; 
     foreach (var bg in FindObjectsOfType<BackgroundColor>())
        {
            bg.SetColor(colorIndex);
        } 
    }
}
