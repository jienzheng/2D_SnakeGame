using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public int score = 0; // Ensure this starts at 0
    private TextMeshProUGUI uiText;

    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        UpdateScoreText(); // Display the initial score of 0
    }

    // Method to update the score display text
    public void UpdateScoreText()
    {
        if (uiText != null)
        {
            uiText.text = score.ToString("#,0");
        }
    }
}