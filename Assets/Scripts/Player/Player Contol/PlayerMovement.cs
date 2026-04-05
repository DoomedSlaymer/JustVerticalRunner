using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerState state;
    [SerializeField] private PlayerConfig config;

    private const float MaxSimulationDelta = 0.05f;

    private void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        float dt = Mathf.Min(Time.deltaTime, MaxSimulationDelta);
        float accel = GetAcceleration();

        state.SetVelocityX(GetNewVelocity(accel, dt));
        ApplyVelocity(dt);
        CheckArrival();
    }

    private float GetAcceleration()
    {
        float time = state.IsMoving ? config.accelerationTime : config.decelerationTime;
        time = Mathf.Max(0.01f, time);
        return GetCurrentMoveSpeed() / time;
    }

    private float GetNewVelocity(float acceleration, float dt)
    {
        float targetVel = (state.TargetX - transform.position.x) * GetCurrentMoveSpeed();
        return Mathf.MoveTowards(state.VelocityX, targetVel, acceleration * dt);
    }

    private float GetCurrentMoveSpeed()
    {
        float difficultyMultiplier = DifficultyApplier.Instance != null
            ? DifficultyApplier.Instance.CurrentSpeedMultiplier
            : 1f;
        float appliedMultiplier = Mathf.Lerp(1f, difficultyMultiplier, config.difficultySpeedInfluence);

        return config.moveSpeed * appliedMultiplier;
    }

    private void ApplyVelocity(float dt)
    {
        Vector3 pos = transform.position;
        float previousX = pos.x;
        float nextX = previousX + state.VelocityX * dt;

        float minX = Mathf.Min(config.leftWallX, config.rightWallX);
        float maxX = Mathf.Max(config.leftWallX, config.rightWallX);
        nextX = Mathf.Clamp(nextX, minX, maxX);

        if (HasCrossedTarget(previousX, nextX))
            nextX = state.TargetX;

        transform.position = new Vector3(nextX, pos.y, pos.z);
    }

    private bool HasCrossedTarget(float previousX, float nextX)
    {
        if (!state.IsMoving)
            return false;

        float target = state.TargetX;
        return (previousX - target) * (nextX - target) <= 0f;
    }

    private void CheckArrival()
    {
        if (Mathf.Abs(transform.position.x - state.TargetX) < config.stopDistance)
            Arrive();
    }

    private void Arrive()
    {
        SetPositionX(state.TargetX);
        state.SetVelocityX(0f);
        state.SetIsMoving(false);
    }

    private void SetPositionX(float x)
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(x, pos.y, pos.z);
    }

    private float GetOppositeX()
    {
        return transform.position.x > 0f ? config.leftWallX : config.rightWallX;
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
