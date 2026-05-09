using UnityEngine;
using System.Collections;

public class RestartRunner : MonoBehaviour
{
    public void Init(float delay)
    {
        StartCoroutine(Run(delay));
    }

    private IEnumerator Run(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
        Destroy(gameObject);
    }
}
