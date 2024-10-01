using UnityEngine;

public class FinishLevelButton : MonoBehaviour, INavigator
{
    public void OnClicked()
    {
        var dm = FindFirstObjectByType<DecorationManager>();
        if (dm != null) dm.TakePolaroid();
        else GameSceneManager.Instance.FinishLevel();

    }
}
