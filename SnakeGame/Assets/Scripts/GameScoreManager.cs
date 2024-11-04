using UnityEngine;

public class GameScoreManager : MonoBehaviour
{
    // Variable to keep track of the current score
    public int currentScore;

    // Method to update the score; call this whenever points are earned
    public void UpdateScore(int score)
    {
        // Add the new score to the current score
        currentScore += score;
        
        // Check if the current score is a new high score
        CheckHighScore();
    }

    // Checks and updates the high score if the current score exceeds it
    private void CheckHighScore()
    {
        // Retrieve the saved high score from PlayerPrefs, defaulting to 0 if no high score is set
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        // If the current score is greater than the saved high score
        if (currentScore > highScore)
        {
            // Update PlayerPrefs with the new high score
            PlayerPrefs.SetInt("HighScore", currentScore);
            
            // Save PlayerPrefs to ensure the high score is saved persistently
            PlayerPrefs.Save();
        }
    }
}