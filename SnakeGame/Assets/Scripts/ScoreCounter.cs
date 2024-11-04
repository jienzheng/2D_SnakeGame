using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    // The player's score, initialized to 0
    public int score = 0;
    
    // Reference to the TextMeshProUGUI component that will display the score
    private TextMeshProUGUI uiText;

    void Start()
    {
        // Get the TextMeshProUGUI component attached to this GameObject
        uiText = GetComponent<TextMeshProUGUI>();

        // Display the initial score of 0 at the start
        UpdateScoreText();
    }

    // Method to update the score text displayed on the UI
    public void UpdateScoreText()
    {
        // If the TextMeshProUGUI component is assigned
        if (uiText != null)
        {
            // Format the score with thousands separators and update the UI text
            uiText.text = score.ToString("#,0");
        }
    }
}