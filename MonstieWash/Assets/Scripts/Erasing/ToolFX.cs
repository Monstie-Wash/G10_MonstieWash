using UnityEngine;

public class ToolFX : MonoBehaviour
{
    [Tooltip("Particles when tool in use")] [SerializeField] private ParticleSystem particlesOnUse;         // ParticleSystem for using the tool on an incomplete scene
    [SerializeField] private bool OnlySpawnOnErasing = true;
    [Tooltip("Particles when tool in use on a completed scene")] [SerializeField] private ParticleSystem particlesOnComplete;    // ParticleSystem for using a tool on a completed scene

    private ParticleSystem m_myParticles;
    private ParticleSystem m_completeParticles;
    private Transform m_parentTransform;
    private Transform m_drawPosTransform;
    private SoundPlayer m_soundPlayer;
    private Eraser m_eraser;

    private void Awake()
    {
        m_parentTransform = transform.parent;
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
        GameSceneManager.Instance.OnSceneSwitch += OnSceneSwitch;
    }

    private void OnDisable()
    {
        m_eraser.OnErasing_Started -= Eraser_OnErasing;
        m_eraser.OnErasing_Ended -= Eraser_OnErasing_Ended;
        InputManager.Instance.OnActivate -= Inputs_OnActivate;
        InputManager.Instance.OnActivate_Ended -= Inputs_OnActivate_Ended;
        GameSceneManager.Instance.OnSceneSwitch -= OnSceneSwitch;

        Inputs_OnActivate_Ended();
    }

    private void Start()
    {
        if (particlesOnUse != null) m_myParticles = Instantiate(particlesOnUse, m_drawPosTransform.position, Quaternion.identity, m_parentTransform);
        m_completeParticles = Instantiate(particlesOnComplete, m_drawPosTransform);
    }

    private void Eraser_OnErasing(bool completeScene, Tool tool)
    {
        if (!completeScene)
        {
            if (OnlySpawnOnErasing) m_myParticles?.Play(); // play non-complete particles
        }
        else 
        {
            m_completeParticles.Play(); // play complete particles
        }

    }

    private void Eraser_OnErasing_Ended(bool completeScene, Tool tool)
    {
        if (!completeScene)
        {
            if (OnlySpawnOnErasing) m_myParticles?.Stop();   // stop non-complete particles
        }
        else
        {
            m_completeParticles.Stop(); // stop complete particles
        }
    }

    private void Inputs_OnActivate()
    {
        if (!OnlySpawnOnErasing) m_myParticles?.Play();
        m_soundPlayer.PlaySound();
    }

    private void Inputs_OnActivate_Ended()
    {
        if (!OnlySpawnOnErasing) m_myParticles?.Stop();
        m_soundPlayer.StopSound();
    }

    private void OnSceneSwitch()
    {
        m_myParticles.Clear();
    }
}
