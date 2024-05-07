using System.Collections;
using System.Collections.Generic;
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

    public Selectable firstSelectedObject;

    public void Pause()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
        firstSelectedObject.Select();

        Cursor.lockState = CursorLockMode.Confined;
    }
    
    public void Continue()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
        
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();       
        #endif
    }
}
