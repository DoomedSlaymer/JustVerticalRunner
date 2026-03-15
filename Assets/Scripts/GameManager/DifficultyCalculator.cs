using UnityEngine;

public class DifficultyCalculator : MonoBehaviour
{
    [Header("Speed By Score")]
    [SerializeField] private float scoreForMaxSpeed = 220f;
    [SerializeField] private float minSpeedMultiplier = 1f;
    [SerializeField] private float maxSpeedMultiplier = 2.35f;
    [SerializeField] private float speedRampPower = 0.8f;

    [Header("Spike Spawn By Score")]
    [SerializeField] private float scoreForMinSpawnInterval = 220f;
    [SerializeField] private float maxSpawnMultiplier = 1.2f;
    [SerializeField] private float minSpawnMultiplier = 0.55f;
    [SerializeField] private float spawnRampPower = 0.72f;

    public float CalculateSpeedMultiplier(float score)
    {
        float progress = GetProgress(score, scoreForMaxSpeed, speedRampPower);
        return Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, progress);
    }

    public float CalculateSpawnMultiplier(float score)
    {
        float progress = GetProgress(score, scoreForMinSpawnInterval, spawnRampPower);
        return Mathf.Lerp(maxSpawnMultiplier, minSpawnMultiplier, progress);
    }

    private float GetProgress(float score, float targetScore, float rampPower)
    {
        if (targetScore <= 0f)
            return 1f;

        float normalized = Mathf.Clamp01(score / targetScore);
        float curved = Mathf.Pow(normalized, Mathf.Max(0.01f, rampPower));
        return Mathf.SmoothStep(0f, 1f, curved);
    }

    private void OnValidate()
    {
        maxSpeedMultiplier = Mathf.Max(maxSpeedMultiplier, minSpeedMultiplier);
        maxSpawnMultiplier = Mathf.Max(maxSpawnMultiplier, minSpawnMultiplier);
        minSpawnMultiplier = Mathf.Clamp(minSpawnMultiplier, 0.2f, maxSpawnMultiplier);
        scoreForMaxSpeed = Mathf.Max(1f, scoreForMaxSpeed);
        scoreForMinSpawnInterval = Mathf.Max(1f, scoreForMinSpawnInterval);
        speedRampPower = Mathf.Max(0.01f, speedRampPower);
        spawnRampPower = Mathf.Max(0.01f, spawnRampPower);
    }
}