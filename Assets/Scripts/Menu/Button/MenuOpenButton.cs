using UnityEngine;

public class MenuOpenButton : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainMenu";

    public void OpenMenu()
    {
        SceneTransition.LoadSceneWithFade(menuSceneName);
    }
}

