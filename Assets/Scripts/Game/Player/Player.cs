using System;
using System.Collections;
using Core.Signals;
using UnityEngine;
using Utils;
using Zenject;

namespace Game.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private CharacterController controller;
        [SerializeField] private Animator animator;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private Collider weaponCollider;
        [Range(0.0f, 0.3f)] [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float topClamp = 70.0f;
        [SerializeField] private float bottomClamp = -20.0f;
        [SerializeField] private float сameraAngleOverride = 0.0f;
        [SerializeField] private float speedChangeRate = 10.0f;

        public Transform CameraTarget => cameraTarget;

        private readonly int _runAnimId = Animator.StringToHash("Running");
        private readonly int _attackAnimId = Animator.StringToHash("Attack");
        private readonly int _deathAnimId = Animator.StringToHash("Dead");

        private float _targetRotation;
        private float _rotationVelocity;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _animationBlend;
        private bool _isAttacking;
        private Camera _mainCamera;
        private Coroutine _colliderCoroutine;
        private SignalBus _signalBus;
        
        private readonly WaitForSeconds _colliderWaiter = new (0.5f);
        private readonly Vector3 _gravity = new (0f, -9.8f, 0f);
        private bool _isDead;

        private const float Threshold = 0.01f;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void Initialize(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Respawn(Transform spawn)
        {
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
            
            _isDead = false;
            weaponCollider.enabled = false;
            animator.SetBool(_deathAnimId, false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Look(Vector2 input)
        {
            if (input.sqrMagnitude >= Threshold)
            {
                _cinemachineTargetYaw += input.x;
                _cinemachineTargetPitch += input.y;
            }

            _cinemachineTargetYaw = MyUtils.ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = MyUtils.ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            cameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch + сameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        public void Move(Vector2 input)
        {
            float targetSpeed = input != Vector2.zero ? moveSpeed : 0f;
            float animSpeed = input != Vector2.zero ? 1f : 0f;
            
            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, speedChangeRate * Time.deltaTime);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            
            _animationBlend = Mathf.Lerp(_animationBlend, animSpeed, speedChangeRate * Time.deltaTime);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            
            Vector3 inputDirection = new Vector3(input.x, 0.0f, input.y).normalized;

            if (input != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, 
                    ref _rotationVelocity, rotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            
            controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + _gravity * Time.deltaTime);
            
            animator.SetFloat(_runAnimId, _animationBlend);
        }

        public void Attack()
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                animator.SetTrigger(_attackAnimId);
                Debug.Log("attack");
            }
        }
        
        public void Death()
        {
            _isDead = true;
            Cursor.lockState = CursorLockMode.None;
            animator.SetBool(_deathAnimId, true);
        }

        private void HandleAttackStartAnimTrigger()
        {
            if (_colliderCoroutine != null)
            {
                StopCoroutine(_colliderCoroutine);
            }

            _colliderCoroutine = StartCoroutine(ColliderResetCoroutine());
        }

        private IEnumerator ColliderResetCoroutine()
        {
            weaponCollider.enabled = true;
            yield return _colliderWaiter;
            weaponCollider.enabled = false;
            _isAttacking = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.EnemyWeapon))
            {
                _signalBus.Fire<PlayerHitSignal>();
            }
            else if (other.CompareTag(Tags.Heal))
            {
                _signalBus.Fire(new HealPickUpSignal{HealGameObject = other.gameObject});
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if(_isDead) return;
            
            Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}