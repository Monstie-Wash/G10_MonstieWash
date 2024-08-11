using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //Tags to use if sorting items is needed later in minigames/crafting;
    public enum ItemTags
    {
        Common,
        Rare,
        Mythical,
        Potion,
        Treat
    }



    private void Awake()
    {
        MakeClassStatic();
    }



    /// <summary>
    /// Assigns this object to not be destroyed on load. If a new script is found will then remove it;
    /// </summary>
    private void MakeClassStatic()
    {
        Inventory[] objs = FindObjectsByType<Inventory>(FindObjectsSortMode.None);

        //When class already exists don't create a new script;
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
