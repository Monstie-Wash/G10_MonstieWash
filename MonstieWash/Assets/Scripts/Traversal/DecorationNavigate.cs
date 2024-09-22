using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationNavigate : MonoBehaviour, INavigator
{
    public void OnClicked()
    {
        GameSceneManager.Instance.BeginDecoration();
    }

}
