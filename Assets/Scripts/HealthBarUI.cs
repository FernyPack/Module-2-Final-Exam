using UnityEngine;
using UnityEngine.UI;

// Health bar built from UI GameObjects. Updates on HealthChanged.
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private HealthSO health;
    [SerializeField] private Vector2 size = new Vector2(360f, 32f);
    [SerializeField] private Vector2 topLeftOffset = new Vector2(20f, -95f);
    [SerializeField] private float animateSpeed = 6f;

    [SerializeField] private Color fullColor = new Color(0.25f, 0.85f, 0.35f);
    [SerializeField] private Color midColor = new Color(0.95f, 0.8f, 0.2f);
    [SerializeField] private Color lowColor = new Color(0.9f, 0.2f, 0.2f);

    RectTransform fillRect;
    Image fillImage;
    RectTransform[] dividers;
    float shownFraction = 1f;
    float targetFraction = 1f;

    void Awake()
    {
        BuildBar();
    }

    void OnEnable()
    {
        health.HealthChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        health.HealthChanged -= OnHealthChanged;
    }

    void Start()
    {
        targetFraction = shownFraction = health.Fraction;
        Apply(shownFraction);
    }

    void OnHealthChanged(float current, float max)
    {
        targetFraction = max <= 0f ? 0f : current / max;
    }

    void Update()
    {
        float t = 1f - Mathf.Exp(-animateSpeed * Time.deltaTime);
        shownFraction = Mathf.Lerp(shownFraction, targetFraction, t);
        if (Mathf.Abs(shownFraction - targetFraction) < 0.001f) shownFraction = targetFraction;
        Apply(shownFraction);
    }

    void Apply(float fraction)
    {
        fillRect.anchorMax = new Vector2(fraction, 1f);
        fillImage.color = fraction > 0.5f
            ? Color.Lerp(midColor, fullColor, (fraction - 0.5f) * 2f)
            : Color.Lerp(lowColor, midColor, fraction * 2f);
    }

    void BuildBar()
    {
        GameObject canvasGO = new GameObject("HealthBarCanvas", typeof(Canvas), typeof(CanvasScaler));
        canvasGO.transform.SetParent(transform, false);
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        RectTransform border = MakeRect("Border", canvasGO.transform, new Color(0f, 0f, 0f, 0.85f));
        border.anchorMin = border.anchorMax = new Vector2(0f, 1f);
        border.pivot = new Vector2(0f, 1f);
        border.anchoredPosition = topLeftOffset;
        border.sizeDelta = size + new Vector2(6f, 6f);

        RectTransform back = MakeRect("Background", border, new Color(0.25f, 0.05f, 0.05f, 1f));
        Stretch(back, 3f);

        fillRect = MakeRect("Fill", back, fullColor);
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = fillRect.offsetMax = Vector2.zero;
        fillImage = fillRect.GetComponent<Image>();

        // Quarter markers
        dividers = new RectTransform[3];
        for (int i = 0; i < 3; i++)
        {
            RectTransform d = MakeRect("Divider" + (i + 1), back, new Color(0f, 0f, 0f, 0.7f));
            float x = (i + 1) * 0.25f;
            d.anchorMin = new Vector2(x, 0f);
            d.anchorMax = new Vector2(x, 1f);
            d.pivot = new Vector2(0.5f, 0.5f);
            d.sizeDelta = new Vector2(3f, 0f);
            d.anchoredPosition = Vector2.zero;
            dividers[i] = d;
        }
    }

    static RectTransform MakeRect(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return go.GetComponent<RectTransform>();
    }

    static void Stretch(RectTransform rt, float inset)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(inset, inset);
        rt.offsetMax = new Vector2(-inset, -inset);
    }
}
