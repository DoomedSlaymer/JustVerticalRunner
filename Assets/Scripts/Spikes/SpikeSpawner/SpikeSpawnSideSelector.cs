using UnityEngine;

public class SpikeSpawnSideSelector : MonoBehaviour
{
    private float leftWallX = -4f;
    private float rightWallX = 4f;
    private float oppositeSideBoost = 0.7f;
    private bool lastSpawnedLeft = true;

    public void SetSpawnerData(float leftX, float rightX, float boost)
    {
        leftWallX = leftX;
        rightWallX = rightX;
        oppositeSideBoost = boost;
    }

    public (float x, float rotation) GetSpawnPosition()
    {
        bool spawnLeft = lastSpawnedLeft ?
            Random.value > oppositeSideBoost :
            Random.value <= oppositeSideBoost;

        lastSpawnedLeft = spawnLeft;

        return spawnLeft ? (leftWallX, -90f) : (rightWallX, 90f);
    }
}
