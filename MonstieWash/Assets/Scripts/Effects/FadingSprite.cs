using UnityEngine;

public class FadingSprite : MonoBehaviour
{
    private SpriteRenderer m_sprite;
    public enum state
    {
        Full,
        FadingIn,
        FadingOut,
        Faded
    }

    [SerializeField] private state status;
    [SerializeField] private float m_currentAlpha;
    [Tooltip("How high the alpha of the sprite must reach to be fully faded in (1-255)")] [SerializeField] private float upperAlphaLimit;
    [Tooltip("How low the alpha of the sprite must reach to be fully faded out (0-254)")] [SerializeField] private float lowerAlphaLimit;
    [Tooltip("How quickly the sprite fades in")] [SerializeField] private float fadeInSpeed;
    [Tooltip("How quickly the sprite fades out")] [SerializeField] private float fadeOutSpeed;
    [Tooltip("Time this sprite will remain at full fade before fading out again. Set to negative number if explicit control of fading out wanted.")][SerializeField] private float fullFadeTime;


    private float m_InternalFullTime; //How long has this remained at full fade.

    private void Awake()
    {
        m_sprite = GetComponent<SpriteRenderer>();
        m_currentAlpha = 0f;
    }

    public void FadeIn()
    {
        status = state.FadingIn;
    }

    public void FadeOut()
    {
        status = state.FadingOut;
    }



    private void Update()
    {
        switch(status)
        {
            case state.FadingOut:
                //Fade out sprite.
                m_currentAlpha = Mathf.MoveTowards(m_currentAlpha, lowerAlphaLimit, fadeOutSpeed * Time.deltaTime);
                m_sprite.color = new Color(255, 255, 255, Mathf.InverseLerp(0, 255, m_currentAlpha));
                //Check if should transition to fully faded.
                if (m_currentAlpha <= lowerAlphaLimit) status = state.Faded;

                break;

            case state.FadingIn:
                //Fade in sprite.
                m_currentAlpha = Mathf.MoveTowards(m_currentAlpha, upperAlphaLimit, fadeInSpeed * Time.deltaTime);
                m_sprite.color = new Color(255, 255, 255, Mathf.InverseLerp(0,255, m_currentAlpha));
                //Check if should transition to fully faded.
                if (m_currentAlpha >= upperAlphaLimit)
                {
                    status = state.Full;
                    m_InternalFullTime = 0f;
                }
                break;

            case state.Full:
                if (fullFadeTime < 0) return;

                //Increment timer.
                m_InternalFullTime += Time.deltaTime;
                //If timer has reached limit then begin fading out.
                if (m_InternalFullTime >= fullFadeTime) status = state.FadingOut;

                break;
        }
    }
}
