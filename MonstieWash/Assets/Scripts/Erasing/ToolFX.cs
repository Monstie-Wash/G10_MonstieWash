using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem particlesOnUse;

    private ParticleSystem m_myParticles;
    private Transform m_drawPosTransform;
    private Eraser m_eraser;

    private void Awake()
    {
        m_drawPosTransform = transform.GetChild(0);
        m_eraser = GetComponent<Eraser>();
    }

    private void OnEnable()
    {
        m_eraser.OnErasing_Started += Eraser_OnErasing;
        m_eraser.OnErasing_Ended += Eraser_OnErasing_Ended;
    }

    private void OnDisable()
    {
        m_eraser.OnErasing_Started -= Eraser_OnErasing;
        m_eraser.OnErasing_Ended -= Eraser_OnErasing_Ended;
    }

    private void Start()
    {
        m_myParticles = Instantiate(particlesOnUse, m_drawPosTransform);
        m_myParticles.Stop();
    }

    private void Eraser_OnErasing()
    {
        m_myParticles.Play();
    }

    private void Eraser_OnErasing_Ended()
    {
        m_myParticles.Stop();
    }
}
