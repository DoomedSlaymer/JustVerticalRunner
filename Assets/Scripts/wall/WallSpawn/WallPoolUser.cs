using UnityEngine;

public class WallPoolUser : MonoBehaviour
{
    private WallPool pool;

    void Start()
    {
        pool = FindObjectOfType<WallPool>();
    }

    void OnDisable()
    {
        if (pool != null)
            pool.ReturnToPool(gameObject);
    }
}
