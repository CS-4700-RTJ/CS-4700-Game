using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    public TMP_Text difficultyText;
    
    // Load Game Scene
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Quit Game
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    public void SetDifficulty(DifficultyOptionSO difficulty) 
    {
        GameManager.SetDifficulty(difficulty);

        difficultyText.text = String.Format("Spawn Rate: {0} seconds\n" +
                                            "Upgrade Rate: {1} seconds\n" +
                                            "Point Multiplier: {2}x",
                                            difficulty.timeBetweenSpawns, difficulty.timeBetweenUpgrades, difficulty.pointMultiplier);
    }
}
