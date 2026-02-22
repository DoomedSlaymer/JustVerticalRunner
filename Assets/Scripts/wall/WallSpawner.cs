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
    private WallPool wallPool; // ✅ НОВОЕ!

    void Start()
    {
        spawnTimer = spawnInterval;
        wallPool = FindObjectOfType<WallPool>(); // ✅ НАЙТИ ПУЛ
        SpawnInitialWalls();
    }

    private void Update()
    {
        SpawnWalls();
        MoveWallsDown();
        CleanupWalls();
    }

    private void MoveWallsDown()
    {
        WallMover[] walls = FindObjectsOfType<WallMover>();
        foreach (WallMover wall in walls)
        {
            if (wall != null)
                wall.MoveDown();
        }
    }

    private void SpawnInitialWalls()
    {
        for (int i = 0; i < initialWallsCount; i++)
        {
            float offsetY = i * spawnInterval;
            SpawnWallPairAtY(spawnY - offsetY);
        }
    }

    private void SpawnWallPairAtY(float yPosition)
    {
        CreateLeftWallAtY(yPosition);
        CreateRightWallAtY(yPosition);
    }

    private void SpawnWalls()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnWallPairAtY(spawnY);
            spawnTimer = spawnInterval;
        }
    }

    private void CreateLeftWallAtY(float yPosition)
    {
        if (leftWallPrefab != null)
        {
            Vector3 position = new Vector3(leftWallX, yPosition, 0f);

            // ✅ Pool вместо Instantiate!
            GameObject leftWall;
            if (wallPool != null)
            {
                leftWall = wallPool.SpawnFromPool(leftWallPrefab, position, Quaternion.identity);
                if (leftWall != null)
                {
                    WallMover leftMover = leftWall.GetComponent<WallMover>();
                    if (leftMover != null) leftMover.Initialize(wallSpeed, destroyY);
                }
            }
            else
            {
                // Fallback
                leftWall = Instantiate(leftWallPrefab, position, Quaternion.identity);
                WallMover leftMover = leftWall.GetComponent<WallMover>() ?? leftWall.AddComponent<WallMover>();
                leftMover.Initialize(wallSpeed, destroyY);
            }
        }
    }

    private void CreateRightWallAtY(float yPosition)
    {
        if (rightWallPrefab != null)
        {
            Vector3 position = new Vector3(rightWallX, yPosition, 0f);

            // ✅ Pool вместо Instantiate!
            GameObject rightWall;
            if (wallPool != null)
            {
                rightWall = wallPool.SpawnFromPool(rightWallPrefab, position, Quaternion.identity);
                if (rightWall != null)
                {
                    WallMover rightMover = rightWall.GetComponent<WallMover>();
                    if (rightMover != null) rightMover.Initialize(wallSpeed, destroyY);
                }
            }
            else
            {
                // Fallback
                rightWall = Instantiate(rightWallPrefab, position, Quaternion.identity);
                WallMover rightMover = rightWall.GetComponent<WallMover>() ?? rightWall.AddComponent<WallMover>();
                rightMover.Initialize(wallSpeed, destroyY);
            }
        }
    }

    private void CleanupWalls()
    {
        // ✅ Pool НЕ использует Destroy - убрано!
        // Стены сами возвращаются через WallPoolUser
    }
}
