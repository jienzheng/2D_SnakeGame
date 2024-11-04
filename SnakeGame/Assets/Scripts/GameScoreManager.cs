using UnityEngine;

public class GameScoreManager : MonoBehaviour
{
    public int currentScore; // Keep track of the current score

    // Call this method when the score updates
    public void UpdateScore(int score)
    {
        currentScore += score;
        CheckHighScore();
    }

    private void CheckHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0); // Get the saved high score, default to 0 if not set

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore); // Update PlayerPrefs with the new high score
            PlayerPrefs.Save(); // Ensures the data is written to disk
        }
    }
}