using UnityEngine;

public class SpikeMover : MonoBehaviour
{
    [Header("БАЗОВЫЕ ПАРАМЕТРЫ")]
    [SerializeField] private float baseSpeed = 5f;

    [Header("ДИНАМИЧЕСКИЕ")]
    private float currentSpeed;

    [Header("УДАЛЕНИЕ")]
    [SerializeField] private float destroyBelowY = -20f;

    void Start()
    {
        currentSpeed = baseSpeed;
    }

    // ✅ ПУБЛИЧНЫЙ метод для изменения скорости
    public void UpdateSpeed(float multiplier)
    {
        currentSpeed = baseSpeed * multiplier;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.y -= currentSpeed * Time.deltaTime; // ✅ Используем currentSpeed
        transform.position = pos;

        if (pos.y < destroyBelowY)
        {
            Destroy(gameObject);
        }
    }
}
