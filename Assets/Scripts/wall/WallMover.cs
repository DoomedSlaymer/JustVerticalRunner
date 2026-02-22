using UnityEngine;

public class WallMover : MonoBehaviour
{
    /// <summary>
    ///  скрипт движения стен, удаляет при позиции ниже указанной
    /// </summary>
    private float speed;           // Скорость движения вниз
    private float destroyY;        // Граница удаления

    public void Initialize(float wallSpeed, float destroyYPosition)
    {
        speed = wallSpeed;
        destroyY = destroyYPosition;
    }

    public void MoveDown()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    private void Update()
    {
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
