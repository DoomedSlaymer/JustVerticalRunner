using UnityEngine;

public class PlayerInputBuffer : MonoBehaviour
{
    [SerializeField] private PlayerState state;
    [SerializeField] private PlayerConfig config;
    [SerializeField] private PlayerMovement movement;

    private void Update()
    {
        UpdateBuffer();
    }

    private void UpdateBuffer()
    {
        if (GameManager.Instance?.CurrentState != GameState.Playing)
            return;

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