using UnityEngine;
using UnityEngine.SceneManagement;

public class SpikeDeath : MonoBehaviour
{
    //[SerializeField] private float restartDelay = 1f; // Задержка перед рестартом

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что коснулся игрок (тег "Player")
        if (other.CompareTag("Player"))
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        // Перезапуск текущей сцены
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
