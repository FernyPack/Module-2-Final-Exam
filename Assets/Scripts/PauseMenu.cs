using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Esc pauses the game. Menu has Resume, Restart and Quit (fades out before quitting).
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float quitFadeDuration = 0.8f;

    public static bool IsPaused { get; private set; }

    CanvasGroup menuGroup;
    Image fadeImage;
    Font font;
    bool isQuitting;
    Coroutine menuFade;

    void Awake()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        EnsureEventSystem();
        BuildMenu();
    }

    void Update()
    {
        if (isQuitting) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        Keyboard kb = Keyboard.current;
        if (kb != null && (kb.escapeKey.wasPressedThisFrame || kb.pKey.wasPressedThisFrame))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        FadeMenu(1f);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        FadeMenu(0f);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        if (isQuitting) return;
        isQuitting = true;
        StartCoroutine(QuitRoutine());
    }

    IEnumerator QuitRoutine()
    {
        menuGroup.interactable = false;
        fadeImage.raycastTarget = true;

        float t = 0f;
        while (t < quitFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(t / quitFadeDuration));
            yield return null;
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void FadeMenu(float target)
    {
        if (menuFade != null) StopCoroutine(menuFade);
        menuFade = StartCoroutine(FadeMenuRoutine(target));
    }

    IEnumerator FadeMenuRoutine(float target)
    {
        bool show = target > 0f;
        menuGroup.blocksRaycasts = show;
        menuGroup.interactable = show;

        float start = menuGroup.alpha;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            menuGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        menuGroup.alpha = target;
    }

    static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null) return;
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    void BuildMenu()
    {
        GameObject canvasGO = new GameObject("PauseCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGO.transform.SetParent(transform, false);
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        // Dimmed background
        RectTransform dim = MakeImage("Menu", canvasGO.transform, new Color(0f, 0f, 0f, 0.6f));
        Stretch(dim);
        menuGroup = dim.gameObject.AddComponent<CanvasGroup>();
        menuGroup.alpha = 0f;
        menuGroup.blocksRaycasts = false;
        menuGroup.interactable = false;

        RectTransform panel = MakeImage("Panel", dim, new Color(0.1f, 0.1f, 0.12f, 0.95f));
        panel.anchorMin = panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(460f, 440f);

        MakeText("Title", panel, "PAUSED", 60, new Vector2(0f, 150f), new Vector2(420f, 80f));
        MakeButton("Resume", panel, new Vector2(0f, 40f), Resume);
        MakeButton("Restart", panel, new Vector2(0f, -50f), Restart);
        MakeButton("Quit", panel, new Vector2(0f, -140f), Quit);
        MakeText("Hint", panel, "Esc to resume", 22, new Vector2(0f, -200f), new Vector2(420f, 30f))
            .color = new Color(1f, 1f, 1f, 0.6f);

        // Full-screen black used for the quit fade
        RectTransform fade = MakeImage("QuitFade", canvasGO.transform, new Color(0f, 0f, 0f, 0f));
        Stretch(fade);
        fadeImage = fade.GetComponent<Image>();
        fadeImage.raycastTarget = false;
    }

    void MakeButton(string label, Transform parent, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        RectTransform rt = MakeImage(label + "Button", parent, Color.white);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(300f, 70f);
        Image img = rt.GetComponent<Image>();
        img.raycastTarget = true;

        Button button = rt.gameObject.AddComponent<Button>();
        button.targetGraphic = img;
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.25f, 0.25f, 0.3f);
        colors.highlightedColor = new Color(0.35f, 0.55f, 0.95f);
        colors.pressedColor = new Color(0.2f, 0.35f, 0.7f);
        colors.selectedColor = colors.normalColor;
        colors.fadeDuration = 0.1f;
        button.colors = colors;
        button.onClick.AddListener(onClick);

        Text text = MakeText("Label", rt, label, 34, Vector2.zero, Vector2.zero);
        Stretch(text.rectTransform);
    }

    Text MakeText(string name, Transform parent, string content, int size, Vector2 pos, Vector2 boxSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        Text text = go.GetComponent<Text>();
        text.text = content;
        text.font = font;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = boxSize;
        return text;
    }

    static RectTransform MakeImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = true;
        return go.GetComponent<RectTransform>();
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}
