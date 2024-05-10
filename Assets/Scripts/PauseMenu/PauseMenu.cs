using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused
    {
        get;
        private set;
    }

    public static bool IsGameOver
    {
        get;
        private set;
    }

    public GameObject pauseTitle;
    public Selectable resumeButton;
    public GameObject gameOverTitle;
    public GameObject scoringPanel;
    public TextMeshProUGUI scoreText;
    public TMP_InputField playerNameInput;
    
    public void Pause()
    {
        scoreText.gameObject.SetActive(false);
        gameOverTitle.SetActive(false);
        scoringPanel.SetActive(false);
        
        pauseTitle.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
        
        Time.timeScale = 0f;
        IsPaused = true;
        resumeButton.Select();

        Cursor.lockState = CursorLockMode.Confined;
    }
    
    public void Continue()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GameOver()
    {
        scoreText.gameObject.SetActive(true);
        scoreText.text = "Score: " + GameManager.PlayerScore;
        
        pauseTitle.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        gameOverTitle.SetActive(true);
        scoringPanel.SetActive(true);

        gameObject.SetActive(true);
        IsPaused = true;
        IsGameOver = true;
        
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void SaveScoreToDB()
    {
        string playerName = playerNameInput.text.Length > 0 ? playerNameInput.text : "Roy";
        
        print("Saved score for " + playerName);
        
        GameManager.AddHighScoreEntry(playerName);
    }
    
    public void GoToMainMenu()
    {
        if (IsGameOver) SaveScoreToDB();
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        if (IsGameOver) SaveScoreToDB();
        
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();       
        #endif
    }
}
