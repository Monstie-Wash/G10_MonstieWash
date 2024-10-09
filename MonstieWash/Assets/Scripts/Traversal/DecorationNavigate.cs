using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationNavigate : MonoBehaviour, INavigator
{
    [HideInInspector] public bool InDecorationScene;

    public void OnClicked()
    {
        if (!InDecorationScene)
        {
            GameSceneManager.Instance.BeginDecoration();
            InDecorationScene = true;
        }
        else FindFirstObjectByType<DecorationManager>().TakePolaroid();
    }

}
