using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HighScoreManager : MonoBehaviour
{
    /// <summary>
    /// Сохраняет лучший рекорд в ключе?
    /// </summary>
    [Header("UI (опционально)")]
    [SerializeField] public TMP_Text highScoreText;

    private const string HIGH_SCORE_KEY = "HighScoreRunner";
    private int highScore;
    private TMP_Text cachedHighScoreText;

    void Start()
    {
        LoadHighScore();
        FindUIText();
        UpdateUI();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindUIText();
    }

    public void FindUIText()
    {
        cachedHighScoreText = highScoreText ? highScoreText : GameObject.Find("HighScoreText")?.GetComponent<TMP_Text>();
    }

    void UpdateUI()
    {
        if (cachedHighScoreText != null)
            cachedHighScoreText.text = $"BEST\n{highScore:N0}";
    }

    void Update()
    {
        CheckNewHighScore();
        UpdateUI();
    }

    public void CheckNewHighScore()
    {
        if (GameManager.Instance != null)
        {
            int currentScore = Mathf.FloorToInt(GameManager.Instance.GetCurrentScore());
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
                PlayerPrefs.Save();
            }
        }
    }

    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    public int GetHighScore() => highScore;
}
