using System;
using UnityEngine;

public class DeathScreenTrigger : MonoBehaviour
{ void Update()
    {
        
        //
        // Summary: Checks for space key, when pressed calls DeathHandler
        //
        if (Input.GetKeyDown("Space"))
        {
            DeathHandler();
        }
    } 
    
    private void DeathHandler()
    {
        Time.timeScale = 0;
        // Chantelle write any code to activate and initialize the UI deathscreen here.
    }
}
