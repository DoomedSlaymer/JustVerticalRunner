using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerState state;
    [SerializeField] private PlayerConfig config;

    private void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        float accel = GetAcceleration();
        state.SetVelocityX(GetNewVelocity(accel));
        ApplyVelocity();
        CheckArrival();
    }

    private float GetAcceleration()
    {
        float time = state.IsMoving ? config.accelerationTime : config.decelerationTime;
        return GetCurrentMoveSpeed() / time;
    }

    private float GetNewVelocity(float acceleration)
    {
        float targetVel = (state.TargetX - transform.position.x) * GetCurrentMoveSpeed();
        return Mathf.MoveTowards(state.VelocityX, targetVel, acceleration * Time.deltaTime);
    }

    private float GetCurrentMoveSpeed()
    {
        float difficultyMultiplier = DifficultyApplier.Instance != null
            ? DifficultyApplier.Instance.CurrentSpeedMultiplier
            : 1f;
        float appliedMultiplier = Mathf.Lerp(1f, difficultyMultiplier, config.difficultySpeedInfluence);

        return config.moveSpeed * appliedMultiplier;
    }

    private void ApplyVelocity()
    {
        transform.position += new Vector3(state.VelocityX * Time.deltaTime, 0f, 0f);
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