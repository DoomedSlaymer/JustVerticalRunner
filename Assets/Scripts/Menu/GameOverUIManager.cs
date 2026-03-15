using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
    public static GameOverUIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Show Animation")]
    [SerializeField] private float showDuration = 0.3f;
    [SerializeField] private float startScale = 0.9f;

    private CanvasGroup canvasGroup;
    private RectTransform panelRect;
    private Coroutine showRoutine;
    private bool initialized;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        EnsureInitialized();
    }

    private void Start()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (initialized)
            return;

        if (gameOverPanel != null)
        {
            canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameOverPanel.AddComponent<CanvasGroup>();

            panelRect = gameOverPanel.GetComponent<RectTransform>();
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartClick);
            restartButton.onClick.AddListener(OnRestartClick);
        }

        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(OnMenuClick);
            menuButton.onClick.AddListener(OnMenuClick);
        }

        initialized = true;
    }

    public void ShowGameOver()
    {
        EnsureInitialized();

        if (gameOverPanel == null)
            return;

        UpdateScoreUI();
        gameOverPanel.SetActive(true);

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(AnimateShow());
    }

    public void HideGameOver()
    {
        EnsureInitialized();

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        HideGameOverImmediate();
    }

    private void HideGameOverImmediate()
    {
        if (gameOverPanel == null)
            return;

        gameOverPanel.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (panelRect != null)
            panelRect.localScale = Vector3.one * startScale;
    }

    private IEnumerator AnimateShow()
    {
        EnsureInitialized();

        if (canvasGroup == null || panelRect == null)
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
            yield break;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        panelRect.localScale = Vector3.one * startScale;

        float elapsed = 0f;
        while (elapsed < showDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / showDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            canvasGroup.alpha = eased;
            panelRect.localScale = Vector3.LerpUnclamped(Vector3.one * startScale, Vector3.one, eased);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        panelRect.localScale = Vector3.one;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        showRoutine = null;
    }

    private void UpdateScoreUI()
    {
        if (currentScoreText != null && GameManager.Instance != null)
            currentScoreText.text = $"SCORE\n{Mathf.FloorToInt(GameManager.Instance.GetCurrentScore()):N0}";

        if (highScoreText != null)
        {
            HighScoreManager hsManager = FindObjectOfType<HighScoreManager>();
            highScoreText.text = $"BEST\n{hsManager?.GetHighScore():N0}";
        }
    }

    private void OnRestartClick()
    {
        SceneTransition.RestartCurrentScene();
    }

    private void OnMenuClick()
    {
        SceneTransition.LoadSceneWithFade("MainMenu");
    }
}
