using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToolFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem particlesOnUse;
    private ParticleSystem m_myParticles;

    private void Start()
    {
        m_myParticles = Instantiate(particlesOnUse, transform); 
        m_myParticles.Stop();
    }

    private void OnEnable()
    {
        InputManager.Inputs.OnActivate_Started += StartParticles;
        InputManager.Inputs.OnActivate_Ended += StopParticles;
    }

    private void OnDisable()
    {
        InputManager.Inputs.OnActivate_Started -= StartParticles;
        InputManager.Inputs.OnActivate_Ended -= StopParticles;
    }

    private void StartParticles()
    {
        m_myParticles.Play();
    }

    private void StopParticles() 
    {
        m_myParticles.Stop();
    }

}