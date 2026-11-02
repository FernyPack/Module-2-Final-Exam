using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Tracks survival time, draws the HUD text and restarts the game.
// It never gets called by the player: it listens for the Health asset's Depleted event.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private HealthSO playerHealth;
    public float restartDelay = 1.5f;

    public bool IsGameOver { get; private set; }

    float survivalTime;
    GUIStyle hudStyle;
    GUIStyle bigStyle;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        playerHealth.Depleted += HandleHealthDepleted;
    }

    void OnDisable()
    {
        playerHealth.Depleted -= HandleHealthDepleted;
    }

    void Update()
    {
        if (IsGameOver) return;
        survivalTime += Time.deltaTime;   // real seconds, whatever the frame rate
    }

    // 4th obstacle hit -> health reaches 0 -> Depleted event -> restart
    void HandleHealthDepleted()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        StartCoroutine(RestartAfterDelay());
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnGUI()
    {
        if (hudStyle == null)
        {
            hudStyle = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold };
            hudStyle.normal.textColor = Color.white;
            bigStyle = new GUIStyle(hudStyle) { fontSize = 48, alignment = TextAnchor.MiddleCenter };
        }

        GUI.Label(new Rect(20, 15, 500, 40),
            "Health: " + playerHealth.CurrentHealth.ToString("0") + " / " + playerHealth.MaxHealth.ToString("0"), hudStyle);
        GUI.Label(new Rect(20, 50, 500, 40), "Time: " + survivalTime.ToString("0.0") + "s", hudStyle);

        if (IsGameOver)
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "GAME OVER\nrestarting...", bigStyle);
    }
}
