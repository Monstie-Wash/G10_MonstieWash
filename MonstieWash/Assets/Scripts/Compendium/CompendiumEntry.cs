using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CompendiumEntry : MonoBehaviour, IPointerEnterHandler
{
    public string Name { get; private set; }
    public string Temperament { get; private set; }
    public string Description { get; private set; }
    public Image ImageOriginal { get; private set; }
    public Image ImageCleared { get; private set; }
    public bool Completed { get; set; }

    [SerializeField] private string name;
    [SerializeField] [TextAreaAttribute] private string temperament;
    [SerializeField] [TextAreaAttribute] private string description;
    [SerializeField] private Image imageOriginal;
    [SerializeField] private Image imageCleared;

    private CompendiumManager m_compendiumManager;

    private void Start()
    {
        Name = name;
        Temperament = temperament;
        Description = description;
        ImageOriginal = ImageOriginal;
        ImageCleared = ImageCleared;
        Completed = false;

        m_compendiumManager = FindFirstObjectByType<CompendiumManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_compendiumManager.DisplayEntry(this);
    }
}
