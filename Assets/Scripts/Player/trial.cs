using UnityEngine;

public class trial : MonoBehaviour
{
    private TrailRenderer body;
    private GameObject Player;

   private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("FliPheus");
        body = GetComponent<TrailRenderer>();
    }
   

    void Update()
    {
        body.transform.position = Player.transform.position;
    }
}
