using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryInteraction : MonoBehaviour
{
    private Transform playerHand;

    private void Awake()
    {
        playerHand = FindFirstObjectByType<PlayerHand>().transform;
        transform.position = playerHand.position;
        transform.parent = playerHand;
    }
}
