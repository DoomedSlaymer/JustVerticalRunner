using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
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
    [SerializeField] private SpikeSpawnTimer spawnTimer;

    private SpikeSpawnSideSelector sideSelector;
    private SpikePrefabSelector prefabSelector;
    private SpikePool spikePool;
    private bool hasLoggedMissingPool;

    private void Start()
    {
        if (spawnTimer == null)
            spawnTimer = GetComponent<SpikeSpawnTimer>();

        if (spawnTimer == null)
        {
            spawnTimer = gameObject.AddComponent<SpikeSpawnTimer>();
            //Debug.LogWarning("SpikeSpawner: SpikeSpawnTimer не найден, компонент добавлен автоматически.", this);
        }

        sideSelector = GetComponent<SpikeSpawnSideSelector>();
        if (sideSelector == null)
            sideSelector = gameObject.AddComponent<SpikeSpawnSideSelector>();

        prefabSelector = GetComponent<SpikePrefabSelector>();
        if (prefabSelector == null)
            prefabSelector = gameObject.AddComponent<SpikePrefabSelector>();

        sideSelector.SetSpawnerData(leftWallX, rightWallX, oppositeSideBoost);
        prefabSelector.SetSpawnerData(spikePrefab, fastSpikePrefab, doubleSpikePrefab);

        spikePool = FindObjectOfType<SpikePool>();
        ValidateConfiguration();
    }

    private void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing || spawnTimer == null)
            return;

        if (spawnTimer.ShouldSpawn)
        {
            SpawnRandomSpike();
            spawnTimer.ResetTimer();
        }
    }

    private void SpawnRandomSpike()
    {
        GameObject prefab = prefabSelector.GetRandomPrefab();
        if (prefab == null)
        {
            Debug.LogWarning("SpikeSpawner: не назначен один из spike prefab, спавн пропущен.", this);
            return;
        }

        var (x, rotation) = sideSelector.GetSpawnPosition();

        Vector3 pos = new Vector3(x, spawnY, 0f);

        if (spikePool != null)
        {
            GameObject spike = spikePool.SpawnFromPool(prefab, pos, Quaternion.Euler(0f, 0f, rotation));
            if (spike != null)
            {
                spike.transform.SetParent(transform);
            }
            else
            {
                Debug.LogWarning($"SpikeSpawner: пул не смог выдать объект для {prefab.name}. Проверь pools в SpikePool.", this);
            }
        }
        else
        {
            if (!hasLoggedMissingPool)
            {
                hasLoggedMissingPool = true;
                Debug.LogWarning("SpikeSpawner: SpikePool не найден, используется Instantiate.", this);
            }

            Instantiate(prefab, pos, Quaternion.Euler(0f, 0f, rotation), transform);
        }
    }

    private void ValidateConfiguration()
    {
        if (spikePrefab == null || fastSpikePrefab == null || doubleSpikePrefab == null)
            Debug.LogWarning("SpikeSpawner: назначены не все spike prefab.", this);

        if (spikePool == null)
            Debug.LogWarning("SpikeSpawner: SpikePool не найден на сцене.", this);
    }
}
