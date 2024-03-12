using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        detectsPlayer = detectOnSpawn;
    }

    public abstract void ChooseNextAction();

    public virtual void OnDamage()
    {
        detectsPlayer = true; // Detect the player when damaged
    }

    public virtual void OnDeath()
    {
        enabled = false;
        StopAction();
    }

    public virtual void OnFreeze(float freezeTime)
    {
        StopAction();

        currentAction = StartCoroutine(Delay(freezeTime));
    }
    
    public void StopAction()
    {
        if (currentAction != null) StopCoroutine(currentAction);
    }
    
    protected IEnumerator WaitForPlayerDetected()
    {
        print("Action - Waiting for player detection");
        while (!detectsPlayer)
        {
            CheckForDetection();
            
            yield return null;
        }

        currentAction = null;
        OnPlayerDetected();
    }

    protected virtual void OnPlayerDetected()
    {
        
    }

    protected virtual void CheckForDetection()
    {
        if (GetDistanceToPlayer() < detectionDistance) detectsPlayer = true;
    }
    
    protected IEnumerator Delay(float delayTime)
    {
        print("Action - Waiting for " + delayTime + " seconds");
        yield return new WaitForSeconds(delayTime);

        currentAction = null;
    }
    
    /// <summary>
    /// IEnumerator that ensures that the enemy always has an action to do.
    /// </summary>
    private IEnumerator HandleActions()
    {
        while (true)
        {
            if (currentAction == null)
            {
                print("choosing action");
                ChooseNextAction();
            }

            yield return null;
        }
    }

    protected virtual float GetDistanceToPlayer()
    {
        return Vector3.Distance(playerTransform.position, transform.position);
    }
    
    #if UNITY_EDITOR
    public bool visualizeDetection = true;
    
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
