using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private GameObject groundPlayer;
    [SerializeField] private GameObject flyingPlayer;

    private void Start()
    {
        if (groundPlayer == null)
            groundPlayer = GameObject.Find("BoXPheus");

        if (flyingPlayer == null)
            flyingPlayer = GameObject.Find("FlYPheus");

        if (groundPlayer == null || flyingPlayer == null)
            Debug.LogError("Portal: One or both player references are missing! Make sure names match or assign in Inspector.", this);
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        Debug.Log("Portal: Trigger hit by: " + other.gameObject.name, this);

        if (!other.CompareTag("Player"))
            return;

        if (groundPlayer == null || flyingPlayer == null)
        {
         return;
        }
        Vector3 targetPos = transform.position;
        targetPos.z = flyingPlayer.transform.position.z;
        flyingPlayer.transform.position = targetPos;

        groundPlayer.SetActive(false);
        flyingPlayer.SetActive(true);
    }
}