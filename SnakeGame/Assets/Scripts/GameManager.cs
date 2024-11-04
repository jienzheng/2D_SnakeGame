using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int score = 0;              // Tracks the player's score
    public Text scoreText;             // Reference to a UI Text element to display the score

    // This method updates the score display
    public void SetScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    // You could add other game management logic here as needed
}
