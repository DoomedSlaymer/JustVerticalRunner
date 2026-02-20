using UnityEngine;

public class SpikePrefabSelector : MonoBehaviour
{
    private GameObject spikePrefab;
    private GameObject fastSpikePrefab;
    private GameObject doubleSpikePrefab;

    public void SetSpawnerData(GameObject normal, GameObject fast, GameObject doubleSpike)
    {
        spikePrefab = normal;
        fastSpikePrefab = fast;
        doubleSpikePrefab = doubleSpike;
    }

    public GameObject GetRandomPrefab()
    {
        int type = Random.Range(0, 3);
        return type switch
        {
            0 => spikePrefab,
            1 => fastSpikePrefab,
            _ => doubleSpikePrefab
        };
    }
}
