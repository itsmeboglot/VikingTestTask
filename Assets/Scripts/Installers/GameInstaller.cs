using Core.InputSystem;
using Game.Player;
using Starters;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameStarter>().AsSingle().NonLazy();
            Container.BindInterfacesTo<InputHandler>().AsSingle().NonLazy();
        }
    }
}