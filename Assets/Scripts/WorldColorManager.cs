using UnityEngine;

public class BackgroundColor : MonoBehaviour
{
    [SerializeField] private Color[] colors = new Color[]
    {
        Color.rebeccaPurple,
        Color.darkSlateGray,
        Color.red,
        Color.darkRed,
        Color.gray1,
        Color.white
    };

    [SerializeField] private float transitionSpeed = 1f;
    private SpriteRenderer _sr;
    private int _currentIndex = 0;
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = colors[0];
    }
    void Update()
    {
        _sr.color = Color.Lerp(
            _sr.color,
            colors[_currentIndex],
            transitionSpeed * Time.deltaTime
        );
    }
    public void NextColor()
    {
        _currentIndex = (_currentIndex + 1) % colors.Length;
    }
    public void SetColor(int index)
    {
        _currentIndex = Mathf.Clamp(index, 0, colors.Length - 1);
    }
}