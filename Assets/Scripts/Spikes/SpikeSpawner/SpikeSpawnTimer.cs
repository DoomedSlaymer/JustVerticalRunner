using UnityEngine;

public class SpikeSpawnTimer : MonoBehaviour
{
    [SerializeField] private float baseMinInterval = 0.75f;
    [SerializeField] private float baseMaxInterval = 1.5f;

    private float currentMinInterval;
    private float currentMaxInterval;
    private float timer;

    public bool ShouldSpawn => timer <= 0f;
    public void ResetTimer() => timer = Random.Range(currentMinInterval, currentMaxInterval);

    void Start()
    {
        currentMinInterval = baseMinInterval;
        currentMaxInterval = baseMaxInterval;
        ResetTimer();
    }

    void Update()
    {
        timer -= Time.deltaTime;
    }

    public void UpdateRates(float multiplier)
    {
        currentMinInterval = Mathf.Max(baseMinInterval * multiplier, 0.2f);
        currentMaxInterval = Mathf.Max(baseMaxInterval * multiplier, 0.4f);
    }
}
