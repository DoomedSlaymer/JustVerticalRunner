using UnityEngine;

public class PlayerInputBuffer : MonoBehaviour
{
    /// <summary>
    /// буффер обновляется каждый кадр
    /// </summary>
    [SerializeField] private PlayerState state;
    [SerializeField] private PlayerConfig config;
    [SerializeField] private PlayerMovement movement;

    void Update()
    {
        UpdateBuffer();
    }

    void UpdateBuffer()
    {
        if (state.InputBuffered && !state.IsMoving &&
            Time.time - state.LastInputTime <= config.InputBufferTime)
        {
            movement.MoveToOppositeWall();
            state.SetInputBuffered(false);
        }
        else if (state.InputBuffered && Time.time - state.LastInputTime > config.InputBufferTime)
        {
            state.SetInputBuffered(false);
        }
    }
}
