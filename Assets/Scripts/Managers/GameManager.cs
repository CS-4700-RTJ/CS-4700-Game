using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    [Header("Scoring")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreChangeText;
    [Tooltip("The score change animation will take this long to complete")]
    public float scoreChangeTime = 1.5f;
    [Tooltip("The score change text will be visible for this amount of time before visibly applying points")]
    public float scoreChangeDelay = 1f;

    // actual player score value
    private int _playerScore;
    
    // static readonly property for everyone to see
    public static int PlayerScore
    {
        get => _instance._playerScore; // anyone can see the player's score

        private set => _instance._playerScore = value; // but only this class can change it
    }
    
    // Score Change variables
    private int _remainingScoreChange = 0;
    private float _scoreChangeTimer;

    private Coroutine _scoreUpdateRoutine;
    private bool _isDelayed;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            scoreText.text = "Score: 0";
            scoreChangeText.text = "";
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Animates the increase in the player's score so that the player can clearly see how many points they gained
    /// while smoothly transitioning to the player's new score.
    /// </summary>
    private IEnumerator LerpScores()
    {
        // Show the total amount of points that are being added/subtracted
        scoreChangeText.text = _remainingScoreChange > 0
            ? "+" + _remainingScoreChange
            : "" + _remainingScoreChange;

        _isDelayed = true;
        
        // Wait for a certain delay before animating, so the player can see how many points were gained
        yield return new WaitForSeconds(scoreChangeDelay);

        _isDelayed = false;
        
        // Only calculate the start score once - this is the score the player had before gaining points
        float startScore = PlayerScore - _remainingScoreChange;
        
        // animate the score indicator's
        while (_scoreChangeTimer < 1)
        {
            // calculate the current score to display via Lerp between the start and end scores
            int currentScore = (int)Mathf.Lerp(startScore, PlayerScore, _scoreChangeTimer);

            // Show the "+" sign on positive numbers - Negative numbers automatically show a "-" sign
            scoreChangeText.text = _remainingScoreChange > 0
                ? "+" + (PlayerScore - currentScore)
                : "" + (PlayerScore - currentScore);
            scoreText.text = "Score: " + currentScore;

            // Increase the lerp timer
            _scoreChangeTimer += Time.deltaTime / scoreChangeTime;
            
            yield return null;
        }

        // The animation is complete, hide the score change indicator and make sure the player's final score is correct
        _remainingScoreChange = 0;
        scoreText.text = "Score: " + PlayerScore;
        scoreChangeText.text = "";
    }

    /// <summary>
    /// Increases the player's score by the specified amount of <b>points</b>.
    /// Use a negative value to subtract from the player's score.
    /// </summary>
    /// <param name="points">The number of points being gained</param>
    public static void IncreaseScore(int points)
    {
        // Don't do anything if the score doesn't change
        if (points == 0) return;

        // Stop the previous Coroutine that was updating the player's score
        if (_instance._scoreUpdateRoutine != null)
        {
            // If the score was actively animating, then jump to the end
            // Otherwise, the score gain indicator is increased so everything can animate at the same time
            if (!_instance._isDelayed)
            {
                _instance._remainingScoreChange = 0;
                _instance.scoreText.text = "Score: " + PlayerScore;
            }

            _instance.StopCoroutine(_instance._scoreUpdateRoutine);
        }
        
        //Increase the player's score
        PlayerScore += points;
        _instance._remainingScoreChange += points;
        _instance._scoreChangeTimer = 0f;
        
        // Start the Coroutine to update the score indicators
        _instance._scoreUpdateRoutine = _instance.StartCoroutine(_instance.LerpScores());
    }
}
