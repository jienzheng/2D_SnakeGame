using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    // Static reference to the TextMeshPro component displaying the high score
    static private TextMeshProUGUI _UI_TEXT;

    // Static variable to store the high score
    static private int _SCORE = 0;

    private void Awake()
    {
        // Get the TextMeshPro component attached to the same GameObject
        _UI_TEXT = GetComponent<TextMeshProUGUI>();

        // Initialize the high score from saved PlayerPrefs, or start at 0 if no high score is saved
        _SCORE = PlayerPrefs.GetInt("HighScore", 0);

        // Update the high score text on the UI
        UpdateHighScoreText();
    }

    // Property to get or set the high score with automatic saving to PlayerPrefs
    static public int SCORE
    {
        get { return _SCORE; }
        private set
        {
            _SCORE = value;

            // Save the high score in PlayerPrefs for persistence
            PlayerPrefs.SetInt("HighScore", value);

            // Update the high score display on the UI
            UpdateHighScoreText();
        }
    }

    // Updates the high score text displayed on the screen
    static private void UpdateHighScoreText()
    {
        if (_UI_TEXT != null)
        {
            // Format the score with thousands separators (e.g., "1,000")
            _UI_TEXT.text = "High Score: " + _SCORE.ToString("#,0");
        }
    }

    // Static method to attempt updating the high score if a new high score is achieved
    static public void TRY_SET_HIGH_SCORE(int scoreToTry)
    {
        if (scoreToTry > SCORE)
        {
            SCORE = scoreToTry; // Update the high score if the new score is higher
        }
    }

    // Tooltip for resetting the high score from the Unity editor
    [Tooltip("Check this box to reset the HighScore in PlayerPrefs")]
    public bool resetHighScoreNow = false;

    // This method runs in the editor to reset the high score when the checkbox is checked
    void OnDrawGizmos()
    {
        if (resetHighScoreNow)
        {
            resetHighScoreNow = false; // Uncheck the box after resetting
            PlayerPrefs.SetInt("HighScore", 0); // Reset the high score in PlayerPrefs
            Debug.LogWarning("PlayerPrefs Highscore Reset to 0.");
        }
    }
}