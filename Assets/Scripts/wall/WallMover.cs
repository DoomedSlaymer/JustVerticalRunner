using UnityEngine;

public class WallMover : MonoBehaviour
{
    private float speed;
    private float destroyY;

    public void Initialize(float wallSpeed, float destroyYPosition)
    {
        speed = wallSpeed;
        destroyY = destroyYPosition;
    }

    public void SetSpeed(float wallSpeed)
    {
        speed = wallSpeed;
    }

    public void MoveDown()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    public bool IsBelowDestroyY()
    {
        return transform.position.y < destroyY;
    }
}