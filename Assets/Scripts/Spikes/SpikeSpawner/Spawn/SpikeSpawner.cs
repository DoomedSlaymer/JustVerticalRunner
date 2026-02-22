using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    /// <summary>
    /// использует префабы (вероятно не нужно) спавнит шипы из пулла, и использует логику из sideselector и prefabside
    /// </summary>
    [Header("Префабы")]
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private GameObject fastSpikePrefab;
    [SerializeField] private GameObject doubleSpikePrefab;

    [Header("Координаты")]
    [SerializeField] private float leftWallX = -4f;
    [SerializeField] private float rightWallX = 4f;
    [SerializeField] private float spawnY = 15f;

    [Header("Баланс")]
    [SerializeField] private float oppositeSideBoost = 0.7f;

    [Header("Таймер")]
    [SerializeField] private float baseMinInterval = 0.75f;
    [SerializeField] private float baseMaxInterval = 1.5f;

    private float currentMinInterval;
    private float currentMaxInterval;
    private float timer;
    private SpikeSpawnSideSelector sideSelector;
    private SpikePrefabSelector prefabSelector;
    private SpikePool spikePool;

    void Start()
    {
        currentMinInterval = baseMinInterval;
        currentMaxInterval = baseMaxInterval;
        timer = Random.Range(currentMinInterval, currentMaxInterval);

        sideSelector = gameObject.AddComponent<SpikeSpawnSideSelector>();
        prefabSelector = gameObject.AddComponent<SpikePrefabSelector>();

        sideSelector.SetSpawnerData(leftWallX, rightWallX, oppositeSideBoost);
        prefabSelector.SetSpawnerData(spikePrefab, fastSpikePrefab, doubleSpikePrefab);

        // ✅ НАЙТИ ПУЛ
        spikePool = FindObjectOfType<SpikePool>();
    }

    void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnRandomSpike();
            timer = Random.Range(currentMinInterval, currentMaxInterval);
        }
    }

    void SpawnRandomSpike()
    {
        GameObject prefab = prefabSelector.GetRandomPrefab();
        var (x, rotation) = sideSelector.GetSpawnPosition();

        Vector3 pos = new Vector3(x, spawnY, 0);

        // ✅ ИЗМЕНЕНО: Pool вместо Instantiate
        if (spikePool != null)
        {
            GameObject spike = spikePool.SpawnFromPool(prefab, pos, Quaternion.Euler(0, 0, rotation));
            if (spike != null)
                spike.transform.SetParent(transform);
        }
        else
        {
            // Fallback
            Instantiate(prefab, pos, Quaternion.Euler(0, 0, rotation), transform);
        }
    }

    public void UpdateSpawnRates(float multiplier)
    {
        currentMinInterval = Mathf.Max(baseMinInterval * multiplier, 0.2f);
        currentMaxInterval = Mathf.Max(baseMaxInterval * multiplier, 0.4f);
    }
}
