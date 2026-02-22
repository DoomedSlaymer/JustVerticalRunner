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
        // ✅ УБРАНО SetState(Playing) - только после полной инициализации!
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

        // ✅ 4. Запуск игры
        SetState(GameState.Playing);
    }

    void FindUITextComponents()
    {
        if (scoreManager != null) scoreManager.FindUIText();
        if (highScoreManager != null) highScoreManager.FindUIText();
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
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float GameTime => gameTimer?.gameTime ?? 0f;
    public float GetCurrentScore() => scoreManager?.Score ?? 0f;
}
