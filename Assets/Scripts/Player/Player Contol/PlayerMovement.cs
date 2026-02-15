using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerState state;
    [SerializeField] private PlayerConfig config;

    void Update()
    {
        MoveToTarget();
    }

    void MoveToTarget()
    {
        float accel = GetAcceleration();
        state.SetVelocityX(GetNewVelocity(accel));
        ApplyVelocity();
        CheckArrival();
    }

    float GetAcceleration()
    {
        float time = state.IsMoving ? config.accelerationTime : config.decelerationTime;
        return config.moveSpeed / time;
    }

    float GetNewVelocity(float acceleration)
    {
        float targetVel = (state.TargetX - transform.position.x) * config.moveSpeed;
        return Mathf.MoveTowards(state.VelocityX, targetVel, acceleration * Time.deltaTime);
    }

    void ApplyVelocity()
    {
        transform.position += new Vector3(state.VelocityX * Time.deltaTime, 0, 0);
    }

    void CheckArrival()
    {
        if (Mathf.Abs(transform.position.x - state.TargetX) < config.stopDistance)
        {
            Arrive();
        }
    }

    void Arrive()
    {
        SetPositionX(state.TargetX);
        state.SetVelocityX(0);
        state.SetIsMoving(false);
    }

    void SetPositionX(float x)
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(x, pos.y, pos.z);
    }

    float GetOppositeX()
    {
        var config = GetComponent<PlayerConfig>();
        return transform.position.x > 0 ? config.leftWallX : config.rightWallX;
    }

    public void MoveToOppositeWall()
    {
        state.SetTargetX(GetOppositeX());
        state.SetIsMoving(true);
    }

    public bool IsMoving() => state.IsMoving;
    public float GetVelocity() => state.VelocityX;
    public float GetTargetX() => state.TargetX;
}
