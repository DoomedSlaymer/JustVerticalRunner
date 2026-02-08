using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    [Header("Префабы стен")]
    [SerializeField] private GameObject leftWallPrefab;
    [SerializeField] private GameObject rightWallPrefab;

    [Header("Позиции спавна (координата X)")]
    [SerializeField] private float leftWallX = -8f;
    [SerializeField] private float rightWallX = 8f;

    [Header("Параметры спавна")]
    [SerializeField] private float spawnY = 12f;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int initialWallsCount = 3;

    [Header("Движение")]
    [SerializeField] private float wallSpeed = 5f;

    [Header("Границы экрана")]
    [SerializeField] private float destroyY = -12f;

    private float spawnTimer;

    /// <summary>
    /// Инициализация спавнера при старте сцены
    /// </summary>
    private void Start()
    {
        spawnTimer = spawnInterval;
        SpawnInitialWalls();  // Спавним начальные стены
    }

    /// <summary>
    /// ✅ ОСНОВНОЙ ЦИКЛ - ВОЗВРАЩЕН MoveWallsDown()
    /// </summary>
    private void Update()
    {
        SpawnWalls();           // Проверяем необходимость спавна
        MoveWallsDown();        // ✅ ДВИГАЕМ СТЕНЫ ВНИЗ (БЫЛО УБРАНО!)
        CleanupWalls();         // Удаляем стены за экраном
    }

    /// <summary>
    /// ✅ ДВИЖЕНИЕ ВСЕХ СТЕН ВНИЗ - КРИТИЧЕСКИЙ МЕТОД
    /// </summary>
    private void MoveWallsDown()
    {
        WallMover[] walls = FindObjectsOfType<WallMover>();
        foreach (WallMover wall in walls)
        {
            if (wall != null)
            {
                wall.MoveDown();  // Вызываем движение каждой стены
            }
        }
    }

    /// <summary>
    /// Спавн начальных стен в начале сцены
    /// </summary>
    private void SpawnInitialWalls()
    {
        for (int i = 0; i < initialWallsCount; i++)
        {
            float offsetY = i * spawnInterval;
            SpawnWallPairAtY(spawnY - offsetY);
        }
    }

    /// <summary>
    /// Спавн пары стен на определенной высоте Y
    /// </summary>
    private void SpawnWallPairAtY(float yPosition)
    {
        CreateLeftWallAtY(yPosition);
        CreateRightWallAtY(yPosition);
    }

    /// <summary>
    /// Спавн пары стен (для таймера)
    /// </summary>
    private void SpawnWalls()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnWallPairAtY(spawnY);
            spawnTimer = spawnInterval;
        }
    }

    /// <summary>
    /// Создание левой стены
    /// </summary>
    private void CreateLeftWallAtY(float yPosition)
    {
        if (leftWallPrefab != null)
        {
            Vector3 position = new Vector3(leftWallX, yPosition, 0f);
            GameObject leftWall = Instantiate(leftWallPrefab, position, Quaternion.identity);
            WallMover leftMover = leftWall.GetComponent<WallMover>() ?? leftWall.AddComponent<WallMover>();
            leftMover.Initialize(wallSpeed, destroyY);
        }
    }

    /// <summary>
    /// Создание правой стены
    /// </summary>
    private void CreateRightWallAtY(float yPosition)
    {
        if (rightWallPrefab != null)
        {
            Vector3 position = new Vector3(rightWallX, yPosition, 0f);
            GameObject rightWall = Instantiate(rightWallPrefab, position, Quaternion.identity);
            WallMover rightMover = rightWall.GetComponent<WallMover>() ?? rightWall.AddComponent<WallMover>();
            rightMover.Initialize(wallSpeed, destroyY);
        }
    }

    /// <summary>
    /// Очистка стен за экраном
    /// </summary>
    private void CleanupWalls()
    {
        WallMover[] walls = FindObjectsOfType<WallMover>();
        foreach (WallMover wall in walls)
        {
            if (wall != null && wall.transform.position.y < destroyY)
            {
                Destroy(wall.gameObject);
            }
        }
    }
}
