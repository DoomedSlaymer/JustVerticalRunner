using UnityEngine;

public class PlayerCoordinator : MonoBehaviour
{
    [SerializeField] private PlayerConfig config;
    [SerializeField] private PlayerState state;
    [SerializeField] private PlayerMovement movement;

    void Start()
    {
        InitializePosition();
        CacheComponents();
    }

    void CacheComponents()
    {
        config = config ? config : GetComponent<PlayerConfig>();
        state = state ? state : GetComponent<PlayerState>();
        movement = movement ? movement : GetComponent<PlayerMovement>();
    }

    void InitializePosition()
    {
        var config = GetComponent<PlayerConfig>();
        state.SetTargetX(config.leftWallX);
        SetPositionX(config.leftWallX);
    }

    void SetPositionX(float x)
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(x, pos.y, pos.z);
    }
}
