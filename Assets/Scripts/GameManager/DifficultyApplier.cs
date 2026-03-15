using UnityEngine;

public class DifficultyApplier : MonoBehaviour
{
    public static DifficultyApplier Instance { get; private set; }

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private DifficultyCalculator calculator;

    public float CurrentSpeedMultiplier { get; private set; } = 1f;
    public float CurrentSpawnMultiplier { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();

        if (calculator == null)
            calculator = FindObjectOfType<DifficultyCalculator>();

        RefreshDifficulty();
    }

    private void Update()
    {
        if (calculator == null)
            return;

        RefreshDifficulty();
    }

    public void RefreshDifficulty()
    {
        if (calculator == null)
            return;

        float currentScore = scoreManager != null ? scoreManager.Score : 0f;
        CurrentSpeedMultiplier = calculator.CalculateSpeedMultiplier(currentScore);
        CurrentSpawnMultiplier = calculator.CalculateSpawnMultiplier(currentScore);
    }

    public float GetCurrentSpeedMultiplier() => CurrentSpeedMultiplier;
    public float GetCurrentSpawnMultiplier() => CurrentSpawnMultiplier;
}