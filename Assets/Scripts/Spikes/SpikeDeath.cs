using UnityEngine;

public class SpikeDeath : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    private bool hasKilledPlayer = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что коснулся игрок (тег "Player")
        if (other.CompareTag("Player") && !hasKilledPlayer)
        {
            Die(other.gameObject);
        }
    }

    void Die(GameObject player)
    {
        Destroy(player);
        hasKilledPlayer = true;

        // ✅ ТЕПЕРЬ через GameManager!
        GameManager.Instance.ShowGameOverUI();
        GameManager.Instance.SetState(GameState.Paused);
    }
}

