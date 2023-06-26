using System.Collections;
using Core.Signals;
using Game.Player;
using UnityEngine;
using UnityEngine.AI;
using Utils;
using Zenject;

namespace Game.Mutant
{
    public class Mutant : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private Collider bodyCollider;
        [SerializeField] private Collider weaponCollider;
        
        [SerializeField] private float attackDistance = 2f;
        [SerializeField] private float attackRate = 2f;
        [SerializeField] private float attackAngle = 45f;
        [SerializeField] private float rotationSpeed = 10f;

        private int _currentHealth;
        private int _maxHealth;
        private float _nextAttackTime;
        private bool _isAttacking;
        private bool _isDead;
        private IPlayerInfo _playerInfo;
        private SignalBus _signalBus;
        private Coroutine _bodyColliderCoroutine;
        private Coroutine _weaponColliderCoroutine;
        private Coroutine _deathCoroutine;
        
        private readonly WaitForSeconds _bodyColliderWaiter = new (0.5f);
        private readonly WaitForSeconds _weaponColliderWaiter = new (0.2f);
        private readonly int _deadAnimId = Animator.StringToHash("Dead");
        private readonly int _hitAnimId = Animator.StringToHash("Hit");
        private readonly int _attackId = Animator.StringToHash("Attack");
        private readonly int _runId = Animator.StringToHash("Running");

        public void Initialize(IPlayerInfo playerInfo, SignalBus signalBus)
        {
            _signalBus = signalBus;
            _playerInfo = playerInfo;
        }
        
        public void Respawn(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _isDead = false;
            bodyCollider.enabled = true;
            weaponCollider.enabled = false;
            animator.SetBool(_deadAnimId, false);
            healthBar.Enable();
            healthBar.UpdateBar();
        }

        private void Update()
        {
            if (_playerInfo.Transform != null && !_playerInfo.IsDead && !_isDead)
            {
                float distanceToTarget = Vector3.Distance(transform.position, _playerInfo.Transform.position);
                
                Vector3 direction = _playerInfo.Transform.position - transform.position;
                direction.y = 0f;

                if (direction != Vector3.zero)
                {
                    float angle = Vector3.Angle(transform.forward, direction);

                    Quaternion toRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                    
                    if (angle <= attackAngle)
                    {
                        if (distanceToTarget <= attackDistance && !_isAttacking)
                        {
                            if (Time.time >= _nextAttackTime)
                            {
                                Attack();
                            }
                        }
                        else if (!_isAttacking)
                        {
                            Move();
                        }
                    }
                }
            }
            else if (!agent.isStopped)
            {
                agent.isStopped = true;
                animator.SetFloat(_runId, 0f);
            }
        }

        private void Move()
        {
            if (agent.isStopped)
                agent.isStopped = false;
            
            agent.SetDestination(_playerInfo.Transform.position);
            animator.SetFloat(_runId, 1f);
        }

        private void Attack()
        {
            _isAttacking = true;
            animator.SetTrigger(_attackId);
            _nextAttackTime = Time.time + 1f / attackRate;
            animator.SetFloat(_runId, 0f);
        }

        private void HandleAnimStart()
        {
            if (_weaponColliderCoroutine != null)
            {
                StopCoroutine(_weaponColliderCoroutine);
            }

            _weaponColliderCoroutine = StartCoroutine(WeaponColliderResetCor());
        }
        
        private void HandleAnimEnd()
        {
            _isAttacking = false;
        }

        private void HandleHit()
        {
            animator.SetTrigger(_hitAnimId);
            weaponCollider.enabled = false;

            _isAttacking = false;
            _nextAttackTime = Time.time + 1f / attackRate;
            
            _currentHealth -= 1;
            healthBar.UpdateBar((float)_currentHealth / _maxHealth);

            if (_currentHealth == 0)
            {
                Death();
                return;
            }
            
            if (_bodyColliderCoroutine != null)
            {
                StopCoroutine(_bodyColliderCoroutine);
            }

            _bodyColliderCoroutine = StartCoroutine(BodyColliderResetCor());
            
        }

        private IEnumerator BodyColliderResetCor()
        {
            bodyCollider.enabled = false;
            yield return _bodyColliderWaiter;
            bodyCollider.enabled = true;
        }
        
        private IEnumerator WeaponColliderResetCor()
        {
            weaponCollider.enabled = true;
            yield return _weaponColliderWaiter;
            weaponCollider.enabled = false;
        }

        private void Death()
        {
            _isDead = true;
            animator.SetBool(_deadAnimId, true);
            healthBar.Disable();
            bodyCollider.enabled = false;
            
            _signalBus.Fire(new EnemyDeathSignal { Mutant = this });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.PlayerWeapon) && !_isDead)
            {
                HandleHit();
            }
        }
    }
}
