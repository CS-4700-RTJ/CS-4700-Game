using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Behaviors
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class WolfBehavior : EnemyBehavior
    {
        [Header("Wolf")]
        [Space]
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
    
        private NavMeshAgent _agent;
        private Animator _animator;

        private static readonly int AnimatorMoveSpeed = Animator.StringToHash("Move Speed");
        private static readonly int AnimatorHowl = Animator.StringToHash("Howl");
        private static readonly int AnimatorLungeAttack = Animator.StringToHash("Lunge");
        private static readonly int AnimatorBiteAttack = Animator.StringToHash("Bite");

        protected override void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();

            base.Start();
        }

        public override void OnFreeze(float freezeTime)
        {
            base.OnFreeze(freezeTime);
            _animator.SetFloat(AnimatorMoveSpeed, 0);
            _agent.isStopped = true;
        }

        public override void OnDeath()
        {
            try
            {
                _agent.isStopped = true;
                _agent.enabled = false;
            }
            finally
            {
                base.OnDeath();
            }
        }

        public override void ChooseNextAction()
        {
            // Available Actions
            // 1 - Lunge attack
            // 2 - Run attack ( ChasePlayer -> Running Bite )
            // 3 - Delay
            // 4 - Howl

            currentAction = StartCoroutine(GetDistanceToPlayer() > idealPlayerDistance ? RunningAttack() : LungingBiteAttack());
        }
        
        protected override void OnPlayerDetected()
        {
            currentAction = StartCoroutine(Howl());
        }

        // Called by Animator Event during Howl animation
        private void PlayHowlSFX(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        // Called by Animator Event during attack animations
        private void PlayAttackSound()
        {
            var clipToPlay = attackSounds[Random.Range(0, attackSounds.Length)];

            audioSource.PlayOneShot(clipToPlay);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().ApplyDamage(_currentDamage);
            }
        }
        
        #region Actions

        /// <summary>
        /// Action that causes the wolf to stand on its hind legs before pouncing and biting at the player.
        /// </summary>
        private IEnumerator LungingBiteAttack()
        {
#if UNITY_EDITOR
            if (debugMessages) print("Action - Lunging Bite");
#endif

            _animator.SetTrigger(AnimatorLungeAttack);
            _currentDamage = lungeDamage;

            float lungeAnimLength = 2f;

            yield return new WaitForSeconds(lungeAnimLength);

            currentAction = null;
        }

        /// <summary>
        /// Action that makes the wolf run towards the player until close, then bite the player.
        /// </summary>
        private IEnumerator RunningAttack()
        {
#if UNITY_EDITOR
            if (debugMessages) print("Action - Running Bite Attack");
#endif

            // Chase the player
            yield return StartCoroutine(ChasePlayer());
            // And then do a running bite
            yield return StartCoroutine(RunningBite());

            currentAction = null;
        }

        /// <summary>
        /// Sub-Action that makes the wolf run at the player while biting.
        /// </summary>
        private IEnumerator RunningBite()
        {
#if UNITY_EDITOR
            if (debugMessages) print("Sub-Action - Running Bite");
#endif

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
        }

        /// <summary>
        /// Sub-Action that makes the wolf run towards the player until within the <b>idealPlayerDistance</b>
        /// </summary>
        private IEnumerator ChasePlayer()
        {
            #if UNITY_EDITOR
            if (debugMessages) print("Sub-Action - Chase Player");
            #endif
            
            _agent.isStopped = false;
        
            while (GetDistanceToPlayer() > idealPlayerDistance)
            {
                _agent.SetDestination(playerTransform.position);
                _agent.speed = GetDistanceToPlayer() >= distanceForRunning ? runSpeed : baseSpeed;
                _animator.SetFloat(AnimatorMoveSpeed, _agent.speed >= runSpeed ? 1 : 0.5f);

                yield return null;
            }
        }

        private IEnumerator Howl()
        {
#if UNITY_EDITOR
            if (debugMessages) print("Action - Howl");
#endif

            _animator.SetTrigger(AnimatorHowl);

            float howlAnimLength = 2.667f;

            yield return new WaitForSeconds(howlAnimLength);

            currentAction = null;
        }

        #endregion
    }
}

