using UnityEngine;
using System.Collections.Generic;

public class SpikeSpawner : MonoBehaviour
{
    [Header("ПРЕФАБ ШИПА")]
    [SerializeField] private GameObject spikePrefab;

    [Header("ПОЗИЦИИ СТЕН")]
    [SerializeField] private float leftWallX = -4f;
    [SerializeField] private float rightWallX = 4f;

    [Header("СПАВН")]
    [SerializeField] private float spawnY = 18f;
    [SerializeField] private float spikeSpeed = 5f;

    [Header("ОЧЕНЬ БЫСТРЫЙ СПАВН")]
    [SerializeField][Range(0.6f, 1.0f)] private float baseInterval = 0.8f;
    [SerializeField] private float spikeChance = 0.65f; // Больше шипов!

    [Header("⚖️ БАЛАНСИРОВКА")]
    [SerializeField] private int balanceThreshold = 3; // Разница > 3 = принудительный спавн

    [Header("БЕЗОПАСНОСТЬ")]
    [SerializeField] private float safeZoneTime = 1.0f;

    private Camera mainCamera;
    private float leftTimer, rightTimer;

    private List<float> leftSpikeTimes = new List<float>();
    private List<float> rightSpikeTimes = new List<float>();

    // Счётчики активных шипов
    private int leftActiveCount => leftSpikeTimes.Count;
    private int rightActiveCount => rightSpikeTimes.Count;

    void Start()
    {
        mainCamera = Camera.main;
        ResetTimers();
    }

    void Update()
    {
        CleanupOldTimes();

        leftTimer -= Time.deltaTime;
        rightTimer -= Time.deltaTime;

        if (leftTimer <= 0f)
        {
            TrySpawnLeft();
            leftTimer = Random.Range(baseInterval * 0.8f, baseInterval * 1.2f);
        }

        if (rightTimer <= 0f)
        {
            TrySpawnRight();
            rightTimer = Random.Range(baseInterval * 0.8f, baseInterval * 1.2f);
        }

        DeleteOldSpikes();
    }

    void ResetTimers()
    {
        leftTimer = Random.Range(baseInterval * 0.8f, baseInterval * 1.2f);
        rightTimer = Random.Range(baseInterval * 0.8f, baseInterval * 1.2f);
    }

    bool CanSpawnLeft() => !HasOppositeSpike(rightSpikeTimes);
    bool CanSpawnRight() => !HasOppositeSpike(leftSpikeTimes);

    // ⭐ БАЛАНСИРОВКА: принудительный спавн если пусто
    bool ShouldBalanceLeft() => rightActiveCount - leftActiveCount >= balanceThreshold && CanSpawnLeft();
    bool ShouldBalanceRight() => leftActiveCount - rightActiveCount >= balanceThreshold && CanSpawnRight();

    bool HasOppositeSpike(List<float> oppositeTimes)
    {
        float currentTime = Time.time;
        foreach (float oppTime in oppositeTimes)
        {
            if (currentTime - oppTime < safeZoneTime)
                return true;
        }
        return false;
    }

    void TrySpawnLeft()
    {
        // ⭐ ПРИНУДИТЕЛЬНЫЙ СПАВН ДЛЯ БАЛАНСА
        if (ShouldBalanceLeft())
        {
            SpawnSingleSpike(leftWallX, Quaternion.Euler(0, 0, -90f));
            leftSpikeTimes.Add(Time.time);
            return;
        }

        // Обычный спавн
        if (Random.value <= spikeChance && CanSpawnLeft())
        {
            SpawnSingleSpike(leftWallX, Quaternion.Euler(0, 0, -90f));
            leftSpikeTimes.Add(Time.time);
        }
    }

    void TrySpawnRight()
    {
        // ⭐ ПРИНУДИТЕЛЬНЫЙ СПАВН ДЛЯ БАЛАНСА
        if (ShouldBalanceRight())
        {
            SpawnSingleSpike(rightWallX, Quaternion.Euler(0, 0, 90f));
            rightSpikeTimes.Add(Time.time);
            return;
        }

        // Обычный спавн
        if (Random.value <= spikeChance && CanSpawnRight())
        {
            SpawnSingleSpike(rightWallX, Quaternion.Euler(0, 0, 90f));
            rightSpikeTimes.Add(Time.time);
        }
    }

    void SpawnSingleSpike(float xPos, Quaternion rotation)
    {
        Vector3 spawnPos = new Vector3(xPos, spawnY, 0);
        GameObject spike = Instantiate(spikePrefab, spawnPos, rotation, transform);

        Rigidbody2D rb = spike.GetComponent<Rigidbody2D>() ?? spike.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(0, -spikeSpeed);
    }

    void CleanupOldTimes()
    {
        float currentTime = Time.time;
        leftSpikeTimes.RemoveAll(t => currentTime - t > safeZoneTime * 1.5f);
        rightSpikeTimes.RemoveAll(t => currentTime - t > safeZoneTime * 1.5f);
    }

    void DeleteOldSpikes()
    {
        float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize * 2f;
        foreach (Transform spike in transform)
        {
            if (spike.position.y < cameraBottom)
                Destroy(spike.gameObject);
        }
    }
}
