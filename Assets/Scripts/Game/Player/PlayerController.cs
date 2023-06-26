using Core.Signals;
using UnityEngine;
using Zenject;

namespace Game.Player
{
    public class PlayerController
    {
        private readonly SignalBus _signalBus;
        private readonly Player _player;

        private float _smoothingVelocity;
        private float _currentHealth;
        private float _maxHealth;

        public bool IsDead => _currentHealth <= 0;

        public PlayerController(SignalBus signalBus, Player player)
        {
            _signalBus = signalBus;
            _player = player;

            Subscribe();
        }

        ~PlayerController()
        {
            Unsubscribe();
        }

        public void Initialize(int health)
        {
            _maxHealth = health;
            _currentHealth = health;
            _player.Initialize(_signalBus);
        }

        public void Respawn(Transform spawn)
        {
            _currentHealth = _maxHealth;
            _player.Respawn(spawn);
        }

        private void Subscribe()
        {
            _signalBus.Subscribe<MovementSignal>(HandleMovement);
            _signalBus.Subscribe<LookSignal>(HandleLook);
            _signalBus.Subscribe<AttackSignal>(HandleAttack);
            _signalBus.Subscribe<PlayerHitSignal>(HandlePlayerHit);
            _signalBus.Subscribe<HealPickUpSignal>(HandleHealPickUp);
        }

        private void Unsubscribe()
        {
            _signalBus.Unsubscribe<MovementSignal>(HandleMovement);
            _signalBus.Unsubscribe<LookSignal>(HandleLook);
            _signalBus.Unsubscribe<AttackSignal>(HandleAttack);
            _signalBus.Unsubscribe<PlayerHitSignal>(HandlePlayerHit);
            _signalBus.Unsubscribe<HealPickUpSignal>(HandleHealPickUp);
        }

        private void HandleHealPickUp()
        {
            if(IsDead && _currentHealth >= _maxHealth) return;
            
            _currentHealth++;
        }

        private void HandlePlayerHit()
        {
            if(IsDead) return;
            
            _currentHealth -= 1;

            if (_currentHealth == 0)
            {
                _player.Death();
            }
        }
        
        private void HandleMovement(MovementSignal signal)
        {
            if(!IsDead)
                _player.Move(signal.Input);
        }
        
        private void HandleLook(LookSignal signal)
        {
            if(!IsDead)
                _player.Look(signal.Input);
        }

        private void HandleAttack()
        {
            if(!IsDead)
                _player.Attack();
        }

    }
}
