using UnityEngine;

public class FinishLevelButton : MonoBehaviour, INavigator
{
    public void OnClicked()
    {
        DecorationManager dm = FindFirstObjectByType<DecorationManager>();
        if (dm != null) dm.TakePolaroid();
        else GameSceneManager.Instance.FinishLevel();

    }
}
