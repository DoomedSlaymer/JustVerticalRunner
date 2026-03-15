using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, WaitingToStart, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("ИГРОВЫЕ СИСТЕМЫ")]
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private HighScoreManager highScoreManager;
    [SerializeField] private DifficultyApplier difficultyApplier;

    [Header("GAME OVER UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameOverUIManager gameOverUIManager;

    public GameState CurrentState { get; private set; } = GameState.Menu;

    private void Awake()
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        SetState(GameState.WaitingToStart);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Invoke(nameof(InitializeAfterLoad), 0.1f);
    }

    private void InitializeAfterLoad()
    {
        ResetGameplay();
        FindUITextComponents();
        HideGameOverUI();
        SetState(GameState.WaitingToStart);
    }

    private void FindUITextComponents()
    {
        if (scoreManager != null)
            scoreManager.FindUIText();

        if (highScoreManager != null)
            highScoreManager.FindUIText();
    }

    public void ShowGameOverUI()
    {
        GameOverUIManager uiManager = ResolveGameOverUIManager();
        if (uiManager != null)
        {
            uiManager.ShowGameOver();
            return;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void HideGameOverUI()
    {
        GameOverUIManager uiManager = ResolveGameOverUIManager();
        if (uiManager != null)
        {
            uiManager.HideGameOver();
            return;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged(newState);
    }

    public void StartGameplay()
    {
        if (CurrentState == GameState.WaitingToStart)
            SetState(GameState.Playing);
    }

    private void OnStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.WaitingToStart:
                Time.timeScale = 1f;
                HideGameOverUI();
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                HideGameOverUI();
                break;
            case GameState.Paused:
                Time.timeScale = 1f;
                break;
            case GameState.GameOver:
                Time.timeScale = 1f;
                ShowGameOverUI();
                break;
        }
    }

    private void ResetGameplay()
    {
        if (scoreManager != null)
            scoreManager.ResetScore();

        if (gameTimer != null)
            gameTimer.ResetTime();
    }

    public void RestartGame()
    {
        SetState(GameState.WaitingToStart);
        Invoke(nameof(DoSceneRestart), 0.05f);
    }

    private void DoSceneRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float GameTime => gameTimer?.gameTime ?? 0f;
    public float GetCurrentScore() => scoreManager?.Score ?? 0f;

    private GameOverUIManager ResolveGameOverUIManager()
    {
        if (gameOverUIManager != null)
            return gameOverUIManager;

        if (gameOverPanel != null)
        {
            gameOverUIManager = gameOverPanel.GetComponent<GameOverUIManager>();
            if (gameOverUIManager == null)
                gameOverUIManager = gameOverPanel.GetComponentInChildren<GameOverUIManager>(true);
        }

        if (gameOverUIManager == null)
            gameOverUIManager = FindObjectOfType<GameOverUIManager>(true);

        return gameOverUIManager;
    }
}
