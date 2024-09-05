using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    private TraversalMenu backButton;
	// Start is called before the first frame update
	private void Start()
	{
	}

	void Awake()
    {
		FindBack();
    }

	void OnEnable()
	{
		InputManager.Instance.OnCancel += Inputs_Back;
	}

	private void OnDisable()
	{
		InputManager.Instance.OnCancel -= Inputs_Back;
	}

	private void Inputs_Back()
	{
		backButton.OnClicked();
	}

	public void FindBack()
	{
		var buttons = FindObjectsByType<TraversalMenu>(FindObjectsSortMode.None);

		foreach (var button in buttons) 
		{
			if (button.gameObject.name == "Back" && button.gameObject.activeInHierarchy)
			{
				backButton = button;
				return;
			}
		}
	}

}
