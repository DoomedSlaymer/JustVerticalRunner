using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    /// <summary>
    /// отвечает за ввод состояение буффера
    /// </summary>
    [SerializeField] private PlayerState state;
    [SerializeField] private PlayerConfig config;
    [SerializeField] private PlayerMovement movement;

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !state.IsMoving)
        {
            movement.MoveToOppositeWall();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && state.IsMoving)
        {
            BufferInput();
        }
    }

    void BufferInput()
    {
        state.SetInputBuffered(true);
        state.SetLastInputTime(Time.time);
    }
}
