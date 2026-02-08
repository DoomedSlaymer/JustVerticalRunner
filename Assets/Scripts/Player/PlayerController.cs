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

    [Header("📦 БУФЕР ВВОДА")]
    [SerializeField] private float inputBufferTime = 0.2f; // 200мс буфер
    private bool inputBuffered = false;
    private float lastInputTime;

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
        UpdateInputBuffer();
    }

    void InitializePosition()
    {
        targetX = leftWallX;
        SetPositionX(targetX);
    }

    void HandleInput()
    {
        // ✅ 1. ПРЯМОЙ ввод (если можно двигаться)
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            MoveToOppositeWall();
            return; // Игнорируем буфер
        }

        // ✅ 2. БУФЕРИЗАЦИЯ (если нажал во время движения)
        if (Input.GetKeyDown(KeyCode.Space) && isMoving)
        {
            BufferInput();
        }
    }

    void BufferInput()
    {
        inputBuffered = true;
        lastInputTime = Time.time;
    }

    void UpdateInputBuffer()
    {
        // ✅ 3. ПРОВЕРКА БУФЕРА (когда остановился)
        if (inputBuffered && !isMoving && Time.time - lastInputTime <= inputBufferTime)
        {
            MoveToOppositeWall();
            inputBuffered = false;
        }
        // ✅ Очистка устаревшего буфера
        else if (inputBuffered && Time.time - lastInputTime > inputBufferTime)
        {
            inputBuffered = false;
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

    // ✅ ДИАГНОСТИКА (для отладки)
    void OnGUI()
    {
        GUILayout.Label($"IsMoving: {isMoving}");
        GUILayout.Label($"InputBuffered: {inputBuffered}");
        GUILayout.Label($"BufferTimeLeft: {inputBufferTime - (Time.time - lastInputTime):F3}s");
    }
}
