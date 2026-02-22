using UnityEngine;

public class StarMovement : MonoBehaviour
{
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float destroyY = -10f;

    private float speed;
    private StarPool pool;

    void Start()
    {
        pool = FindObjectOfType<StarPool>();
        speed = Random.Range(minSpeed, maxSpeed);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < destroyY)
        {
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        if (pool != null)
            pool.ReturnStar(gameObject);
        else
            Destroy(gameObject);
    }
}
