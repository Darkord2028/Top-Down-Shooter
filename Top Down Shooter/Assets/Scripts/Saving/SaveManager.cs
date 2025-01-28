using UnityEngine;

public class SaveManager
{
    private const string HighScoreKey = "HighScore";
    private const string BestTimeInSeconds = "BestTimeInSeconds";
    private const string BestTimeInMinutes = "BestTimeInMinutes";

    #region Save Functions

    // Save the high score if it's higher than the current saved score
    public void SaveHighScore(int score)
    {
        int currentHighScore = GetHighScore();
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save(); // Ensure the data is written to disk
        }
    }

    // Save the best time in seconds if it's higher than the current seconds
    public void SaveTimeInSeconds(int seconds)
    {
        int currentSeconds = GetBestTimeInSeconds();
        if (seconds > currentSeconds)
        {
            PlayerPrefs.SetInt(BestTimeInSeconds, seconds);
            PlayerPrefs.Save(); // Ensure the data is written to disk
        }
    }

    // Save the best time in minutes if it's higher than the current minutes
    public void SaveTimeInMinutes(int minutes)
    {
        int currentSeconds = GetBestTimeInMinutes();
        if (minutes > currentSeconds)
        {
            PlayerPrefs.SetInt(BestTimeInMinutes, minutes);
            PlayerPrefs.Save(); // Ensure the data is written to disk
        }
    }

    #endregion

    #region Get Functions
    // Get the current high score
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0); // Default to 0 if no high score exists
    }

    public int GetBestTimeInSeconds()
    {
        return PlayerPrefs.GetInt(BestTimeInSeconds, 0); // Default to 0 if no best time exists
    }

    public int GetBestTimeInMinutes()
    {
        return PlayerPrefs.GetInt(BestTimeInMinutes, 0); // Default to 0 if no best time exists
    }

    #endregion

    #region Reset Functions

    // Reset the high score
    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
    }

    // Reset the best time
    public void ResetBestTimeInSeconds()
    {
        PlayerPrefs.DeleteKey(BestTimeInSeconds);
    }

    public void ResetBestTimeInMinutes()
    {
        PlayerPrefs.DeleteKey(BestTimeInMinutes);
    }

    #endregion

}
