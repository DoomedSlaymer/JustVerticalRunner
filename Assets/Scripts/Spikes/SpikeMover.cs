using UnityEngine;

public class SpikeMover : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float returnBelowY = -20f;

    private SpikePool pool;

    private void OnEnable()
    {
        if (pool == null)
            pool = FindObjectOfType<SpikePool>();
    }

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

        if (pos.y < returnBelowY)
            ReturnToPoolOrDisable();
    }

    private void ReturnToPoolOrDisable()
    {
        if (pool != null)
        {
            pool.ReturnToPool(gameObject);
            return;
        }

        gameObject.SetActive(false);
    }
}
