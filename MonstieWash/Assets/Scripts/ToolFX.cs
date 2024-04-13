using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToolFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem particlesOnUse;
    ParticleSystem myParticles;

    private void Start()
    {
        myParticles = Instantiate(particlesOnUse, transform); 
        myParticles.Stop();
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

        /// <summary>
    /// Called every frame until the activate button is released.
    /// </summary>
    private void StartParticles()
    {
        Debug.Log("Subscribed");
        myParticles.Play();
    }

    private void StopParticles() 
    {
        myParticles.Stop();
    }

}