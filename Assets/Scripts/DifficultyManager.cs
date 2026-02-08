using UnityEngine;
using UnityEngine.UI; // ✅ UI
using TMPro;

public class DifficultyManager : MonoBehaviour
{
    [Header("ВРЕМЯ ИГРЫ (секунды)")]
    [SerializeField] private float gameTime;

    [Header("💰 ОЧКИ")]
    [SerializeField] private TMP_Text scoreText;           // Text UI компонент
    [SerializeField] private int basePointsPerSecond = 10;
    private float currentScore = 0f;

    [Header("ЭКСПОНЕНЦИАЛЬНЫЕ КРИВЫЕ")]
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float spawnRateMultiplier = 1f;
    [SerializeField] private float difficultyCurvePower = 1.5f;

    public static DifficultyManager Instance { get; private set; }

    private SpikeSpawner spawner;
    private SpikeMover[] spikeMovers;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        spawner = FindObjectOfType<SpikeSpawner>();
        InvokeRepeating(nameof(UpdateDifficulty), 1f, 0.5f);
    }

    void Update()
    {
        gameTime += Time.deltaTime;

        // ✅ ОЧКИ = время * скорость (экспоненциально растут!)
        float pointsThisFrame = basePointsPerSecond * speedMultiplier * Time.deltaTime;
        currentScore += pointsThisFrame;

        // ✅ ВЫВОД НА ЭКРАН
        if (scoreText != null)
        {
            // В Update() заменить строку вывода:
            scoreText.text = $"SCORE\n{Mathf.FloorToInt(currentScore):N0}";

        }
    }

    void UpdateDifficulty()
    {
        // ✅ ЭКСПОНЕНЦИАЛЬНАЯ формула
        float normalizedTime = gameTime / 60f;
        speedMultiplier = Mathf.Pow(1.3f, normalizedTime * difficultyCurvePower);
        spawnRateMultiplier = Mathf.Pow(0.85f, normalizedTime * difficultyCurvePower);

        speedMultiplier = Mathf.Clamp(speedMultiplier, 1f, 3f);
        spawnRateMultiplier = Mathf.Clamp(spawnRateMultiplier, 0.3f, 1f);

        ApplyDifficultyToSpawner();
        ApplyDifficultyToMovers();

        Debug.Log($"⏱️ {gameTime:F1}s | 💰 {currentScore:F0} | ⚡ {speedMultiplier:F2}x | ⏱️ {spawnRateMultiplier:F2}x");
    }

    // ✅ ПУБЛИЧНЫЕ ГЕТТЕРЫ
    public float GetCurrentSpeedMultiplier() => speedMultiplier;
    public float GetCurrentSpawnMultiplier() => spawnRateMultiplier;
    public float GetCurrentScore() => currentScore;

    void ApplyDifficultyToSpawner()
    {
        if (spawner != null)
            spawner.UpdateSpawnRates(spawnRateMultiplier);
    }

    void ApplyDifficultyToMovers()
    {
        spikeMovers = FindObjectsOfType<SpikeMover>();
        foreach (var mover in spikeMovers)
            mover.UpdateSpeed(speedMultiplier);
    }
}
