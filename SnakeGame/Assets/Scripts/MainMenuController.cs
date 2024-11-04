using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Called when the "Play" button is clicked
    public void onPlayButton()
    {
        // Loads the game scene, assumed to be at index 1 in the Build Settings
        SceneManager.LoadScene(1);
    }

    // Called when the "Quit" button is clicked
    public void onQuitButton()
    {
        // Exits the application (will not work in the Unity editor)
        Application.Quit();
    }
}