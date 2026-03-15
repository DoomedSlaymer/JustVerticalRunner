using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PlayerState state;
    [SerializeField] private PlayerConfig config;
    [SerializeField] private PlayerMovement movement;

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (!WasMovePressed())
            return;

        if (GameManager.Instance?.CurrentState == GameState.WaitingToStart)
            GameManager.Instance.StartGameplay();

        if (GameManager.Instance?.CurrentState != GameState.Playing)
            return;

        if (!state.IsMoving)
        {
            movement.MoveToOppositeWall();
            return;
        }

        BufferInput();
    }

    private bool WasMovePressed()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
    }

    private void BufferInput()
    {
        state.SetInputBuffered(true);
        state.SetLastInputTime(Time.time);
    }
}