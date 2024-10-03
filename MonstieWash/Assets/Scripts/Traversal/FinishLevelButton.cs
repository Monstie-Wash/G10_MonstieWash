using UnityEngine;
using System.IO;

public class FinishLevelButton : MonoBehaviour, INavigator
{
    public void OnClicked()
    {
        SaveCurrentPolaroidTransform();

        GameSceneManager.Instance.FinishLevel();
    }

    /// <summary>
    /// Saves the position and rotation of the most recently placed polaroid.
    /// </summary>
    private void SaveCurrentPolaroidTransform()
    {
        var polaroidTransform = FindFirstObjectByType<GalleryManager>().CurrentPolaroid;
        var polaroidPos = polaroidTransform.position;
        var polaroidRot = polaroidTransform.rotation.eulerAngles.z;

        var saveLocation = Path.Combine(Application.persistentDataPath, "PolaroidPositions.txt");

        using (var outputFile = new StreamWriter(saveLocation, true))
        {
            outputFile.WriteLine($"{polaroidPos.x.ToString("0.00")}, {polaroidPos.y.ToString("0.00")}, {polaroidRot.ToString("0.00")}");
        }
    }
}
