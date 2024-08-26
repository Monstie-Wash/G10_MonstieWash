using UnityEngine;

public class ToolFX : MonoBehaviour
{
    [Tooltip("Particles when tool in use")] [SerializeField] private ParticleSystem particlesOnUse;         // ParticleSystem for using the tool on an incomplete scene
    [Tooltip("Particles when tool in use on a completed scene")] [SerializeField] private ParticleSystem particlesOnComplete;    // ParticleSystem for using a tool on a completed scene

    private ParticleSystem m_myParticles;
    private ParticleSystem m_completeParticles;
    private Transform m_drawPosTransform;
    private SoundPlayer m_soundPlayer;
    private Eraser m_eraser;

    private void Awake()
    {
        m_drawPosTransform = transform.GetChild(0);
        m_soundPlayer = GetComponent<SoundPlayer>();
        m_eraser = GetComponent<Eraser>();
    }

    private void OnEnable()
    {
        m_eraser.OnErasing_Started += Eraser_OnErasing;
        m_eraser.OnErasing_Ended += Eraser_OnErasing_Ended;
        InputManager.Instance.OnActivate += Inputs_OnActivate;
        InputManager.Instance.OnActivate_Ended += Inputs_OnActivate_Ended;
    }

    private void OnDisable()
    {
        m_eraser.OnErasing_Started -= Eraser_OnErasing;
        m_eraser.OnErasing_Ended -= Eraser_OnErasing_Ended;
        InputManager.Instance.OnActivate -= Inputs_OnActivate;
        InputManager.Instance.OnActivate_Ended -= Inputs_OnActivate_Ended;
    }

    private void Start()
    {
        m_myParticles = Instantiate(particlesOnUse, m_drawPosTransform);
        m_completeParticles = Instantiate(particlesOnComplete, m_drawPosTransform);
    }

    private void Eraser_OnErasing(bool completeScene)
    {
        if (!completeScene)
        {
            m_myParticles.Play(); // play non-complete particles
        }
        else 
        {
            m_completeParticles.Play(); // play complete particles
        }
    }

    private void Eraser_OnErasing_Ended(bool completeScene)
    {
        if (!completeScene)
        {
            m_myParticles.Stop();   // stop non-complete particles
        }
        else
        {
            m_completeParticles.Stop(); // stop complete particles
        }
    }

    private void Inputs_OnActivate()
    {
        m_soundPlayer.PlaySound();
    }

    private void Inputs_OnActivate_Ended()
    {
        m_soundPlayer.StopSound();
    }
}
