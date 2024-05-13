using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var isHand = collision.gameObject.GetComponentInChildren<PlayerHealth>();
        if (isHand != null)
        {
            Debug.Log("haha you died idiot");
            isHand.TakeDamage(20);
        }
    }
}
