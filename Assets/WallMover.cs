using UnityEngine;

public class WallMover : MonoBehaviour
{
    private float speed;           // Скорость движения вниз
    private float destroyY;        // Граница удаления

    /// <summary>
    /// Инициализация движения стены
    /// </summary>
    /// <param name="wallSpeed">Скорость движения</param>
    /// <param name="destroyYPosition">Позиция удаления</param>
    public void Initialize(float wallSpeed, float destroyYPosition)
    {
        speed = wallSpeed;
        destroyY = destroyYPosition;
    }

    /// <summary>
    /// Движение стены вниз каждый кадр
    /// </summary>
    public void MoveDown()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    /// <summary>
    /// Автоматическое удаление при выходе за экран (резервный вариант)
    /// </summary>
    private void Update()
    {
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
