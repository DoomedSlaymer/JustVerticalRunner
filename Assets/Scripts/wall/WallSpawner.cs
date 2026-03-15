using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    [Header("Wall Prefabs")]
    [SerializeField] private GameObject leftWallPrefab;
    [SerializeField] private GameObject rightWallPrefab;

    [Header("Spawn Positions")]
    [SerializeField] private float leftWallX = -8f;
    [SerializeField] private float rightWallX = 8f;
    [SerializeField] private float spawnY = 12f;

    [Header("Spawn Layout")]
    [SerializeField] private int initialWallsCount = 3;
    [SerializeField] private float wallSpacingMultiplier = 1f;
    [SerializeField] private float extraWallSpacing = 0f;
    [SerializeField] private float wallOverlap = 0.15f;

    [Header("Movement")]
    [SerializeField] private float wallSpeed = 5f;

    [Header("Bounds")]
    [SerializeField] private float destroyY = -12f;

    private readonly List<WallMover> activeWalls = new List<WallMover>();
    private WallPool wallPool;
    private float wallPairSpacing;
    private bool hasLoggedPoolFallback;

    private void Start()
    {
        wallPool = FindObjectOfType<WallPool>();
        wallPairSpacing = CalculateWallPairSpacing();
        SpawnInitialWalls();
    }

    private void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing)
            return;

        MoveWallsDown();
        SpawnWallsIfNeeded();
    }

    private void SpawnInitialWalls()
    {
        for (int i = 0; i < initialWallsCount; i++)
        {
            SpawnWallPairAtY(spawnY - (i * wallPairSpacing));
        }
    }

    private void SpawnWallsIfNeeded()
    {
        while (ShouldSpawnNextPair())
        {
            SpawnWallPairAtY(spawnY);
        }
    }

    private bool ShouldSpawnNextPair()
    {
        float highestWallY = float.MinValue;

        for (int i = activeWalls.Count - 1; i >= 0; i--)
        {
            WallMover wall = activeWalls[i];
            if (wall == null || !wall.gameObject.activeInHierarchy)
            {
                activeWalls.RemoveAt(i);
                continue;
            }

            highestWallY = Mathf.Max(highestWallY, wall.transform.position.y);
        }

        if (highestWallY == float.MinValue)
            return true;

        return spawnY - highestWallY >= wallPairSpacing;
    }

    private void MoveWallsDown()
    {
        float currentWallSpeed = GetCurrentWallSpeed();

        for (int i = activeWalls.Count - 1; i >= 0; i--)
        {
            WallMover wall = activeWalls[i];
            if (wall == null)
            {
                activeWalls.RemoveAt(i);
                continue;
            }

            wall.SetSpeed(currentWallSpeed);
            wall.MoveDown();

            if (wall.IsBelowDestroyY())
            {
                activeWalls.RemoveAt(i);
                ReturnWall(wall.gameObject);
            }
        }
    }

    private void SpawnWallPairAtY(float yPosition)
    {
        CreateWall(leftWallPrefab, leftWallX, yPosition);
        CreateWall(rightWallPrefab, rightWallX, yPosition);
    }

    private void CreateWall(GameObject prefab, float xPosition, float yPosition)
    {
        if (prefab == null)
            return;

        Vector3 position = new Vector3(xPosition, yPosition, 0f);
        GameObject wallObject = null;

        if (wallPool != null)
            wallObject = wallPool.SpawnFromPool(prefab, position, Quaternion.identity);

        if (wallObject == null)
        {
            if (wallPool != null && !hasLoggedPoolFallback)
            {
                hasLoggedPoolFallback = true;
                Debug.LogWarning("WallSpawner: пул не выдал стену, используется Instantiate. Проверь pools и poolSize в WallPool.", this);
            }

            wallObject = Instantiate(prefab, position, Quaternion.identity);
        }

        WallMover mover = wallObject.GetComponent<WallMover>() ?? wallObject.AddComponent<WallMover>();
        mover.Initialize(GetCurrentWallSpeed(), destroyY);
        RegisterWall(mover);
    }

    private void RegisterWall(WallMover wall)
    {
        if (wall == null || activeWalls.Contains(wall))
            return;

        activeWalls.Add(wall);
    }

    private float GetCurrentWallSpeed()
    {
        float speedMultiplier = DifficultyApplier.Instance != null
            ? DifficultyApplier.Instance.CurrentSpeedMultiplier
            : 1f;

        return wallSpeed * speedMultiplier;
    }

    private float CalculateWallPairSpacing()
    {
        float wallHeight = GetPrefabHeight(leftWallPrefab);
        if (wallHeight <= 0f)
            wallHeight = GetPrefabHeight(rightWallPrefab);

        if (wallHeight <= 0f)
            wallHeight = 3f;

        float spacing = (wallHeight * wallSpacingMultiplier) + extraWallSpacing - wallOverlap;
        return Mathf.Max(spacing, 0.1f);
    }

    private float GetPrefabHeight(GameObject prefab)
    {
        if (prefab == null)
            return 0f;

        SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            return spriteRenderer.bounds.size.y;

        Collider2D collider2D = prefab.GetComponentInChildren<Collider2D>();
        if (collider2D != null)
            return collider2D.bounds.size.y;

        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        if (renderer != null)
            return renderer.bounds.size.y;

        return 0f;
    }

    private void ReturnWall(GameObject wallObject)
    {
        if (wallObject == null)
            return;

        if (wallPool != null)
        {
            wallPool.ReturnToPool(wallObject);
            return;
        }

        wallObject.SetActive(false);
    }
}
