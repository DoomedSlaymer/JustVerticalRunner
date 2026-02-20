using UnityEngine;

public class SpikePoolUser : MonoBehaviour
{
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float moveSpeed = 5f;

    private SpikePool pool;

    void Start()
    {
        pool = FindObjectOfType<SpikePool>();
        Destroy(gameObject, lifetime); // Fallback
    }

    void OnDisable()
    {
        if (pool != null)
            pool.ReturnToPool(gameObject);
    }

    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
    }
}
