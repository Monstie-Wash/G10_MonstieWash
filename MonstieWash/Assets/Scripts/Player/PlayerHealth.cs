using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [Tooltip("Player's current health (initial value is starting health)")][SerializeField] private float playerHealth;
    [Tooltip("Damage beyond this value won't affect the intensity of damage animations")][SerializeField] private float damageAnimationCap;
    [Tooltip("Duration of the damage animation (in seconds)")][SerializeField] private float damageAnimationDuration;
    [Tooltip("Animation curve for screen shake upon taking damage")][SerializeField] private AnimationCurve damageShake;
    [Tooltip("The collider for being hit by attacks")][SerializeField] private Collider2D hitbox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TakeDamage(3f);
    }

    public void TakeDamage(float dmgTaken)
    {
        playerHealth -= dmgTaken;
        StartCoroutine(PlayDamageEffects(dmgTaken));
    }

    IEnumerator PlayDamageEffects(float dmgTaken)
    {
        var activeCam = Camera.main;
        var camStartPos = activeCam.transform.position;
        var elapsedTime = 0f;
        var dmgNormal = Mathf.Clamp((dmgTaken / damageAnimationCap) + 1, 1, 2);

        while (elapsedTime < damageAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            var strength = damageShake.Evaluate(elapsedTime / dmgTaken);
            activeCam.transform.position = camStartPos + Random.insideUnitSphere * strength * dmgNormal;
            yield return null;
        }

        activeCam.transform.position = camStartPos;
    }

}
