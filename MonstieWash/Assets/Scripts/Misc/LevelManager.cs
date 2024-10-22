using UnityEngine;
using System.Threading.Tasks;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ToolSwitcher toolSwitcher;
    [SerializeField] private TaskTracker taskTracker;

    public async Task InitializeLevel()
    {
        await CombineDirt();
        PopulateToolErasables();
        taskTracker.Initialize();
    }

    private async Task CombineDirt()
    {
        foreach (var erasable in GameObject.FindGameObjectsWithTag("Erasable")) erasable.GetComponent<SpriteRenderer>().enabled = false;

        var dirtCombiners = FindObjectsByType<DirtCombiner>(FindObjectsSortMode.None);

        foreach (var dirtCombiner in dirtCombiners)
        {
            await dirtCombiner.Combine();
        }

        foreach (var erasable in GameObject.FindGameObjectsWithTag("Erasable")) erasable.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void PopulateToolErasables()
    {
        foreach (var tool in toolSwitcher.ToolInstances)
        {
            tool.GetComponent<Eraser>().PopulateErasables();
        }
    }
}
