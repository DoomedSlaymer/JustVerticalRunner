using UnityEngine;

public class SpikeMover : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float destroyBelowY = -20f;

    private void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing)
            return;

        float speedMultiplier = DifficultyApplier.Instance != null
            ? DifficultyApplier.Instance.CurrentSpeedMultiplier
            : 1f;

        Vector3 pos = transform.position;
        pos.y -= baseSpeed * speedMultiplier * Time.deltaTime;
        transform.position = pos;

        if (pos.y < destroyBelowY)
            gameObject.SetActive(false);
    }
}
