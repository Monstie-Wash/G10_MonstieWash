using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CompendiumEntry : MonoBehaviour
{
    public string MonsterName { get => monsterName;}
    public string Temperament { get => temperament; }
    public string Description { get => description;}
    public Sprite ImageOriginal { get => imageOriginal; }
    public Sprite ImageCleared { get => imageCleared; }
    public bool Completed { get => completed;}

    [SerializeField] private string monsterName;
    [SerializeField] [TextAreaAttribute] private string temperament;
    [SerializeField] [TextAreaAttribute] private string description;
    [SerializeField] private Sprite imageOriginal;
    [SerializeField] private Sprite imageCleared;
    [SerializeField] private bool completed;

    private CompendiumManager m_compendiumManager;

    private void Start()
    {
        m_compendiumManager = FindFirstObjectByType<CompendiumManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_compendiumManager.DisplayEntry(this);
    } 
}
