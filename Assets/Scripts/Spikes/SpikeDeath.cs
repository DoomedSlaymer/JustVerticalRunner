using UnityEngine;

public class SpikeDeath : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    private bool hasKilledPlayer = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasKilledPlayer)
        {
            Die(other.gameObject);
        }
    }

    private void Die(GameObject player)
    {
        Destroy(player);
        hasKilledPlayer = true;

        GameManager.Instance.ShowGameOverUI();
        GameManager.Instance.SetState(GameState.GameOver);
    }
}