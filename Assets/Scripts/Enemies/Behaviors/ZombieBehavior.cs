using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.Behaviors
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieBehavior : EnemyBehavior
    {
        private static readonly int AnimatorMoveSpeed = Animator.StringToHash("Move Speed");
        private static readonly int AnimatorAttackTrigger = Animator.StringToHash("Attack");

        [Header("Zombie")]
        [Space]
        [Header("Attacks")] 
        public AudioClip[] attackSounds;
        public float idealPlayerDistance = 2f;
        public float attackDamage = 2f;

        [Header("Death")] 
        public GameObject[] clothesObjects;

        private NavMeshAgent _agent;
        private Animator _animator;

        protected override void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();

            base.Start();
        }

        public override void OnFreeze(float freezeTime)
        {
            StopAction();
            _agent.isStopped = true;
        }

        public override void OnDeath()
        {
            foreach (var clothing in clothesObjects)
            {
                Destroy(clothing);
            }
            
            _agent.isStopped = true;
            _agent.enabled = false;
            StopAllCoroutines();
            base.OnDeath();
        }



        public override void ChooseNextAction()
        {
            // Only has chase player -> attack when close action
            currentAction = StartCoroutine(ChaseAndAttackAction());
        }

        private void PlayAttackSound()
        {
            var clipToPlay = attackSounds[Random.Range(0, attackSounds.Length)];

            audioSource.PlayOneShot(clipToPlay);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().ApplyDamage(attackDamage);
            }
        }

        #region Actions

        private IEnumerator ChaseAndAttackAction()
        {
#if UNITY_EDITOR
            if (debugMessages) print("Action - Chase and Attack");
#endif

            yield return StartCoroutine(ChasePlayer());

            yield return StartCoroutine(SwipeAttack());

            currentAction = null;
        }

        private IEnumerator ChasePlayer()
        {
#if UNITY_EDITOR
            if (debugMessages) print("Sub-Action - Chase Player");
#endif

            // make the zombie move again
            _agent.isStopped = false;
            _animator.SetFloat(AnimatorMoveSpeed, 1f);

            // Move to the player until within the ideal player distance
            while (GetDistanceToPlayer() > idealPlayerDistance)
            {
                _agent.SetDestination(playerTransform.position);

                yield return null;
            }
        }

        private IEnumerator SwipeAttack()
        {
#if UNITY_EDITOR
            if (debugMessages) print("Sub-Action - Swipe Attack");
#endif

            _animator.SetTrigger(AnimatorAttackTrigger);
            _agent.isStopped = false;
            _animator.SetFloat(AnimatorMoveSpeed, 1f);

            float attackAnimLength = 1f;
            
            PlayAttackSound();

            while (attackAnimLength >= 0)
            {
                _agent.SetDestination(playerTransform.position);

                yield return null;

                attackAnimLength -= Time.deltaTime;
            }
            
            _animator.SetFloat(AnimatorMoveSpeed, 0);
            _agent.isStopped = true;

            currentAction = StartCoroutine(Delay(0.5f));
        }

        #endregion
    }
}