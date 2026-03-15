using UnityEngine;

public class StartTutorialPresenter : MonoBehaviour
{
    [Header("Show In Waiting State")]
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private bool hideOnGameOver = false;

    private void Awake()
    {
        if (tutorialObject == null)
            tutorialObject = gameObject;
    }

    private void OnEnable()
    {
        RefreshVisibility();
    }

    private void Update()
    {
        RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        if (tutorialObject == null || GameManager.Instance == null)
            return;

        bool shouldShow = GameManager.Instance.CurrentState == GameState.WaitingToStart;
        if (hideOnGameOver && GameManager.Instance.CurrentState == GameState.GameOver)
            shouldShow = false;

        if (tutorialObject.activeSelf != shouldShow)
            tutorialObject.SetActive(shouldShow);
    }
}
