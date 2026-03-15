using UnityEngine;

public class ScoreUpdater : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private int basePointsPerSecond = 10;

    private void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing || scoreManager == null)
            return;

        float pointsThisFrame = basePointsPerSecond * Time.deltaTime;
        scoreManager.AddPoints(pointsThisFrame);
    }
}