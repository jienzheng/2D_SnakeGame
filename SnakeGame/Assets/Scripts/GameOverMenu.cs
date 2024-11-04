using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    // Method called when the Play Again button is pressed
    public void onPlayButton()
    {
        // Loads the main game scene (Scene index 1 should be set to the main game in the Build Settings)
        SceneManager.LoadScene(1);
    }

    // Method called when the Quit button is pressed
    public void onQuitButton()
    {
        // Exits the application (note: this only works in a built application, not in the editor)
        Application.Quit();
    }
}