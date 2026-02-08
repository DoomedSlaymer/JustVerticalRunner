using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Позиции")]
    public float leftWallX = -4f;
    public float rightWallX = 4f;

    [Header("Движение")]
    public float moveSpeed = 8f;
    public float accelerationTime = 0.3f;
    public float decelerationTime = 0.4f;
    [Range(0.01f, 0.5f)]
    public float stopDistance = 0.1f;

    private float velocityX;
    private bool isMoving;
    private float targetX;

    void Start()
    {
        InitializePosition();
    }

    void Update()
    {
        HandleInput();
        MoveToTarget();
    }

    void InitializePosition()
    {
        targetX = leftWallX;
        SetPositionX(targetX);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            MoveToOppositeWall();
        }
    }

    void MoveToTarget()
    {
        float accel = GetAcceleration();
        velocityX = GetNewVelocity(accel);
        ApplyVelocity();
        CheckArrival();
    }

    float GetAcceleration()
    {
        float time = isMoving ? accelerationTime : decelerationTime;
        return moveSpeed / time;
    }

    float GetNewVelocity(float acceleration)
    {
        float targetVel = (targetX - transform.position.x) * moveSpeed;
        return Mathf.MoveTowards(velocityX, targetVel, acceleration * Time.deltaTime);
    }

    void ApplyVelocity()
    {
        transform.position += new Vector3(velocityX * Time.deltaTime, 0, 0);
    }

    void CheckArrival()
    {
        if (Mathf.Abs(transform.position.x - targetX) < stopDistance)
        {
            Arrive();
        }
    }

    void MoveToOppositeWallPrivate()
    {
        targetX = GetOppositeX();
        isMoving = true;
    }

    float GetOppositeX()
    {
        return transform.position.x > 0 ? leftWallX : rightWallX;
    }

    void SetPositionX(float x)
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(x, pos.y, pos.z);
    }

    void Arrive()
    {
        SetPositionX(targetX);
        velocityX = 0;
        isMoving = false;
    }

    // Публичный API
    public void MoveToOppositeWall()
    {
        MoveToOppositeWallPrivate();
    }

    public bool IsMoving() => isMoving;
    public float GetVelocity() => velocityX;
    public float GetTargetX() => targetX;
}
