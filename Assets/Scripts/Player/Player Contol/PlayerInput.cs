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
        bool keyboardOrMousePressed = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
        bool touchPressed = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        return keyboardOrMousePressed || touchPressed;
    }

    private void BufferInput()
    {
        state.SetInputBuffered(true);
        state.SetLastInputTime(Time.time);
    }
}
