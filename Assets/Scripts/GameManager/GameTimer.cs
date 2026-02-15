using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float gameTime { get; private set; }

    public void ResetTime()
    {
        gameTime = 0f;
    }
    void Update()
    {
        if (GameManager.Instance?.CurrentState == GameState.Playing)
            gameTime += Time.deltaTime;
    }
}
