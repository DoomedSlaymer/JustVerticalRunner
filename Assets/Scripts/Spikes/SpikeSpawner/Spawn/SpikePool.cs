using UnityEngine;
using System.Collections.Generic;

public class SpikePool : MonoBehaviour
{
    [System.Serializable]
    public class SpikePoolItem
    {
        public GameObject prefab;
        public int poolSize = 20;
        public bool autoExpand = false;
    }

    [SerializeField] private SpikePoolItem[] pools;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning("Нет пула для " + prefab.name);
            return null;
        }

        Queue<GameObject> pool = poolDictionary[prefab];
        GameObject objToSpawn;

        if (pool.Count > 0)
        {
            objToSpawn = pool.Dequeue();
        }
        else if (pools[GetPoolIndex(prefab)].autoExpand)
        {
            objToSpawn = Instantiate(prefab);
        }
        else
        {
            return null;
        }

        objToSpawn.SetActive(true);
        objToSpawn.transform.SetPositionAndRotation(pos, rot);
        objToSpawn.transform.SetParent(transform);

        return objToSpawn;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }

    int GetPoolIndex(GameObject prefab)
    {
        for (int i = 0; i < pools.Length; i++)
        {
            if (pools[i].prefab == prefab) return i;
        }
        return 0;
    }
}

