using UnityEngine;

public class SpikeSpawnTimer : MonoBehaviour
{
    [SerializeField] private float baseMinInterval = 0.75f;
    [SerializeField] private float baseMaxInterval = 1.5f;

    private float timer;

    public bool ShouldSpawn => timer <= 0f;

    public void ResetTimer()
    {
        DifficultyApplier.Instance?.RefreshDifficulty();

        float spawnMultiplier = DifficultyApplier.Instance != null
            ? DifficultyApplier.Instance.CurrentSpawnMultiplier
            : 1f;

        float minInterval = Mathf.Max(baseMinInterval * spawnMultiplier, 0.25f);
        float maxInterval = Mathf.Max(baseMaxInterval * spawnMultiplier, minInterval + 0.05f);

        timer = Random.Range(minInterval, maxInterval);
    }

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing)
            return;

        timer -= Time.deltaTime;
    }
}