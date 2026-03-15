using UnityEngine;

public class SpikePoolUser : MonoBehaviour
{
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float moveSpeed = 5f;

    private SpikePool pool;
    private SpikeMover spikeMover;
    private float lifetimeTimer;

    private void Start()
    {
        pool = FindObjectOfType<SpikePool>();
        spikeMover = GetComponent<SpikeMover>();
    }

    private void OnEnable()
    {
        lifetimeTimer = lifetime;
    }

    private void OnDisable()
    {
        if (pool != null)
            pool.ReturnToPool(gameObject);
    }

    private void Update()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing)
            return;

        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        if (spikeMover != null)
            return;

        float speedMultiplier = DifficultyApplier.Instance != null
            ? DifficultyApplier.Instance.CurrentSpeedMultiplier
            : 1f;

        transform.Translate(Vector3.down * moveSpeed * speedMultiplier * Time.deltaTime);
    }
}
