using Core.UiSystem.Data;
using Core.UiSystem.Interfaces;
using Zenject;

namespace Starters
{
    public class GameStarter : IInitializable
    {
        private readonly IWindowHandler _windowHandler;

        public GameStarter(IWindowHandler windowHandler)
        {
            _windowHandler = windowHandler;
        }

        public void Initialize()
        {
            _windowHandler.OpenWindow(WindowType.GameMenu);
        }
    }
}
