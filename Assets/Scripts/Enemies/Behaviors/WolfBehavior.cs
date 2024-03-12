using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WolfBehavior : EnemyBehavior
{
    [Header("Movement")] 
    public float baseSpeed = 5f;
    public float runSpeed = 7f;
    public float distanceForRunning = 5f;

    [Header("Attacks")] 
    public AudioClip[] attackSounds;
    public float idealPlayerDistance = 2f;
    public float runAttackTimeout = 5f;
    public float biteDamage = 2f;
    public float lungeDamage = 2f;
    private float _currentDamage;
    
    [Header("Debugging")]
    public bool lungeAttackNext = false;
    public bool chasePlayerNext = false;
    public bool howlNext = false;
    
    [Header("Attack Colliders")]
    public Collider attackCollider;

    private NavMeshAgent _agent;
    private Animator _animator;

    private static readonly int AnimatorMoveSpeed = Animator.StringToHash("Move Speed");
    private static readonly int AnimatorHowl = Animator.StringToHash("Howl");
    private static readonly int AnimatorLungeAttack = Animator.StringToHash("Lunge");
    private static readonly int AnimatorBiteAttack = Animator.StringToHash("Bite");

    protected override void Start()
    {
        base.Start();

        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(Howl());
    }

    private IEnumerator LungingBite()
    {
        print("Action - Lunging Bite");

        _animator.SetTrigger(AnimatorLungeAttack);
        _currentDamage = lungeDamage;

        float lungeAnimLength = 2f;

        yield return new WaitForSeconds(lungeAnimLength);

        currentAction = null;
    }

    private IEnumerator RunningBite()
    {
        print("Action - Running Bite");

        _animator.SetTrigger(AnimatorBiteAttack);
        _currentDamage = biteDamage;

        float biteAnimLength = 1.7f;

        _agent.speed = baseSpeed;
        _animator.SetFloat(AnimatorMoveSpeed, 0.5f);
        _agent.isStopped = false;

        while (biteAnimLength >= 0)
        {
            _agent.SetDestination(playerTransform.position);
            _agent.speed = GetDistanceToPlayer() >= distanceForRunning ? runSpeed : baseSpeed;
            _animator.SetFloat(AnimatorMoveSpeed, _agent.speed >= runSpeed ? 1 : 0.5f);

            yield return null;

            biteAnimLength -= Time.deltaTime;
        }

        _animator.SetFloat(AnimatorMoveSpeed, 0f);
        _agent.isStopped = true;

        currentAction = StartCoroutine(Delay(1f));
    }

    private IEnumerator ChasePlayer()
    {
        print("Action - Chase Player");

        _agent.isStopped = false;
        
        while (GetDistanceToPlayer() > idealPlayerDistance)
        {
            _agent.SetDestination(playerTransform.position);
            _agent.speed = GetDistanceToPlayer() >= distanceForRunning ? runSpeed : baseSpeed;
            _animator.SetFloat(AnimatorMoveSpeed, _agent.speed >= runSpeed ? 1 : 0.5f);

            yield return null;
        }

        currentAction = StartCoroutine(RunningBite());
    }

    private IEnumerator Howl()
    {
        print("Action - Howl");

        _animator.SetTrigger(AnimatorHowl);

        float howlAnimLength = 2.667f;

        yield return new WaitForSeconds(howlAnimLength);

        currentAction = null;
    }

    protected override void OnPlayerDetected()
    {
        currentAction = StartCoroutine(Howl());
    }

    public override void OnFreeze(float freezeTime)
    {
        base.OnFreeze(freezeTime);
        _agent.isStopped = true;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        _agent.isStopped = true;
    }

    public override void ChooseNextAction()
    {
        if (lungeAttackNext) currentAction = StartCoroutine(LungingBite());
        else if (howlNext) currentAction = StartCoroutine(Howl());
        else if (chasePlayerNext) currentAction = StartCoroutine(ChasePlayer());

        else currentAction = StartCoroutine(Delay(3f));
    }

    private void PlayHowlSFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    private void PlayAttackSound()
    {
        var clipToPlay = attackSounds[Random.Range(0, attackSounds.Length)];

        audioSource.PlayOneShot(clipToPlay);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Wolf trigger hit " + other);
        if (other.CompareTag("Player"))
        {
            print("Hit player");
            other.GetComponent<PlayerHealth>().ApplyDamage(_currentDamage);
        }
    }
}

