using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// добавляет State неиспользуемые, их некоторая связка всех остальных скриптов
    /// </summary>
    public static GameManager Instance { get; private set; }

    [Header("ИГРОВЫЕ СИСТЕМЫ")]
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private HighScoreManager highScoreManager;
    [SerializeField] private DifficultyApplier difficultyApplier;

    [Header("GAME OVER UI")]
    [SerializeField] private GameObject gameOverPanel;

    public GameState CurrentState { get; private set; } = GameState.Menu;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        SetState(GameState.Playing);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ✅ 1. Ждём 1 кадр для полной инициализации
        Invoke(nameof(InitializeAfterLoad), 0.1f);
    }

    void InitializeAfterLoad()
    {
        // ✅ 2. СБРОС
        ResetGameplay();

        // ✅ 3. Поиск UI
        FindUITextComponents();
        HideGameOverUI(); // ✅ СКРЫВАЕМ при старте

        // ✅ 4. Запуск игры
        SetState(GameState.Playing);
    }

    void FindUITextComponents()
    {
        if (scoreManager != null) scoreManager.FindUIText();
        if (highScoreManager != null) highScoreManager.FindUIText();
    }

    /// ✅ НОВЫЙ МЕТОД: Показать GameOver UI
    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("📱 GameOver UI показан");
    }

    /// ✅ НОВЫЙ МЕТОД: Спрятать GameOver UI
    public void HideGameOverUI()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged(newState);
    }

    void OnStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                HideGameOverUI(); // ✅ Прятать меню при игре
                break;
            case GameState.Paused:
                Time.timeScale = 1f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }

    void ResetGameplay()
    {
        if (scoreManager != null) scoreManager.ResetScore();
        if (gameTimer != null) gameTimer.ResetTime();
    }

    public void RestartGame()
    {
        // ✅ 1. Сразу ставим Playing для корректной инициализации
        SetState(GameState.Playing);

        // ✅ 2. Ждём кадр, затем рестарт
        Invoke(nameof(DoSceneRestart), 0.05f);
    }

    void DoSceneRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float GameTime => gameTimer?.gameTime ?? 0f;
    public float GetCurrentScore() => scoreManager?.Score ?? 0f;
}
