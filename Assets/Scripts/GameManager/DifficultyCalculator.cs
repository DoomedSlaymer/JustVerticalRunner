using UnityEngine;

public class DifficultyCalculator : MonoBehaviour
{
    /// <summary>
    /// 2 скрипт на расчёт сложности :D
    /// </summary>
    [SerializeField] public float difficultyCurvePower = 1.5f; // ✅ public

    public float CalculateSpeedMultiplier(float gameTime)
    {
        float normalizedTime = gameTime / 60f;
        return Mathf.Clamp(Mathf.Pow(1.3f, normalizedTime * difficultyCurvePower), 1f, 3f);
    }

    public float CalculateSpawnMultiplier(float gameTime)
    {
        float normalizedTime = gameTime / 60f;
        return Mathf.Clamp(Mathf.Pow(0.85f, normalizedTime * difficultyCurvePower), 0.3f, 1f);
    }
}
