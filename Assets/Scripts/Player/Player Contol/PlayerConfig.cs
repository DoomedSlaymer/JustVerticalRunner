using UnityEngine;
/// <summary>
/// конфиг характеристик игрока, считаю излишним
/// </summary>
public class PlayerConfig : MonoBehaviour
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

    [Header("Буфер ввода")]
    [SerializeField] private float inputBufferTime = 0.2f;
    public float InputBufferTime => inputBufferTime;
}
