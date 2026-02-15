using UnityEngine;

public class ScoreUpdater : MonoBehaviour
{
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private DifficultyCalculator difficulty;
    [SerializeField] private int basePointsPerSecond = 10;

    private float speedMultiplier;

    void Update() // ✅ Изменено с InvokeRepeating на Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing) return;

        // ✅ РАСЧЕТ speedMultiplier каждый кадр
        speedMultiplier = difficulty.CalculateSpeedMultiplier(gameTimer.gameTime);

        // ✅ ОЧКИ = base * speedMultiplier * deltaTime
        float pointsThisFrame = basePointsPerSecond * speedMultiplier * Time.deltaTime;
        scoreManager.AddPoints(pointsThisFrame);
    }

    public float GetSpeedMultiplier() => speedMultiplier;
}
