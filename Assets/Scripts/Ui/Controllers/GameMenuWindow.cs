using Core.UiSystem;
using Core.UiSystem.Data;
using Core.UiSystem.Interfaces;
using Core.UiSystem.Pool;
using Game.Player;
using Ui.Views;
using UnityEngine;

namespace Ui.Controllers
{
    public class GameMenuWindow : WindowController<GameMenuView>
    {
        private readonly IPlayerHandler _playerHandler;

        public GameMenuWindow(WindowType type, IWindowHandler windowHandler, ViewPool viewPool, 
            IPlayerHandler playerHandler) 
            : base(type, windowHandler, viewPool)
        {
            _playerHandler = playerHandler;
        }

        protected override void OnOpen(object data = null)
        {
            ConcreteView.OnClick += HandleClick;
        }
        
        protected override void OnClose()
        {
            ConcreteView.OnClick -= HandleClick;
        }

        private void HandleClick(GameMenuView.ButtonType type)
        {
            switch (type)
            {
                case GameMenuView.ButtonType.Play:
                    _playerHandler.SpawnPlayer();
                    WindowHandler.OpenWindow(WindowType.PlayerHud);
                    Close();
                    break;
                case GameMenuView.ButtonType.Exit:
                    Application.Quit();
                    break;
            }
        }
    }
}