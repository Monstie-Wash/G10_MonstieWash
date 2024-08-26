using UnityEngine;

public class FinishLevelButton : MonoBehaviour, INavigator
{
    public void OnClicked()
    {
        GameSceneManager.Instance.FinishLevel();
    }
}
