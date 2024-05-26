using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour
{
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void LoadSceneAdditive(string targetScene)
    {
        SceneManager.LoadScene(targetScene, LoadSceneMode.Additive);
        this.enabled = false;
    }

    public void LoadSceneSingle(string targetScene)
    {
        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }
}
