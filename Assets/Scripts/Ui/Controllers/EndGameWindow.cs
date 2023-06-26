using Core.UiSystem;
using Core.UiSystem.Data;
using Core.UiSystem.Interfaces;
using Core.UiSystem.Pool;
using Game.Mutant.Interfaces;
using Game.Player;
using Ui.Views;
using UnityEngine;

namespace Ui.Controllers
{
    public class EndGameWindow : WindowController<EndGameView>
    {
        private readonly IPlayerHandler _playerHandler;
        private readonly IMutantHandler _mutantHandler;

        public EndGameWindow(WindowType type, IWindowHandler windowHandler, ViewPool viewPool, 
            IPlayerHandler playerHandler, IMutantHandler mutantHandler) 
            : base(type, windowHandler, viewPool)
        {
            _playerHandler = playerHandler;
            _mutantHandler = mutantHandler;
        }

        protected override void OnOpen(object data = null)
        {
            ConcreteView.SetScore((int) data!);

            ConcreteView.OnClick += HandleClick;
        }
        
        protected override void OnClose()
        {
            ConcreteView.OnClick -= HandleClick;
        }

        private void HandleClick(EndGameView.ButtonType type)
        {
            switch (type)
            {
                case EndGameView.ButtonType.Restart:
                    _playerHandler.SpawnPlayer();
                    _mutantHandler.RespawnEnemies();
                    WindowHandler.OpenWindow(WindowType.PlayerHud);
                    Close();
                    break;
                case EndGameView.ButtonType.Exit:
                    Application.Quit();
                    break;
            }
        }
    }
}