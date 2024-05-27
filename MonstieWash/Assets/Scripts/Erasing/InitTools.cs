using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTools : MonoBehaviour
{
    [SerializeField] private Tool[] tools;
    [SerializeField] private Tool[] originalTools;

    void Awake()
    {
        for (var i = 0; i < tools.Length; i++)
        {
            tools[i].size = originalTools[i].size;
            tools[i].InputStrength = originalTools[i].InputStrength;
        }
    }
}
