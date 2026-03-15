using System.Collections.Generic;
using UnityEngine;

public class WallPool : MonoBehaviour
{
    [System.Serializable]
    public class WallPoolItem
    {
        public GameObject prefab;
        public int poolSize = 15;
        public bool autoExpand = false;
    }

    [SerializeField] private WallPoolItem[] pools;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;
    private Dictionary<GameObject, GameObject> prefabByInstance;

    private void Awake()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        if (poolDictionary != null)
            return;

        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        prefabByInstance = new Dictionary<GameObject, GameObject>();

        foreach (WallPoolItem pool in pools)
        {
            if (pool.prefab == null)
                continue;

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                prefabByInstance[obj] = pool.prefab;
            }

            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (poolDictionary == null)
            InitializePools();

        if (prefab == null)
            return null;

        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning("WallPool: no pool configured for " + prefab.name, this);
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
            Debug.LogWarning("WallPool: pool expanded for " + prefab.name, this);
            objToSpawn = Instantiate(prefab);
            prefabByInstance[objToSpawn] = prefab;
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
        if (obj == null || !prefabByInstance.TryGetValue(obj, out GameObject prefabKey))
            return;

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolDictionary[prefabKey].Enqueue(obj);
    }

    private int GetPoolIndex(GameObject prefab)
    {
        for (int i = 0; i < pools.Length; i++)
        {
            if (pools[i].prefab == prefab)
                return i;
        }

        return 0;
    }
}
