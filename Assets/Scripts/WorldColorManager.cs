using UnityEngine;

public class WorldColorManager : MonoBehaviour
{
    public static WorldColorManager Instance;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer groundRenderer;
    [SerializeField] private float transitionSpeed = 2f;

    private Color _targetBackground;
    private Color _targetGround;

    void Awake()
    {
        Instance = this;
        _targetBackground = mainCamera.backgroundColor;
        _targetGround = groundRenderer.color;
    }

    void Update()
    {
        mainCamera.backgroundColor = Color.Lerp(
            mainCamera.backgroundColor,
            _targetBackground,
            transitionSpeed * Time.deltaTime
        );

        groundRenderer.color = Color.Lerp(
            groundRenderer.color,
            _targetGround,
            transitionSpeed * Time.deltaTime
        );
    }

    public void SetColors(Color background, Color ground)
    {
        _targetBackground = background;
        _targetGround = ground;
    }
}