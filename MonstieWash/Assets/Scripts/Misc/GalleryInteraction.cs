using UnityEngine;

public class GalleryInteraction : MonoBehaviour
{
    private float m_rotatingDirection;
    private bool m_holding = true;
    private Transform m_currentPolaroid;
    private float m_pickupDistance = 1.5f;
    private float m_rotation;
    private GalleryManager m_galleryManager;
    private GameObject m_finishButton;

    private void Start()
    {
        m_galleryManager = FindFirstObjectByType<GalleryManager>();
        m_finishButton = FindFirstObjectByType<FinishLevelButton>().gameObject;
        m_finishButton.SetActive(false);
        m_currentPolaroid = m_galleryManager.CurrentPolaroid.transform;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnSwitchTool += CycleOptions;
        InputManager.Instance.OnSwitchTool_Ended += ResetRotation;
        InputManager.Instance.OnActivate += Interact;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnSwitchTool -= CycleOptions;
        InputManager.Instance.OnSwitchTool_Ended -= ResetRotation;
        InputManager.Instance.OnActivate -= Interact;
    }

    private void Update()
    {
        //Rotate held object if desired.
        if (m_holding && m_rotatingDirection != 0)
        {
            var rotation = 100 * m_rotatingDirection * Time.deltaTime;
            m_rotation = Mathf.Clamp(m_rotation + rotation, -30f, 30f);
            m_currentPolaroid.rotation = Quaternion.Euler(0, 0, m_rotation);
        }
    }

    /// <summary>
    /// Either places or picks up the current polaroid
    /// </summary>
    private void Interact()
    {
        if (m_holding)
        {
            if (m_galleryManager.Bounds.Contains(m_currentPolaroid.transform.position))
            {
                m_currentPolaroid.parent = m_galleryManager.transform;
                m_finishButton.SetActive(true);
                m_holding = false;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, m_currentPolaroid.position) <= m_pickupDistance)
            {
                m_currentPolaroid.parent = transform;
                m_finishButton.SetActive(false);
                m_holding = true;
            }
        }
    }

    /// <summary>
    /// Takes an input direction from input manager and calls correct cycle method.
    /// </summary>
    /// <param name="dir">-1 for left 1 for right.</param>
    private void CycleOptions(int dir)
    {
        m_rotatingDirection = -dir;
    }

    /// <summary>
    /// Called by releasing rotation input.
    /// </summary>
    private void ResetRotation()
    {
        m_rotatingDirection = 0f;
    }
}
