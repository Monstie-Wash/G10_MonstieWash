using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFeature : MonoBehaviour
{
    //Hello, this is a file for the git tutorial!
    //Pretend that there's lots of great code in here
    private float moveSpd = 5f;
    private int secretNumber = 102946;
    private string secretMessage = "Hello, World!";

    private void Start()
    {
        Debug.Log($"Secret number is: {secretNumber} | Secret message is: {secretMessage}");
    }
}
