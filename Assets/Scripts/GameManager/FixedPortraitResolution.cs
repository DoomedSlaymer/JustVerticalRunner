using UnityEngine;

public class FixedPortraitResolution : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        GameObject obj = new GameObject("FixedPortraitResolution");
        DontDestroyOnLoad(obj);
        obj.AddComponent<FixedPortraitResolution>();
    }

    private void Awake()
    {
        ApplyPlatformResolution();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            ApplyPlatformResolution();
    }

    private static void ApplyPlatformResolution()
    {
        Screen.orientation = ScreenOrientation.Portrait;

#if !UNITY_WEBGL
        Screen.SetResolution(1080, 1920, FullScreenMode.FullScreenWindow, 60);
#endif
    }
}
