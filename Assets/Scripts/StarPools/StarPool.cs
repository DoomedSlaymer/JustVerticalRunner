using UnityEngine;
using System.Collections.Generic;

public class StarPool : MonoBehaviour
{
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private int poolSize = 50;
    [SerializeField] private bool autoExpand = false;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject star = Instantiate(starPrefab);
            star.SetActive(false);
            pool.Enqueue(star);
        }
    }

    public GameObject SpawnStar(Vector3 position, float speed)
    {
        GameObject star;

        if (pool.Count > 0)
        {
            star = pool.Dequeue();
        }
        else if (autoExpand)
        {
            star = Instantiate(starPrefab);
        }
        else return null;

        star.SetActive(true);
        star.transform.position = position;

        // ✅ Передаём скорость через компонент
        StarMovement starMovement = star.GetComponent<StarMovement>();
        if (starMovement != null)
            starMovement.SetSpeed(speed);

        star.transform.SetParent(transform);
        return star;
    }

    public void ReturnStar(GameObject star)
    {
        star.SetActive(false);
        star.transform.SetParent(transform);
        pool.Enqueue(star);
    }
}
