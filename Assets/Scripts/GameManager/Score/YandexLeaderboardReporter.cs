using System;
using System.Reflection;
using UnityEngine;
#if PLUGIN_YG_2
using YG;
#endif

public class YandexLeaderboardReporter : MonoBehaviour
{
    [Header("Yandex Leaderboard")]
    [SerializeField] private string leaderboardName = "BestScore";
    [SerializeField] private bool sendOnlyIfHigherThanLast = true;
    [SerializeField] private string lastSentScoreKey = "yg_lb_last_sent_score";

    public void ReportCurrentScoreFromGameManager()
    {
        if (GameManager.Instance == null)
            return;

        ReportScore(Mathf.FloorToInt(GameManager.Instance.GetCurrentScore()));
    }

    public void ReportHighScoreFromManager()
    {
        HighScoreManager manager = FindObjectOfType<HighScoreManager>();
        if (manager == null)
            return;

        ReportScore(manager.GetHighScore());
    }

    public void ReportScore(int score)
    {
        int normalizedScore = Mathf.Max(0, score);

        if (sendOnlyIfHigherThanLast)
        {
            int lastSent = PlayerPrefs.GetInt(lastSentScoreKey, 0);
            if (normalizedScore <= lastSent)
                return;
        }

        if (!TrySendToYandexLeaderboard(leaderboardName, normalizedScore))
            return;

        PlayerPrefs.SetInt(lastSentScoreKey, normalizedScore);
        PlayerPrefs.Save();
    }

    private bool TrySendToYandexLeaderboard(string lbName, int score)
    {
#if !PLUGIN_YG_2
        Debug.LogWarning("YG2 plugin is not enabled. Leaderboard score was not sent.");
        return false;
#else
        if (string.IsNullOrWhiteSpace(lbName))
        {
            Debug.LogWarning("Leaderboard name is empty.");
            return false;
        }

        if (!YG2.isSDKEnabled)
        {
            Debug.Log("YG2 SDK is not ready yet. Leaderboard score was not sent.");
            return false;
        }

        Type yg2Type = typeof(YG2);
        BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

        if (InvokeLeaderboardMethod(yg2Type, flags, "NewLeaderboardScores", lbName, score))
            return true;
        if (InvokeLeaderboardMethod(yg2Type, flags, "SetLeaderboard", lbName, score))
            return true;
        if (InvokeLeaderboardMethod(yg2Type, flags, "SetLeaderboardScores", lbName, score))
            return true;
        if (InvokeLeaderboardMethod(yg2Type, flags, "LeaderboardSend", lbName, score))
            return true;

        Debug.LogWarning("Leaderboard API method not found. Install/enable Leaderboards module in PluginYG2.");
        return false;
#endif
    }

#if PLUGIN_YG_2
    private static bool InvokeLeaderboardMethod(Type type, BindingFlags flags, string methodName, string leaderboard, int score)
    {
        MethodInfo method = type.GetMethod(methodName, flags, null, new[] { typeof(string), typeof(int) }, null);
        if (method == null)
            return false;

        method.Invoke(null, new object[] { leaderboard, score });
        return true;
    }
#endif
}

