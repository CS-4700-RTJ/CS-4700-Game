using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // private static EventManager _instance;
    
    

    public static event Action OnPlayerDeath;

    public static void TriggerOnPlayerDeath()
    {
        OnPlayerDeath?.Invoke();
        HighScoreTable.AddHighScoreEntry(10,"Bob");
    }

    // private void Awake()
    // {
    //     if (_instance == null)
    //     {
    //         _instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(this);
    //     }
    // }
}
