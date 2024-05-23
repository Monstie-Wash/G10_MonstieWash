using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Start is called before the first frame update
    public void Quit()
    { 
        Application.Quit();
        Debug.Log("Quitting...");
    }
}
