using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public abstract class EnemyBehavior : MonoBehaviour
{
    [Header("Player Detection")]
    [Tooltip("How far can the enemy see?")]
    public float viewDistance = 30f;
    [Tooltip("What is the enemy's FOV?")]
    public float viewAngle = 180f;
    [Tooltip("If the player gets within the detection distance, they will instantly be detected")]
    public float detectionDistance = 5f;
    [Tooltip("Should the enemy automatically detect the player on spawn?")]
    public bool detectOnSpawn = false;
    
    [SerializeField]
    protected bool detectsPlayer;
    
    // Make sure you set currentAction to null at the end of each action!
    protected Coroutine currentAction;
    private Coroutine _actionHandlerCoroutine;

    protected Transform playerTransform;

    protected AudioSource audioSource;
    
    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        playerTransform = PlayerHealth.PlayerTransform;

        currentAction = StartCoroutine(WaitForPlayerDetected());
        _actionHandlerCoroutine = StartCoroutine(HandleActions());
        
        if (detectOnSpawn)
        {
            detectsPlayer = true;
            OnPlayerDetected();
        }
    }

    /// <summary>
    /// Function that determines which action should be run next.
    /// </summary>
    public abstract void ChooseNextAction();

    /// <summary>
    /// Function that runs when the Enemy is damaged.
    /// </summary>
    public virtual void OnDamage()
    {
        detectsPlayer = true; // Detect the player when damaged
    }

    /// <summary>
    /// Function that runs when the Enemy dies.
    /// </summary>
    public virtual void OnDeath()
    {
        enabled = false;
        StopAction();
    }

    /// <summary>
    /// Function that runs when the Enemy is frozen
    /// </summary>
    /// <param name="freezeTime">The amount of time for which the Enemy is frozen</param>
    public virtual void OnFreeze(float freezeTime)
    {
        StopAction();

        currentAction = StartCoroutine(Delay(freezeTime));
    }
    
    /// <summary>
    /// Stops the current action
    /// </summary>
    protected void StopAction()
    {
        if (currentAction != null) StopCoroutine(currentAction);
    }

    /// <summary>
    /// Function that is triggered when the Enemy detects the Player.
    /// </summary>
    protected virtual void OnPlayerDetected()
    {
        
    }

    /// <summary>
    /// Checks to see if the Enemy can now detect the player
    /// </summary>
    protected virtual void CheckForDetection()
    {
        if (GetDistanceToPlayer() < detectionDistance) detectsPlayer = true;
    }
    
    /// <summary>
    /// Gets the distance between the Enemy and the Player
    /// </summary>
    /// <returns>The distance</returns>
    protected virtual float GetDistanceToPlayer()
    {
        return Vector3.Distance(playerTransform.position, transform.position);
    }
    
    /// <summary>
    /// IEnumerator that ensures that the enemy always has an action to do.
    /// This will always be running in the background of the enemy's behavior.
    /// </summary>
    private IEnumerator HandleActions()
    {
        while (true)
        {
            if (currentAction == null)
            {
                #if UNITY_EDITOR
                if (debugMessages) print("choosing action");
                #endif
                ChooseNextAction();
            }

            yield return null;
        }
    }
    
    #region Actions
    
    /// <summary>
    /// Action that causes the Enemy to wait until it detects the Player.
    /// Calls <b>CheckForDetection</b> every frame until the Player is detected.
    /// </summary>
    protected IEnumerator WaitForPlayerDetected()
    {
        #if UNITY_EDITOR
        if (debugMessages) print("Action - Waiting for player detection");
        #endif
        
        while (!detectsPlayer)
        {
            CheckForDetection();
            
            yield return null;
        }

        currentAction = null;
        OnPlayerDetected();
    }
    
    /// <summary>
    /// Action that causes the Enemy to wait for a specified amount of time before picking a new action.
    /// </summary>
    /// <param name="delayTime">How long to delay, in seconds</param>
    protected IEnumerator Delay(float delayTime)
    {
        #if UNITY_EDITOR
        if (debugMessages) print("Action - Waiting for " + delayTime + " seconds");
        #endif
        
        yield return new WaitForSeconds(delayTime);

        currentAction = null;
    }
    
    #endregion

#if UNITY_EDITOR
    public bool visualizeDetection = false;
    public bool debugMessages = false;
    
    protected virtual void OnDrawGizmosSelected()
    {
        if (!visualizeDetection) return;
        
        var arcColor = Color.yellow;
        arcColor.a = 0.5f;
        
        UnityEditor.Handles.color = arcColor;

        Vector3 arcStart =
            Vector3.RotateTowards(transform.forward, -transform.forward, viewAngle * Mathf.Deg2Rad / 2, 1f);
        
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, arcStart, viewAngle, viewDistance);

        arcColor = Color.red;
        arcColor.a = 0.5f;
        UnityEditor.Handles.color = arcColor;
        
        UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, detectionDistance);
    }
    #endif
}
