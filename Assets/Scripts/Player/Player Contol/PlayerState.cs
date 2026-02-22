using UnityEngine;

public class PlayerState : MonoBehaviour
{
    /// <summary>
    /// мне бы стоило узнать что это такое.
    /// </summary>
    private float velocityX;
    private bool isMoving;
    private float targetX;
    private bool inputBuffered;
    private float lastInputTime;

    public float VelocityX => velocityX;
    public bool IsMoving => isMoving;
    public float TargetX => targetX;
    public bool InputBuffered => inputBuffered;
    public float LastInputTime => lastInputTime;

    public void SetVelocityX(float value) => velocityX = value;
    public void SetIsMoving(bool value) => isMoving = value;
    public void SetTargetX(float value) => targetX = value;
    public void SetInputBuffered(bool value) => inputBuffered = value;
    public void SetLastInputTime(float value) => lastInputTime = value;
}
