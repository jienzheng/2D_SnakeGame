using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    static private TextMeshProUGUI _UI_TEXT;
    static private int _SCORE = 0;

    private void Awake()
    {
        _UI_TEXT = GetComponent<TextMeshProUGUI>();

        // Initialize SCORE with the saved high score or start at 0
        _SCORE = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreText();
    }

    static public int SCORE
    {
        get { return _SCORE; }
        private set
        {
            _SCORE = value;
            PlayerPrefs.SetInt("HighScore", value);
            UpdateHighScoreText();
        }
    }

    static private void UpdateHighScoreText()
    {
        if (_UI_TEXT != null)
        {
            _UI_TEXT.text = "High Score: " + _SCORE.ToString("#,0");
        }
    }

    static public void TRY_SET_HIGH_SCORE(int scoreToTry)
    {
        if (scoreToTry > SCORE)
        {
            SCORE = scoreToTry;
        }
    }

    [Tooltip("Check this box to reset the HighScore in PlayerPrefs")]
    public bool resetHighScoreNow = false;

    void OnDrawGizmos()
    {
        if (resetHighScoreNow)
        {
            resetHighScoreNow = false;
            PlayerPrefs.SetInt("HighScore", 0);
            Debug.LogWarning("PlayerPrefs Highscore Reset to 0.");
        }
    }
}