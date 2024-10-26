using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private List<Tool> tools = new();

    private List<Tool> m_originalTools = new();

    [HideInInspector] public List<Tool> Tools { get; private set; }

    private void Awake()
    {
        Tools = new List<Tool>(tools);
        
        foreach (var tool in tools)
        {
            var newTool = ScriptableObject.CreateInstance<Tool>();
            newTool.SetValues(tool);
            m_originalTools.Add(newTool);
        }    
    }

    private void OnDestroy()
    {
        for (int i = 0; i < tools.Count; i++)
        {
            tools[i].SetValues(m_originalTools[i]);
        }
    }
}
