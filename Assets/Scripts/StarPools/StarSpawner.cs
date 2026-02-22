using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    [Header("Звезда")]
    [SerializeField] private GameObject starPrefab;

    [Header("Спавн")]
    [SerializeField] private float spawnY = 15f;
    [SerializeField] private float minSpawnInterval = 0.5f;
    [SerializeField] private float maxSpawnInterval = 2f;
    [SerializeField] private float minX = -6f;
    [SerializeField] private float maxX = 6f;

    [Header("Скорость")]
    [SerializeField] private float minStarSpeed = 1f;
    [SerializeField] private float maxStarSpeed = 4f;

    private float spawnTimer;
    private StarPool starPool;

    void Start()
    {
        starPool = FindObjectOfType<StarPool>();
        spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnStar();
            spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnStar()
    {
        if (starPool == null) return;

        float x = Random.Range(minX, maxX);
        Vector3 position = new Vector3(x, spawnY, 0f);
        float speed = Random.Range(minStarSpeed, maxStarSpeed);

        starPool.SpawnStar(position, speed);
    }
}
