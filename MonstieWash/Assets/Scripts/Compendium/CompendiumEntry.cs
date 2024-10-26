using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CompendiumEntry : MonoBehaviour
{
    [SerializeField] private string monsterName;
    [SerializeField] [TextAreaAttribute] private string temperament;
    [SerializeField] [TextAreaAttribute] private string description;
    [SerializeField] private Sprite imageOriginal;
    [SerializeField] private Sprite imageCleared;
    [SerializeField] private bool completed;

    private CompendiumManager m_compendiumManager;
    private PlayerHand m_playerHand;

    public string MonsterName { get => monsterName;}
    public string Temperament { get => temperament; }
    public string Description { get => description;}
    public Sprite ImageOriginal { get => imageOriginal; }
    public Sprite ImageCleared { get => imageCleared; }
    public bool Completed { get => completed;}

    private void Awake()
    {
        m_compendiumManager = FindFirstObjectByType<CompendiumManager>();
        m_playerHand = FindFirstObjectByType<PlayerHand>();
    }

    private void OnEnable()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), m_playerHand.GetComponentInChildren<PolygonCollider2D>(), true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_compendiumManager.DisplayEntry(this);
    } 
}
