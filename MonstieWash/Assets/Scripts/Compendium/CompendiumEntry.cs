using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CompendiumEntry : MonoBehaviour
{
    public string Name { get; private set; }
    public string Temperament { get; private set; }
    public string Description { get; private set; }
    public Sprite ImageOriginal { get; private set; }
    public Sprite ImageCleared { get; private set; }
    public bool Completed { get; private set; }

    [SerializeField] private string name;
    [SerializeField] [TextAreaAttribute] private string temperament;
    [SerializeField] [TextAreaAttribute] private string description;
    [SerializeField] private Sprite imageOriginal;
    [SerializeField] private Sprite imageCleared;
    [SerializeField] private bool completed;

    [SerializeField] private CompendiumManager m_compendiumManager;

    private void Start()
    {
        Name = name;
        Temperament = temperament;
        Description = description;
        ImageOriginal = imageOriginal;
        ImageCleared = imageCleared;
        Completed = completed;

        m_compendiumManager = FindFirstObjectByType<CompendiumManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_compendiumManager.DisplayEntry(this);
    } 
}
