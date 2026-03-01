using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverUIManager : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    void Start()
    {
        ShowGameOver();
        // Подписка на кнопки
        if (restartButton) restartButton.onClick.AddListener(OnRestartClick);
        if (menuButton) menuButton.onClick.AddListener(OnMenuClick);
    }

    public void ShowGameOver()
    {
        UpdateScoreUI();
        gameOverPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        gameOverPanel.SetActive(false);
    }

    void UpdateScoreUI()
    {
        // Текущий счёт
        if (currentScoreText && GameManager.Instance)
            currentScoreText.text = $"SCORE\n{Mathf.FloorToInt(GameManager.Instance.GetCurrentScore()):N0}";

        // Лучший счёт
        if (highScoreText)
        {
            HighScoreManager hsManager = FindObjectOfType<HighScoreManager>();
            highScoreText.text = $"BEST\n{hsManager?.GetHighScore():N0}";
        }
    }

    void OnRestartClick()
    {
        GameManager.Instance.RestartGame();
    }

    void OnMenuClick()
    {
        SceneManager.LoadScene("MainMenu"); // ИЛИ твоё имя меню
    }
}
    