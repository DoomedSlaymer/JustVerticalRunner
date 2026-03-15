using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    private static SceneTransition runtimeInstance;
    private static bool isShuttingDown;

    [SerializeField] private float fadeDuration = 0.35f;

    private Canvas fadeCanvas;
    private Image fadeImage;
    private Coroutine transitionRoutine;
    private bool isRuntimeHost;

    private void Awake()
    {
        if (isRuntimeHost)
        {
            DontDestroyOnLoad(gameObject);
            EnsureOverlay();
        }
    }

    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    public void LoadSampleScene()
    {
        LoadSceneWithFade("SampleScene");
    }

    public static void LoadSceneWithFade(string sceneName)
    {
        EnsureInstance().BeginTransition(sceneName);
    }

    public static void RestartCurrentScene()
    {
        EnsureInstance().BeginTransition(SceneManager.GetActiveScene().name);
    }

    private static SceneTransition EnsureInstance()
    {
        if (runtimeInstance != null)
            return runtimeInstance;

        if (isShuttingDown)
            return null;

        GameObject go = new GameObject("SceneTransitionRuntime");
        runtimeInstance = go.AddComponent<SceneTransition>();
        runtimeInstance.isRuntimeHost = true;
        DontDestroyOnLoad(go);
        runtimeInstance.EnsureOverlay();
        return runtimeInstance;
    }

    private void BeginTransition(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        EnsureOverlay();

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        yield return Fade(0f, 1f);
        SceneManager.LoadScene(sceneName);
        yield return null;
        EnsureOverlay();
        yield return Fade(1f, 0f);
        transitionRoutine = null;
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        SetFadeAlpha(from);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float eased = Mathf.SmoothStep(0f, 1f, t);
            SetFadeAlpha(Mathf.Lerp(from, to, eased));
            yield return null;
        }

        SetFadeAlpha(to);
    }

    private void EnsureOverlay()
    {
        if (fadeCanvas == null)
        {
            GameObject canvasObject = new GameObject("FadeCanvas");
            canvasObject.transform.SetParent(transform, false);
            fadeCanvas = canvasObject.AddComponent<Canvas>();
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCanvas.sortingOrder = 9999;
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        if (fadeImage == null)
        {
            GameObject imageObject = new GameObject("FadeImage");
            imageObject.transform.SetParent(fadeCanvas.transform, false);
            fadeImage = imageObject.AddComponent<Image>();
            fadeImage.color = Color.black;
            RectTransform rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        SetFadeAlpha(0f);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage == null)
            return;

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
        fadeImage.raycastTarget = alpha > 0.001f;
    }
}
