using UnityEngine;

public class DifficultyApplier : MonoBehaviour
{
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private DifficultyCalculator calculator;
    [SerializeField] private SpikeSpawnTimer spawnerTimer; // ✅ Изменено!

    private float speedMultiplier;
    private float spawnRateMultiplier;

    void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing) return;

        float normalizedTime = gameTimer.gameTime / 60f;
        speedMultiplier = Mathf.Pow(1.3f, normalizedTime * calculator.difficultyCurvePower);
        spawnRateMultiplier = Mathf.Pow(0.85f, normalizedTime * calculator.difficultyCurvePower);

        speedMultiplier = Mathf.Clamp(speedMultiplier, 1f, 3f);
        spawnRateMultiplier = Mathf.Clamp(spawnRateMultiplier, 0.3f, 1f);

        // ✅ ПРЯМО к SpikeSpawnTimer
        if (spawnerTimer != null)
            spawnerTimer.UpdateRates(spawnRateMultiplier);

        Debug.Log($"⏱️ {gameTimer.gameTime:F1}s | ⚡ {speedMultiplier:F2}x | ⏱️ {spawnRateMultiplier:F2}x");
    }

    public float GetCurrentSpeedMultiplier() => speedMultiplier;
}
