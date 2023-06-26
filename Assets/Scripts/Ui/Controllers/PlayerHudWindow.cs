using Core.Signals;
using Core.UiSystem;
using Core.UiSystem.Data;
using Core.UiSystem.Interfaces;
using Core.UiSystem.Pool;
using Game.Player;
using Ui.Views;
using Zenject;

namespace Ui.Controllers
{
    public class PlayerHudWindow : WindowController<PlayerHudView>
    {
        private int _score;
        private int _health;
        
        private readonly SignalBus _signalBus;
        private readonly IPlayerInfo _playerInfo;

        public PlayerHudWindow(WindowType type, IWindowHandler windowHandler, ViewPool viewPool,
            SignalBus signalBus, IPlayerInfo playerInfo) 
            : base(type, windowHandler, viewPool)
        {
            _signalBus = signalBus;
            _playerInfo = playerInfo;
        }

        protected override void OnOpen(object data = null)
        {
            _health = _playerInfo.MaxHealth;
            _score = 0;
            ConcreteView.UpdateScore(_score);
            ConcreteView.UpdateHealthBar((float)_health / _playerInfo.MaxHealth);
            
            _signalBus.Subscribe<EnemyDeathSignal>(HandleEnemyDeath);
            _signalBus.Subscribe<PlayerHitSignal>(HandlePlayerHit);
            _signalBus.Subscribe<HealPickUpSignal>(HandleHealPickUp);
        }

        protected override void OnClose()
        {
            _signalBus.Unsubscribe<EnemyDeathSignal>(HandleEnemyDeath);
            _signalBus.Unsubscribe<PlayerHitSignal>(HandlePlayerHit);
            _signalBus.Unsubscribe<HealPickUpSignal>(HandleHealPickUp);
        }

        private void HandleHealPickUp()
        {
            _health ++;
            ConcreteView.UpdateHealthBar((float) _health / _playerInfo.MaxHealth);
        }

        private void HandleEnemyDeath()
        {
            _score += 1;
            ConcreteView.UpdateScore(_score);
        }
        
        private void HandlePlayerHit()
        {
            _health -= 1;
            ConcreteView.UpdateHealthBar((float) _health / _playerInfo.MaxHealth);

            if (_health == 0)
            {
                Close();
                WindowHandler.OpenWindow(WindowType.EndGame, _score);
            }
        }
    }
}