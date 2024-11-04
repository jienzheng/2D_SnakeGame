using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1; // Unpause the game if it was paused
        SceneManager.LoadScene("GameScene"); 
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1; // Unpause the game if it was paused
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with the name of your main menu scene
    }
}