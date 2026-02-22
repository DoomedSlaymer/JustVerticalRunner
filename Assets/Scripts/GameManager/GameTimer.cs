using UnityEngine;

public class GameTimer : MonoBehaviour
{
    // таймер используемый для математики
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
