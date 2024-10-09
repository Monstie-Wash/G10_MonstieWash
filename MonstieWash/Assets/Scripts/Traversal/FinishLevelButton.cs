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

        var saveLocation = Application.persistentDataPath + "/PolaroidSaving/PolaroidPositions.txt";
        var saveString = $"{polaroidPos.x.ToString("0.00")}, {polaroidPos.y.ToString("0.00")}, {polaroidRot.ToString("0.00")}";
        Debug.Log(saveString);

        if (File.Exists(saveLocation))
        {
            using (var outputFile = File.AppendText(saveLocation))
            {
                outputFile.WriteLine(saveString);
            }
        }
        else
        {
            using (var outputFile = File.CreateText(saveLocation))
            {
                outputFile.WriteLine(saveString);
            }
        }

        
    }
}
