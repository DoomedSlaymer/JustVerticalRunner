using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [Header("UI (опционально)")]
    [SerializeField] public TMP_Text scoreText;

    public float currentScore;
    private TMP_Text cachedScoreText;

    public float Score => currentScore;

    void Start()
    {
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
        ResetScore();
    }

    public void FindUIText()
    {
        cachedScoreText = scoreText ? scoreText : GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
    }

    void UpdateUI()
    {
        if (cachedScoreText != null)
            cachedScoreText.text = $"{translator.ScoreLabel}\n{Mathf.FloorToInt(currentScore):N0}";

        DayNightCycle.Instance?.UpdateScore(currentScore);
    }

    public void AddPoints(float points)
    {
        currentScore += points * 2;
        UpdateUI();
    }

    public void ResetScore()
    {
        currentScore = 0f;
        UpdateUI();
    }
}
