using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Loading UI")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Slider progressBar;
    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] TextMeshProUGUI pressAnyButton;

    [Header("High Score UI")]
    [SerializeField] TextMeshProUGUI highscoreText;
    [SerializeField] TextMeshProUGUI secondsText;
    [SerializeField] TextMeshProUGUI minutesText;

    private void Start()
    {
        SetHighScore();
    }

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private void SetHighScore()
    {
        SaveManager saveManager = new SaveManager();
        string HighScore = saveManager.GetHighScore().ToString();
        int seconds = saveManager.GetBestTimeInSeconds();
        int minutes = saveManager.GetBestTimeInMinutes();
        highscoreText.text = HighScore;
        secondsText.text = seconds.ToString();
        minutesText.text = minutes.ToString();
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        //Enabling loading screen gameobject
        loadingScreen.SetActive(true);

        // Start loading the scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Calculate the loading progress (Clamp it for proper slider fill)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Update the progress bar and optional text
            progressBar.value = progress;
            if (progressText != null)
            {
                progressText.text = $"{(progress * 100):0}%"; // Example: "85%"
            }

            // Scene is ready to activate when progress reaches 90% or higher
            if (operation.progress >= 0.9f)
            {
                pressAnyButton.text = "Press Any Key to Continue"; // Optional message
                if (Input.anyKeyDown) // Wait for user input
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null; // Wait for the next frame
        }

        // Hide the loading screen after the scene is fully loaded
        loadingScreen.SetActive(false);
    }
}
