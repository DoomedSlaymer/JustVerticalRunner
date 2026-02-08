using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    [Header("ДИНАМИЧЕСКИЕ ПАРАМЕТРЫ")]
    [SerializeField] private float baseMinInterval = 0.75f;
    [SerializeField] private float baseMaxInterval = 1.5f;

    private float currentMinInterval;
    private float currentMaxInterval;

    [Header("ПРЕФАБЫ (3 типа опасностей)")]
    [SerializeField] private GameObject spikePrefab;        // Обычный шип
    [SerializeField] private GameObject fastSpikePrefab;    // Быстрый шип
    [SerializeField] private GameObject doubleSpikePrefab;  // Двойной шип

    [Header("КООРДИНАТЫ СТЕН")]
    [SerializeField] private float leftWallX = -4f;    // Левая стена
    [SerializeField] private float rightWallX = 4f;    // Правая стена

    [Header("СПАВН")]
    [SerializeField] private float spawnY = 15f;       // Выше камеры

    [Header("БАЛАНСИРОВКА СТОРОН")]
    [SerializeField] private float oppositeSideBoost = 0.7f; // Шанс на противоположную сторону

    private float timer;
    private bool lastSpawnedLeft = true; // Последний спавн слева (начальное значение)

    void Start()
    {
        currentMinInterval = baseMinInterval;
        currentMaxInterval = baseMaxInterval;
        timer = Random.Range(currentMinInterval, currentMaxInterval);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnRandomSpike();
            timer = Random.Range(currentMinInterval, currentMaxInterval);
        }
    }
    void SpawnRandomSpike()
    {
        // ✅ РАНДОМНЫЙ ВЫБОР ТИПА ОБЪЕКТА
        GameObject prefab = GetRandomPrefab();

        // ✅ БАЛАНСИРОВКА: повышенный шанс на противоположную сторону
        bool spawnLeft;
        if (lastSpawnedLeft)
        {
            // После левой → больше шанс на правую
            spawnLeft = Random.value > oppositeSideBoost; // 30% левая, 70% правая
        }
        else
        {
            // После правой → больше шанс на левую
            spawnLeft = Random.value <= oppositeSideBoost; // 70% левая, 30% правая
        }

        // ✅ СПАВН с ПРАВИЛЬНЫМ РАЗВОРОТОМ
        if (spawnLeft)
        {
            SpawnSpike(prefab, leftWallX, spawnY, -90f);  // Левая: ← смотрит вправо
            lastSpawnedLeft = true;
        }
        else
        {
            SpawnSpike(prefab, rightWallX, spawnY, 90f);  // Правая: → смотрит влево
            lastSpawnedLeft = false;
        }
    }

    GameObject GetRandomPrefab()
    {
        // Равный шанс на любой тип (можно настроить веса)
        int type = Random.Range(0, 3);
        return type switch
        {
            0 => spikePrefab,
            1 => fastSpikePrefab,
            _ => doubleSpikePrefab
        };
    }

    void SpawnSpike(GameObject prefab, float x, float y, float rotation)
    {
        Vector3 pos = new Vector3(x, y, 0);
        GameObject spike = Instantiate(prefab, pos, Quaternion.Euler(0, 0, rotation));
        spike.transform.SetParent(transform);
    }
    public void UpdateSpawnRates(float multiplier)
    {
        currentMinInterval = baseMinInterval * multiplier;
        currentMaxInterval = baseMaxInterval * multiplier;
        currentMinInterval = Mathf.Max(currentMinInterval, 0.2f); // Минимум
    }
}
