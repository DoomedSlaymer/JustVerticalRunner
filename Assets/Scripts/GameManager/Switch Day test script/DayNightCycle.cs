using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Header("Цвета дня/ночи")]
    public Color dayColor = new Color(0.58f, 0.66f, 0.76f);     // 94A8C1
    public Color nightColor = new Color(0.10f, 0.11f, 0.16f);   // 1A1B29

    [Header("Объект для fade (луна/звезды)")]
    public Renderer fadeObject;
    public Image fadeImage;

    [Header("Настройки")]
    public float switchInterval = 500f;
    public float fadeDuration = 2f;

    private float currentScore = 0f;
    private bool isDay = true;
    private Camera mainCamera;
    private Material objectMaterial;        // 🔥 КЭШ материала
    private Color objectStartColor;         // 🔥 Исходный цвет

    public static DayNightCycle Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            mainCamera = Camera.main;
            CacheObjectMaterial();
            InitializeDay();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔥 КЭШ материала для плавного alpha
    private void CacheObjectMaterial()
    {
        if (fadeObject != null)
        {
            objectMaterial = fadeObject.material;
            objectStartColor = objectMaterial.color;
            // Включаем прозрачность в материале
            objectMaterial.color = new Color(objectStartColor.r, objectStartColor.g, objectStartColor.b, 0f);
        }
    }

    public void UpdateScore(float newScore)
    {
        currentScore = newScore;
        int cycleIndex = Mathf.FloorToInt(currentScore / switchInterval);
        bool shouldBeDay = (cycleIndex % 2 == 0);

        if (shouldBeDay != isDay)
        {
            ToggleDayNight(shouldBeDay);
        }
    }

    private void ToggleDayNight(bool toDay)
    {
        isDay = toDay;
        StopAllCoroutines();
        StartCoroutine(SmoothTransition());
    }

    private System.Collections.IEnumerator SmoothTransition()
    {
        float elapsed = 0f;
        Color startBgColor = mainCamera.backgroundColor;
        Color targetBgColor = isDay ? dayColor : nightColor;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Фон камеры
            mainCamera.backgroundColor = Color.Lerp(startBgColor, targetBgColor, t);

            // 🔥 ПЛАВНЫЙ ALPHA объекта СИНХРОННО с фоном
            float targetAlpha = isDay ? 0f : 1f;
            SetObjectAlpha(Mathf.Lerp(1f - targetAlpha, targetAlpha, t));

            yield return null;
        }

        mainCamera.backgroundColor = targetBgColor;
        SetObjectAlpha(isDay ? 0f : 1f);
    }

    // 🔥 ИСПРАВЛЕННЫЙ плавный alpha
    private void SetObjectAlpha(float alpha)
    {
        if (fadeObject != null && objectMaterial != null)
        {
            Color color = objectStartColor;
            color.a = alpha;
            objectMaterial.color = color;
        }
        else if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }

    private void InitializeDay()
    {
        mainCamera.backgroundColor = dayColor;
        SetObjectAlpha(0f);
    }
}
